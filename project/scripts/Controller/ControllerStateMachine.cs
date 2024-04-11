using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sylves;
using Tessera;
using TMPro;

namespace SlimeGame
{
    [ShowOdinSerializedPropertiesInInspector]
    public class ControllerStateMachine : SerializedMonoBehaviour 
    {
        private static Dictionary<ControllerStates,Keys> DefaultControllerStatesToKeys => new()
        {
            { ControllerStates.Place      ,Keys.num1},
            { ControllerStates.Mine       ,Keys.num2},
            { ControllerStates.Paint      ,Keys.num3},
            { ControllerStates.Move       ,Keys.num4},
            { ControllerStates.Select     ,Keys.num5},
            { ControllerStates.Regenerate ,Keys.num6},
        };
        private static Dictionary<UIObjectType,GameObject[]> DefaultUIObjectTypeToObjects => new ()
        {
            { UIObjectType.HUD         , new GameObject[0] },
            { UIObjectType.Retical     , new GameObject[0] },
            { UIObjectType.Hotbar      , new GameObject[0] },
            { UIObjectType.MenuHolder  , new GameObject[0] },
            { UIObjectType.MainMenu    , new GameObject[0] },
            { UIObjectType.SubMenu     , new GameObject[0] },
            { UIObjectType.NameDisplay , new GameObject[0] },
            { UIObjectType.ModeDisplay , new GameObject[0] },
        };
        private static Dictionary<UITextType,TMP_Text> DefaultUITextTypeToText => new()
        {
            { UITextType.SelectedName           , null },
            { UITextType.CurrentModeName        , null },
            { UITextType.CurrentGeneratorType   , null },
            { UITextType.CurrentGeneratorName   , null },
        };
        private static Dictionary<ControllerStates,Dictionary<Enum,GameObject>> DefaultSubStateIconObjects => new ()
        {
            { ControllerStates.Place      , new () { { PlaceStates.Bound        ,null },{ PlaceStates.AirCell        ,null } } },
            { ControllerStates.Mine       , new () { } },
            { ControllerStates.Paint      , new () { { PaintStates.Floor        ,null },{ PaintStates.Wall           ,null },{ PaintStates.Ceiling   ,null } } },
            { ControllerStates.Move       , new () { { MoveStates.Relocate      ,null },{ MoveStates.Resize          ,null } } },
            { ControllerStates.Select     , new () { { SelectStates.Single      ,null },{ SelectStates.Multiple      ,null },{ SelectStates.Children ,null } } },
            { ControllerStates.Regenerate , new () { { RegenerateStates.Bounded ,null },{ RegenerateStates.Unbounded ,null } } },
        };

        [FoldoutGroup("References",order:0)]
        [OdinSerialize, LabelWidth(150)]
        private readonly GenerationManager _generationManager;

