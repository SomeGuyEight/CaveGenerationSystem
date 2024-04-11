using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sylves;
using TMPro;

namespace SlimeGame
{
    [ShowOdinSerializedPropertiesInInspector]
    public class BasicCameraController : SerializedMonoBehaviour
    {
        private static Dictionary<UITextType,TMP_Text> DefaultUITextTypeToText => new()
        {
            { UITextType.SelectedName           , null },
        }; 
        private static Dictionary<UIObjectType,GameObject[]> DefaultUIObjectTypeToObjects => new()
        {
            { UIObjectType.HUD         , new GameObject[0] },
            { UIObjectType.MenuHolder  , new GameObject[0] },
            { UIObjectType.Retical     , new GameObject[0] },
            { UIObjectType.NameDisplay , new GameObject[0] },
        };

        [Header("Cameras"),FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly CinemachineVirtualCamera _freeCamera;

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly CinemachineVirtualCamera _dollyCamera;

        [Header("UI Objects"),FoldoutGroup("References")]
        [OdinSerialize,LabelWidth(150)]
        private readonly Dictionary<UIObjectType,GameObject[]> _uiObjectTypeToObjects = DefaultUIObjectTypeToObjects;

        [Header("UI Text"),FoldoutGroup("References")]
        [OdinSerialize,LabelWidth(150)]
        private readonly Dictionary<UITextType,TMP_Text> _uiTextTypeToText = DefaultUITextTypeToText;

        [FoldoutGroup("Options",order:0)]
        [OdinSerialize,LabelWidth(150)]
        private Vector3Int _cellSize = Vector3Int.one;

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150),OnValueChanged("UpdateCameraProperties")]
        private readonly  bool _useDollyCameraMode = false;

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

        private ControllerInput _input;
        private GameObject _selectedCellObject;
        private CubeGizmoComponent _selectedCellGizmo;
        private CubeGrid _cellGrid;
        private bool _canMove   = false;
        private bool _paused    = false;
        private Vector2 _lookInput;
        private Vector2 _moveInput;
        private bool _isMovePressed;
        private bool _isJumpPressed;
        private bool _isCrouchPressed;
        private bool _isSprintPressed;

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
            Unpause();
            SetUIObjectsActive(UIObjectType.NameDisplay,true);
        }
        void Start()
        {
            _cellGrid = new (_cellSize);
            InitializeSelectedCellOutline();
            InitializeInputCallBacks();
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
            _input.Editor.Esc.started += OnEscInput;

            _input.Editor.Look.performed += OnLookInput;
            _input.Editor.Look.canceled += OnLookInput;
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

            _input.Editor.R.started += OnRInput;
            _input.Editor.R.canceled += OnRInput;
            _input.Editor.H.started += OnHInput;
            _input.Editor.H.canceled += OnHInput;
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

        public Vector3Int GetCellFromPosition(Vector3 position)
        {
            _cellGrid.FindCell(position,out var newCenterCell);
            return (Vector3Int)newCenterCell;
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
            newPosition.y += _isJumpPressed ? (verticalSpeed * Time.deltaTime) : 0;
            transform.position = newPosition;
        }
        private void HandleSelectedCellOutline()
        {
            var gizmoEnabled = TryRaycastForObject(out var hit) ? Hit() : Miss();
            _selectedCellGizmo.SetEnabled(gizmoEnabled);

            bool Hit()
            {
                _cellGrid.FindCell(hit.point,out Cell cell);
                _selectedCellGizmo.UpdateBounds(new (Vector3.Scale((Vector3Int)cell + new Vector3(.5f,.5f,.5f),_cellSize),_cellSize));
                SetUIText(UITextType.SelectedName,hit.collider.transform.parent.gameObject.transform.parent.gameObject.name);
                SetUIObjectsActive(UIObjectType.NameDisplay,true);
                return true;
            }
            bool Miss()
            {
                SetUIText(UITextType.SelectedName,"_");
                SetUIObjectsActive(UIObjectType.NameDisplay,false);
                return false;
            }
        }

        public bool TryRaycastForObject(out RaycastHit hit)
        {
            return Physics.SphereCast(transform.position,_sphereCastRadius,transform.forward,out hit,_maxCastDistance);
        }

        #region Input Callbacks

        private void OnEscInput(InputAction.CallbackContext context)
        {
            if (_paused)
            {
                Unpause();
                return;
            }
            Pause();
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
        private void OnSprintInput(InputAction.CallbackContext context)
        {
            _isSprintPressed = context.ReadValueAsButton();
        }
        private void OnCrouchInput(InputAction.CallbackContext context)
        {
            _isCrouchPressed = context.ReadValueAsButton();
        }
        private void OnJumpInput(InputAction.CallbackContext context)
        {
            _isJumpPressed = context.ReadValueAsButton();
        }

        private void OnHInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                ToggleUIObjectActive(UIObjectType.HUD);
            }
        }      
        private void OnRInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                ToggleUIObjectActive(UIObjectType.Retical);
            }
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
            SetUIObjectsActive(UIObjectType.MenuHolder,false);
            SetUIObjectsActive(UIObjectType.Retical,true);
            SetCursorLock(true);
            _paused = false;
        }
        private void SetCursorLock(bool isLocked)
        {
            _canMove = isLocked;
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
        public void SetUIObjectsActive(UIObjectType type,bool isActive)
        {
            if (_uiObjectTypeToObjects.TryGetValue(type,out var goArray) && goArray != null)
            {
                foreach (var menuObject in goArray.Where(x => x != null && x.activeSelf != isActive))
                {
                    menuObject.SetActive(isActive);
                }
            }
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