        [Header("Cameras"),FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly CinemachineVirtualCamera _freeCamera;

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly CinemachineVirtualCamera _dollyCamera;

        [Header("Windows"),FoldoutGroup("References")]
        [OdinSerialize,LabelWidth(150)]
        private readonly Dictionary<UIObjectType,GameObject[]> _uiObjectTypeToObjects = DefaultUIObjectTypeToObjects;

        [Header("UI Text"),FoldoutGroup("References")]
        [OdinSerialize,LabelWidth(150)]
        private readonly Dictionary<UITextType,TMP_Text> _uiTextTypeToText = DefaultUITextTypeToText;

        [Header("Hot Bar"),FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150)]
        private readonly Dictionary<ControllerStates,Keys> _controllerStatesToKeys = DefaultControllerStatesToKeys;

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150)]
        private readonly Dictionary<Keys,GameObject> _activeSlotObjects = new ();

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150)]
        private readonly Dictionary<ControllerStates,Dictionary<Enum,GameObject>> _subStateIconObjects = DefaultSubStateIconObjects;

        [FoldoutGroup("Options",order:0)]
        [OdinSerialize,LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly  bool _useDollyCameraMode;

        [Header("Movement"),FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly  Vector2 _mouseSensitivity = new (.1f,.1f);

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly  Vector3 _movementSpeed = new (15,15,15);

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly  float _sprintMultiplier = 5;

        [Header("Raycasting"),FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly  float _maxCastDistance = 40;

        [FoldoutGroup("Options")]
        [OdinSerialize, LabelWidth(150)]
        private readonly  float _sphereCastRadius = .05f;

        [FoldoutGroup("Options")]
        [OdinSerialize,HideLabel,Title("Selected Cell Outline Color")]
        private readonly Color _selectedCellOutlineColor = new (1,0,1,.75f);

        private Dictionary<Keys,ControllerStates> _keysToControllerStates;
        private ControllerStateFactory _stateFactory;
        private ControllerBaseState _currentState;
        private ControllerInput _input;
        private GameObject _selectedCellObject;
        private CubeGizmoComponent _selectedCellGizmo;
        private CubeGrid _cellGrid;
        private Vector3Int _cellSize;
        private bool _canMove   = false;
        private bool _canPlace  = false;
        private bool _paused    = false;
#pragma warning disable
        private Vector2 _lookInput;
        private Vector2 _moveInput;
        private Vector2 _scrollInput;
        private bool _isMovePressed;
        private bool _isScrollStarted;
        private bool _jumpStarted;
        private bool _isJumpPressed;
        private bool _isCrouchStarted;
        private bool _isCrouchPressed;  
        private bool _isSprintStarted;
        private bool _isSprintPressed;
        private bool _isLeftClickStarted;
        private bool _isLeftClickPressed;
        private bool _isRightClickStarted;
        private bool _isRightClickPressed;
        private bool _isEPressed;
#pragma warning restore

        public GenerationManager GenerationManager { get { return _generationManager; } }
        public Vector2 ScrollInput { get { return _scrollInput; } }
        public bool IsEPressed { get { return _isEPressed; } }
        public void SetCurrentState(ControllerBaseState newSuperState) => _currentState = newSuperState;

        void OnEnable() 
        {
            _input = new ControllerInput();
            _input.Editor.Enable();
        }
        void OnDisable() 
        {
            _input.Editor.Disable();
        }
        private void Awake()
        {
            _paused = false;
            SetCursorLock(true);
            SetUIObjectsActive(UIObjectType.SubMenu,false);
            SetUIObjectsActive(UIObjectType.MenuHolder,false);
            SetUIObjectsActive(UIObjectType.MainMenu,true);
            SetUIObjectsActive(UIObjectType.ModeDisplay,true);
            SetAllSelectedSlotObjectsActive(false);
            SetAllSubStateIconObjectsActive(false);
            InitializeKeysToControllerStates();
            InitializeStateFactory(ControllerStates.Default);
        }
        void Start()
        {
            _cellGrid = _generationManager.CellGrid;
            _cellSize = _generationManager.CellSize;
            InitializeSelectedCellOutline();
            InitializeInputCallBacks();
            SetUIText(UITextType.CurrentGeneratorType,_generationManager.GeneratorType.ToString() + " Generators");
            SetCurrentGeneratorNameUIText();
        }
        void Update()
        {
            if (_canMove)
            {
                HandleAim();
                HandleMovement();
            }
           
            HandleSelectedCellOutline();
        }
        void FixedUpdate()
        {
            if (_isScrollStarted)
            {
                _currentState.HandleScrollInput();
                _isScrollStarted = false;
            }
        }

        public void SetAllSelectedSlotObjectsActive(bool isActive)
        {
            foreach (var gameObject in _activeSlotObjects.Values.Where(go => go != null && go.activeSelf != isActive))
            {
                gameObject.SetActive(isActive);
            }
        }
        public void SetAllSubStateIconObjectsActive(bool isActive)
        {
            foreach (var dictionary in _subStateIconObjects.Values.Where(d => d != null && d.Count > 0))
            {
                foreach (var iconObject in dictionary.Values.Where(go => go != null && go.activeSelf != isActive))
                {
                    iconObject.SetActive(isActive);
                }
            }
        }
        public void InitializeKeysToControllerStates()
        {
            _keysToControllerStates = new();
            foreach (var (states, keys) in _controllerStatesToKeys)
            {
                _keysToControllerStates.Add(keys,states);
            }
        }
        public void InitializeStateFactory(ControllerStates firstStates = ControllerStates.Default)
        {
            _stateFactory = new ControllerStateFactory(this);
            _currentState = _stateFactory.GetSuperState(firstStates);
            _currentState.EnterState();
        }
        public void InitializeSelectedCellOutline()
        {
            _selectedCellObject = new GameObject("Selected Cell Outline");
            _selectedCellGizmo = _selectedCellObject.AddComponent<CubeGizmoComponent>();
            _selectedCellGizmo.UpdateColor(_selectedCellOutlineColor);
            _selectedCellGizmo.SetEnabled(false);
            var min = Vector3.Scale(Vector3Int.zero,_cellSize);
            var center = min + Vector3.Scale(_cellSize,new Vector3(.5f,.5f,.5f));
            var bounds = new Bounds(center,_cellSize);
            _selectedCellGizmo.UpdateBounds(bounds);
            _selectedCellGizmo.SetEnabled(false);
        }
        private void InitializeInputCallBacks()
        {
            #region General Input
            _input.Editor.Enter.started += OnEnterInput;
            _input.Editor.Esc.started += OnEscInput;
            _input.Editor.Arrows.started += OnArrowsInput;
            #endregion
            #region Movement Input
            _input.Editor.Look.started += OnLookInput;
            _input.Editor.Look.performed += OnLookInput;
            _input.Editor.Look.canceled += OnLookInput;
            _input.Editor.Move.started += OnMoveInput;
            _input.Editor.Move.performed += OnMoveInput;
            _input.Editor.Move.canceled += OnMoveInput;
            _input.Editor.Crouch.started += OnCrouchInput;
            _input.Editor.Crouch.performed += OnCrouchInput;
            _input.Editor.Crouch.canceled += OnCrouchInput;
            _input.Editor.Jump.started += OnJumpInput;
            _input.Editor.Jump.performed += OnJumpInput;
            _input.Editor.Jump.canceled += OnJumpInput;
            _input.Editor.Sprint.started += OnSprintInput;
            _input.Editor.Sprint.performed += OnSprintInput;
            _input.Editor.Sprint.canceled += OnSprintInput;
            #endregion
            #region Mouse Input
            _input.Editor.LeftMouse.started += OnLeftMouseInput;
            _input.Editor.RightMouse.started += OnRightMouseInput;
            _input.Editor.Scroll.performed += OnScrollInput;
            #endregion
            #region Number Input
            _input.Editor.One.started += OnOneInput;
            _input.Editor.Two.started += OnTwoInput;
            _input.Editor.Three.started += OnThreeInput;
            _input.Editor.Four.started += OnFourInput;
            _input.Editor.Five.started += OnFiveInput;
            _input.Editor.Six.started += OnSixInput;
            _input.Editor.Seven.started += OnSevenInput;
            _input.Editor.Eight.started += OnEightInput;
            _input.Editor.Nine.started += OnNineInput;
            _input.Editor.Zero.started += OnZeroInput;
            #endregion
            #region Letter Input
            _input.Editor.Q.performed += OnQInput;
            _input.Editor.E.performed += OnEInput;
            _input.Editor.R.started += OnRInput;
            _input.Editor.T.started += OnTInput;
            _input.Editor.F.started += OnFInput;
            _input.Editor.G.started += OnGInput;
            _input.Editor.H.started += OnHInput;
            _input.Editor.N.started += OnNInput;
            #endregion
        }

#pragma warning disable
        /// <summary>
        /// ( !! ) Keep => Called by <see cref="OnValueChangedAttribute"/> when <see cref="_freeCamera"/> &amp; <see cref="_dollyCamera"/> are changed in inspector
        /// </summary>
        private void UpdateCameraProperties()
        {
            if ((_useDollyCameraMode && _dollyCamera != null) || (_dollyCamera != null && _freeCamera == null))
            {
                _dollyCamera.Priority = 100;
                if (_freeCamera != null)
                {
                    _freeCamera.Priority = 0;
                }
            }
            else if (_freeCamera != null)
            {
                _freeCamera.Priority = 100;
                if (_dollyCamera != null)
                {
                    _dollyCamera.Priority = 0;
                }
            }
        }
#pragma warning restore

        public GameObject GetSelectedSlotObject(ControllerStates states)
        {
            _controllerStatesToKeys.TryGetValue(states,out var keys);
            return _activeSlotObjects.TryGetValue(keys,out var obj) ? obj : null;
        }
        public GameObject GetSubStateIconObject(ControllerStates controllerStates,Enum subStates)
        {
            if (_subStateIconObjects.TryGetValue(controllerStates,out var subStateToIconObject))
            {
                if (subStateToIconObject.TryGetValue(subStates,out var iconObject))
                {
                    return iconObject;
                }
            }
            return null;
        }
        private ControllerStates GetStateFromKeys(Keys keys)
        {
            return _keysToControllerStates.TryGetValue(keys,out var states) ? states : ControllerStates.None;
        }
        public Vector3Int GetCellOffsetFromScrollInput()
        {
            var moveVector = MoveHelper.NeighborCellOffsetFromVector(transform.forward);
            var scrollValue = ScrollInput.y;
            if (scrollValue < 0)
            {
                moveVector = -moveVector;
            }
            return moveVector;
        }
        public Vector3Int GetCellFromPosition(Vector3 position)
        {
            _cellGrid.FindCell(position,out var newCenterCell);
            return (Vector3Int)newCenterCell;
        }
        public void GetEnabledCollidersValue(out ColliderTypes selected,out ColliderTypes unselected)
        {
            _currentState.GetEnabledColliders(out selected,out unselected);
        }
        public BaseGenerationInstance GetLastSelectedInstance()
        {
            return _generationManager.InstanceManager.GetLastSelectedInstance();
        }
        public BaseGenerationInstance GetFirstSelectedInstance(Type type)
        {
            _generationManager.InstanceManager.TryGetFirstSelected(type,out var instance);
            return instance;
        }

        private void HandleAim()
        {
            transform.rotation *= Quaternion.AngleAxis(_lookInput.x * _mouseSensitivity.x,Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(-_lookInput.y * _mouseSensitivity.y,Vector3.right);
            transform.localEulerAngles = new(transform.localEulerAngles.x,transform.localEulerAngles.y,0);
        }
        private void HandleMovement()
        {
            if (!_isMovePressed)
            {
                return;
            }
            float horizontalSpeed = _isSprintPressed ? _movementSpeed.x * _sprintMultiplier : _movementSpeed.x;
            float verticalSpeed   = _isSprintPressed ? _movementSpeed.y * _sprintMultiplier : _movementSpeed.y;
            var appliedMovement = Vector3.zero;
            appliedMovement.x = _moveInput.x * (horizontalSpeed * Time.deltaTime);
            appliedMovement.z = _moveInput.y * (horizontalSpeed * Time.deltaTime);
            appliedMovement = (transform.right * appliedMovement.x) + (transform.up * appliedMovement.y) + (transform.forward * appliedMovement.z);
            var newPosition = transform.position += appliedMovement;
            newPosition.y -= _isCrouchPressed ? (verticalSpeed * Time.deltaTime) : 0;
            newPosition.y += _isJumpPressed   ? (verticalSpeed * Time.deltaTime) : 0;
            transform.position = newPosition;            
        }
        private void HandleSelectedCellOutline()
        {
            var gizmoEnabled = TryRaycastForInstance(out var hit,out var instance) ? Hit() : Miss();
            _selectedCellGizmo.SetEnabled(gizmoEnabled);
            bool Hit()
            {
                var hitCell = _cellGrid.FindCell(hit.point);
                if (hitCell != null)
                {
                    var center = Vector3.Scale((Vector3Int)hitCell + new Vector3(.5f,.5f,.5f),_cellSize);
                    _selectedCellGizmo.UpdateBounds(new Bounds(center,_cellSize));
                    if (instance != null)
                    {
                        SetUIText(UITextType.SelectedName,instance.DisplayName);
                        SetUIObjectsActive(UIObjectType.NameDisplay,true);
                        return true;
                    }
                }
                SetUIObjectsActive(UIObjectType.NameDisplay,false);
                return true;
            }
            bool Miss()
            {
                var selectedInstance = _generationManager.InstanceManager.SelectedInstance;
                if (selectedInstance != null && selectedInstance.MainObject != null)
                {
                    SetUIText(UITextType.SelectedName,selectedInstance.DisplayName);
                    return false;
                }
                SetUIObjectsActive(UIObjectType.NameDisplay,false);
                return false;
            }
        }

        /// <summary>
        /// Only returns <see langword="true"/> if :
        /// <br/><br/> <see cref="Physics.SphereCast"/> hit == <see langword="true"/>
        /// <br/> &amp;&amp;
        /// <br/> <see cref="BaseGenerationInstance"/> instance != null
        /// </summary>
        public bool TryRaycastForInstance(out RaycastHit hit,out BaseGenerationInstance instance)
        {
            if (Physics.SphereCast(transform.position,_sphereCastRadius,transform.forward,out hit,_maxCastDistance))
            {
                if (_generationManager.InstanceManager.TryGetInstanceFromGameObject(hit.transform.gameObject,out instance) && instance != null)
                {
                    return true;
                }
            }
            instance = null;
            return false;
        }

        #region => on Key Inputs

        private void OnEnterInput(InputAction.CallbackContext context) 
        {

        }
        private void OnEscInput(InputAction.CallbackContext context)
        {
            if (_paused) 
            {
                Unpause();
                return;
            }
            Pause();
        }
        /// <summary>
        /// TODO: Seperate out into individual buttons
        /// </summary>
        private void OnArrowsInput(InputAction.CallbackContext context)
        {

        }
        private void OnLookInput(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            _isMovePressed = _moveInput.x != 0 || _moveInput.y != 0;
        }
        private void OnLeftMouseInput(InputAction.CallbackContext context)
        {
            _isLeftClickPressed = context.ReadValueAsButton();
            _isLeftClickStarted = context.phase == InputActionPhase.Started;
            if (_paused) 
            {
                return;
            }
            SetCursorLock(true);
            if (!_canPlace)
            {
                return;
            }
            if (context.phase == InputActionPhase.Started)
            {
                _currentState.HandleLeftMouseInputStarted();
            }
        }
        private void OnRightMouseInput(InputAction.CallbackContext context)
        {
            _isRightClickPressed = context.ReadValueAsButton();
            _isRightClickStarted = context.phase == InputActionPhase.Started;
        }
        private void OnScrollInput(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            if (value.y != 0) 
            {
                _scrollInput = value;
                _isScrollStarted = true;
            }
        }
        private void OnSprintInput(InputAction.CallbackContext context)
        {
            _isSprintPressed = context.ReadValueAsButton();
            _isSprintStarted = context.phase == InputActionPhase.Started;
        }
        private void OnCrouchInput(InputAction.CallbackContext context)
        {
            _isCrouchPressed = context.ReadValueAsButton();
            _isCrouchStarted = context.phase == InputActionPhase.Started;
        }
        private void OnJumpInput(InputAction.CallbackContext context)
        {
            _isJumpPressed = context.ReadValueAsButton();
            _jumpStarted = context.phase == InputActionPhase.Started;
        }

        #endregion
        #region => On Number Inputs

        private void OnOneInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num1));
            }
        }
        private void OnTwoInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num2));
            }
        }
        private void OnThreeInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num3));
            }
        }
        private void OnFourInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num4));
            }
        }
        private void OnFiveInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num5));
            }
        }
        private void OnSixInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                _currentState.HandleKeysInput(GetStateFromKeys(Keys.num6));
            }
        }
        private void OnSevenInput(InputAction.CallbackContext context) 
        {

        }
        private void OnEightInput(InputAction.CallbackContext context) 
        {

        }
        private void OnNineInput(InputAction.CallbackContext context) 
        {

        }
        private void OnZeroInput(InputAction.CallbackContext context)
        {

        }

        #endregion
        #region => On Letter Input

        private void OnEInput(InputAction.CallbackContext context)
        {
            _isEPressed = context.ReadValueAsButton();
        }
        private void OnFInput(InputAction.CallbackContext context) 
        {

        }
        private void OnGInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                ToggleUIObjectActive(UIObjectType.Hotbar);
            }
        }
        private void OnHInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                ToggleUIObjectActive(UIObjectType.HUD);
            }
        }
        private void OnNInput(InputAction.CallbackContext context) 
        {

        }
        private void OnQInput(InputAction.CallbackContext context) 
        { 
                    
        }
        private void OnRInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) 
            {
                ToggleUIObjectActive(UIObjectType.Retical);
            }
        }
        private void OnTInput(InputAction.CallbackContext context) 
        { 

        }

        #endregion

        public void Pause()
        {  
            SetUIObjectsActive(UIObjectType.MenuHolder,true);
            SetUIObjectsActive(UIObjectType.Retical,false);
            SetCursorLock(false);
            _paused = true;
        }
        public void Unpause()
        {
            SetUIObjectsActive(UIObjectType.MainMenu,true);
            SetUIObjectsActive(UIObjectType.SubMenu,false);
            SetUIObjectsActive(UIObjectType.MenuHolder,false);
            SetUIObjectsActive(UIObjectType.Retical,true);
            SetCursorLock(true);
            _paused = false;
        }
        private void SetCursorLock(bool isLocked)
        {
            _canMove = isLocked;
            _canPlace = isLocked;
            Cursor.visible = !isLocked;
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        private void ToggleUIObjectActive(UIObjectType type)
        {
            if (_uiObjectTypeToObjects.TryGetValue(type,out var goArray) && goArray != null)
            {
                foreach (var menuObject in goArray.Where(x => x != null))
                {
                    menuObject.SetActive(!menuObject.activeSelf);
                }
            }
        }
        public void SetUIObjectsActive(UIObjectType type, bool isActive)
        {
            if (_uiObjectTypeToObjects.TryGetValue(type,out var goArray) && goArray != null)
            {
                foreach (var menuObject in goArray.Where(x => x != null && x.activeSelf != isActive))
                {
                    menuObject.SetActive(isActive);
                }
            }
        }

        public void SetDefaultGeneratorTypeUIText()
        {
            SetUIText(UITextType.CurrentGeneratorType,"Default Generators");
            SetCurrentGeneratorNameUIText();
        }
        public void SetBigTileGeneratorTypeUIText()
        {
            SetUIText(UITextType.CurrentGeneratorType,"Big Tile Generators");
            SetCurrentGeneratorNameUIText();
        }
        public void SetAdjacentModelGeneratorTypeUIText() 
        {
            SetUIText(UITextType.CurrentGeneratorType,"Adjacent Model Generators");
            SetCurrentGeneratorNameUIText();
        } 
        public void SetCurrentGeneratorNameUIText()
        {
            if (_generationManager.SurfaceGenerator != null) 
            {
                SetUIText(UITextType.CurrentGeneratorName,_generationManager.SurfaceGenerator.name);
                return;
            }
            SetUIText(UITextType.CurrentGeneratorName,"_");
        } 
        public void SetUIText(UITextType type,string text)
        {
            if (_uiTextTypeToText.TryGetValue(type,out var tmpText))
            {
                if (tmpText != null)
                {
                    tmpText.text = text;
                }
            }
        }
    
    }
}