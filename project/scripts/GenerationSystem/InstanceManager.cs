using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sylves;
using Tessera;

namespace SlimeGame
{
    [ShowOdinSerializedPropertiesInInspector]
    public class InstanceManager : SerializedMonoBehaviour
    {
        private static Dictionary<GizmoTypes,Color> DefaultGizmoColors => new()
        {
            { GizmoTypes.None                                 , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Bound        | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Bound        | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Tile         | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Tile         | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.TileSet      | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.TileSet      | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Socket       | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Socket       | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.OriginSocket | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.OriginSocket | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SeedSocket   | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SeedSocket   | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.StepSocket   | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.StepSocket   | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SplitSocket  | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SplitSocket  | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Cell         | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.Cell         | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.VoidCell     | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.VoidCell     | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.AirCell      | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.AirCell      | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SurfaceCell  | GizmoTypes.Selected   , new Color(0.5f,0.5f,0.5f,.8f) },
            { GizmoTypes.SurfaceCell  | GizmoTypes.Unselected , new Color(0.5f,0.5f,0.5f,.8f) },
        };
        private static readonly Dictionary<TileTypes,Dictionary<SocketTypes,int>> TileTypesToSocketCounts = new()
        {
            {
                TileTypes.Seed, new ()
                {
                    { SocketTypes.Origin , 1    },
                    { SocketTypes.Seed   , 27   },
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
            { 
                TileTypes.HorizontalCenter, new ()
                {
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                } 
            },
            { 
                TileTypes.VerticalCenter, new ()
                {
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
            {
                TileTypes.TransitionFloor, new ()
                {
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
            {
                TileTypes.TransitionCeiling, new ()
                {
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
            {
                TileTypes.HorizontalCap, new ()
                {
                    { SocketTypes.Seed   , 27   },
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
            {
                TileTypes.VerticalCap, new ()
                {
                    { SocketTypes.Seed   , 27   },
                    { SocketTypes.Step   , 2    },
                    { SocketTypes.Split  , 27   },
                }
            },
        };

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150)]
        private readonly GenerationManager _generationManager;

        [FoldoutGroup("References")]
        [OdinSerialize,LabelWidth(150)]
        private TileSO _basicTile;
#pragma warning disable
        [FoldoutGroup("References"),Button,GUIColor("#29FF00")]
        [InfoBox("If the MCTile Database has a Basic Tile => _basicTile = value")]
        private void TryGetBasicTileFromDatabase() 
        {
            if (_generationManager?.Database?.BasicTile != null)
            {
                _basicTile = _generationManager.Database.BasicTile;
            }
        } 
#pragma warning restore

        [FoldoutGroup("References")]
        [OdinSerialize, LabelWidth(150)]
        private readonly TileSO _tileReadyToLoad;

        [FoldoutGroup("References")]
        //[OdinSerialize, LabelWidth(150)]
        [OdinSerialize,InfoBox("To use TileSO[] => Unpack prefab and remove readonly in script to use\n-> throws error if trying to use in prefab"),LabelWidth(150)]
        private readonly TileSO[] _tilesReadyToLoad;

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly bool _autoUpdateSockets = true;

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly int _minSocketUpdateDistance = 2;

        [FoldoutGroup("Options")]
        [OdinSerialize,LabelWidth(150)]
        private readonly TileProperties _tileProperties = new ();

        [FoldoutGroup("Options")]
        [OdinSerialize, LabelWidth(150)]
        private readonly Dictionary<GizmoTypes,Color> _gizmoColors = DefaultGizmoColors;

        [FoldoutGroup("Instance Collections")]
        [OdinSerialize,LabelWidth(150),ReadOnly]
        private HashSet<BaseGenerationInstance> _allInstances = new ();

        [FoldoutGroup("Instance Collections")]
        [OdinSerialize,LabelWidth(150),ReadOnly]
        private List<BaseGenerationInstance> _selectedInstances = new ();

        private Dictionary<GameObject,BaseGenerationInstance> _gameObjectToInstance = new ();
        private CellTypes[,,] _defaultNeighborCellTypes;
        private GizmoTypes _selectedGizmosEnabled = GizmoTypes.Bound | GizmoTypes.Tile | GizmoTypes.TileSet | GizmoTypes.AllSocketTypes | GizmoTypes.Surface | GizmoTypes.Cell;
        private GizmoTypes _unselectedGizmosEnabled = GizmoTypes.Bound | GizmoTypes.Tile | GizmoTypes.TileSet | GizmoTypes.AllSocketTypes;
        private ColliderTypes _selectedCollidersEnabled  = 0;
        private ColliderTypes _unselectedCollidersEnabled  = 0;

        public bool AutoUpdateSockets { get { return _autoUpdateSockets; } }
        public int MinSocketUpdateDistance { get { return _minSocketUpdateDistance; } }
        public CellTypes[,,] DefaultNeighborCellTypes { get { return _defaultNeighborCellTypes; } }
        public Vector3Int DefaultNeighborsArraySize => new Vector3Int(3,3,3);
        public CubeGrid CellGrid { get { return _generationManager.CellGrid; } }
        public BaseGenerationInstance SelectedInstance => (_selectedInstances != null && _selectedInstances.Count > 0) ? _selectedInstances[0] : null;
        public TesseraInitialConstraintBuilder Builder => _generationManager.ConstraintBuilder;

        #region { Buttons => Instance Loading & Clearing }
#pragma warning disable
        [FoldoutGroup("Buttons",order: 0)]
        [Title("Instance Actions"),FoldoutGroup("Buttons/Instance Actions",order: 0)]
        [Button("Initialize New Bound",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void InitializeNewBoundButton() => InitializeNewBound(Vector3Int.zero);

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Merge Selected Bound Instances",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void MergeSelectedBoundInstancesButton()
        {
            if (!TryGetAllSelected(typeof(BoundInstance),out var instances) || instances == null)
            {
                return;
            }
            BoundInstance firstBound = null;
            for (int i = 0; i < instances.Length;i++)
            {
                if (instances[i] != null && instances[i] is BoundInstance nextInstance)
                {
                    if (firstBound == null)
                    {
                        firstBound = nextInstance;
                        continue;
                    }
                    firstBound.AddBound(nextInstance.CellBound);
                    nextInstance.TryUnregisterAndDestroy();
                }
            }
        }

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Initialize New Tile",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void InitializeNewTileButton() => StartCoroutine(InitializeNewTile(Vector3Int.zero));

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Load Ready Tile",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void LoadReadyTileButton() => StartCoroutine(LoadTileAsInstance(_tileReadyToLoad));

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Load AllTypes Tiles In Array",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void LoadAllTilesInArrayButton() => LoadAllTilesInArray(_tilesReadyToLoad);

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Reset Tile",ButtonSizes.Medium), GUIColor("#FF5656")]
        public void ResetTileButton()
        {
            if (_generationManager.SurfaceGenerator == null)
            {
                Debug.Log("Surface Generator is null => skipping Tile Reset");
                return;
            }
            if (TryGetFirstSelected(typeof(TileInstance),out var instance) && instance is TileInstance tileInstance)
            {
                tileInstance.TryUnregisterAndDestroy();
                var properties = tileInstance.Properties.DeepClone();
                var centerCell = instance.CenterCell;
                var originalTile = tileInstance.OriginalTile;
                var parent = tileInstance.Parent;
                TileInstance newTileInstance = new (this,_generationManager.Database,properties,centerCell,originalTile,parent);
                StartCoroutine(InstanceGenerator.GenerateInstanceBounds(_generationManager,newTileInstance,newTileInstance.CellBound,_generationManager.SurfaceGenerator));
            }
        }

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Clear Selected Tile",ButtonSizes.Medium), GUIColor("#FF5656")]
        public void ClearSelectedTileButton() => ClearFirstSelectedInstance(typeof(TileInstance));

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Clear Selected Instances",ButtonSizes.Medium), GUIColor("#FF5656")]
        public void ClearSelectedInstancesButton() => ClearSelectedInstances();

        [FoldoutGroup("Buttons/Instance Actions")]
        [Button("Clear All Instances",ButtonSizes.Medium), GUIColor("#FF5656")]
        public void ClearAllInstancesButton() => ClearAllInstances();

#pragma warning restore
        #endregion
        #region { Buttons => Add Sockets }
#pragma warning disable
        [FoldoutGroup("Buttons/Sockets Actions")]
        [Title("Add Sockets"),VerticalGroup("Buttons/Sockets Actions/Add Sockets", order: 0)]
        [Button("Origin"), GUIColor("#29FF00")]
        public void AddOriginSocketButton() => TryAddSocket(SocketTypes.Origin,Directions.None);

        [VerticalGroup("Buttons/Sockets Actions/Add Sockets")]
        [Button("Seed"), GUIColor("#29FF00")]
        public void AddSeedSocketButton(Directions dirs) => TryAddSocket(SocketTypes.Seed,dirs);

        [VerticalGroup("Buttons/Sockets Actions/Add Sockets")]
        [Button("Split"), GUIColor("#29FF00")]
        public void AddSplitSocketButton(Directions dirs) => TryAddSocket(SocketTypes.Split,dirs);

        [HorizontalGroup("Buttons/Sockets Actions/Add Sockets/FwdBackSteps",order: 0)]
        [Button("Fwd Step"), GUIColor("#29FF00")]
        public void AddFwdStepSocketButton() => TryAddSocket(SocketTypes.Step,Directions.Fwd);

        [HorizontalGroup("Buttons/Sockets Actions/Add Sockets/FwdBackSteps")]
        [Button("Back Step"), GUIColor("#29FF00")]
        public void AddBackStepSocketButton() => TryAddSocket(SocketTypes.Step,Directions.Back);

        [HorizontalGroup("Buttons/Sockets Actions/Add Sockets/UpDownSteps",order: 0)]
        [Button("Up Step"), GUIColor("#29FF00")]
        public void AddUpStepSocketButton() => TryAddSocket(SocketTypes.Step,Directions.Up);

        [HorizontalGroup("Buttons/Sockets Actions/Add Sockets/UpDownSteps")]
        [Button("Down Step"), GUIColor("#29FF00")]
        public void AddDownStepSocketButton() => TryAddSocket(SocketTypes.Step,Directions.Down);

#pragma warning restore
        #endregion
        #region { Buttons => Gizmo Toggle }
#pragma warning disable
        [FoldoutGroup("Buttons/Gizmo Toggles")]
        [Title("Gizmo Toggles"),Button("Selected Bound"), HorizontalGroup("Buttons/Gizmo Toggles/Bound"), GUIColor("#29FF00")]
        public void ToggleSelectedBoundGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Bound,true);
        [Title(""), Button("Unselected Bound"), HorizontalGroup("Buttons/Gizmo Toggles/Bound"), GUIColor("#29FF00")]
        public void ToggleUnselectedBoundGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Bound,false);
        [Button("Selected Tile"), HorizontalGroup("Buttons/Gizmo Toggles/Tile"), GUIColor("#29FF00")]
        public void ToggleSelectedTileGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Tile,true);
        [Button("Unselected Tile"), HorizontalGroup("Buttons/Gizmo Toggles/Tile"), GUIColor("#29FF00")]
        public void ToggleUnselectedTileGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Tile,false);
        [Button("Selected Tile Set"), HorizontalGroup("Buttons/Gizmo Toggles/TileSet"), GUIColor("#29FF00")]
        public void ToggleSelectedTileSetGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.TileSet,true);
        [Button("Unselected Tile Set"), HorizontalGroup("Buttons/Gizmo Toggles/TileSet"), GUIColor("#29FF00")]
        public void ToggleUnselectedTileSetGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.TileSet,false);
        [Button("Selected Sockets"), HorizontalGroup("Buttons/Gizmo Toggles/Sockets"), GUIColor("#29FF00")]
        public void ToggleSelectedSocketGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Socket,true);
        [Button("Unselected Sockets"), HorizontalGroup("Buttons/Gizmo Toggles/Sockets"), GUIColor("#29FF00")]
        public void ToggleUnselectedSocketGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Socket,false);
        [Button("Selected Cell"), HorizontalGroup("Buttons/Gizmo Toggles/Cell"), GUIColor("#29FF00")]
        public void ToggleSelectedCellGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Cell,true);
        [Button("Unselected Cell"), HorizontalGroup("Buttons/Gizmo Toggles/Cell"), GUIColor("#29FF00")]
        public void ToggleUnselectedCellGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Cell,false);
        [Button("Selected Void"), HorizontalGroup("Buttons/Gizmo Toggles/Void"), GUIColor("#29FF00")]
        public void ToggleSelectedVoidGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Void,true);
        [Button("Unselected Void"), HorizontalGroup("Buttons/Gizmo Toggles/Void"), GUIColor("#29FF00")]
        public void ToggleUnselectedVoidGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Void,false);
        [Button("Selected Air"), HorizontalGroup("Buttons/Gizmo Toggles/Air"), GUIColor("#29FF00")]
        public void ToggleSelectedAirGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Air,true);
        [Button("Unselected Air"), HorizontalGroup("Buttons/Gizmo Toggles/Air"), GUIColor("#29FF00")]
        public void ToggleUnselectedAirGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Air,false);
        [Button("Selected Surface"), HorizontalGroup("Buttons/Gizmo Toggles/Surface"), GUIColor("#29FF00")]
        public void ToggleSelectedSurfaceGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Surface,true);
        [Button("Unselected Surface"), HorizontalGroup("Buttons/Gizmo Toggles/Surface"), GUIColor("#29FF00")]
        public void ToggleUnselectedSurfaceGizmosEnabledButton() => ToggleGizmosEnabled(GizmoTypes.Surface,false);
#pragma warning restore
        #endregion

        private void Start()
        {
            _defaultNeighborCellTypes = _basicTile.CellTypesArrays;
        }

        private void TryAddSocket(SocketTypes socketTypes,Directions directions)
        {
            if (!TryGetFirstSelected(typeof(TileInstance),out var instance) || instance == null)
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> no valid TileInstance selected");
                return;
            }

            var tileInstance = instance as TileInstance;

            if (!TileTypesToSocketCounts.TryGetValue(tileInstance.Properties.Types,out var socketTypesToCount) || socketTypesToCount == null)
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> no valid SocketSubTypes to Count found");
                return;
            }
            if (!socketTypesToCount.TryGetValue(socketTypes,out var count) || count < 1)
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> permited count already maxed {count}");
                return;
            }

            var typesToCount = tileInstance.GetCurrentSocketTypesCounts(out var stepDirs);
            if (socketTypes == SocketTypes.Step)
            {
                if (directions == Directions.None)
                {
                    Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Step Directions were None ( invalid )");
                    return;
                }
                if (stepDirs.one == directions || stepDirs.two == directions)
                {
                    Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Step Directions were already occupied");
                    return;
                }
                if (!TileHelper.TileTypesToStepDirections.TryGetValue(tileInstance.Properties.Types,out var stepDirsTuple))
                {
                    Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Step Directions could not be found from TileTypes");
                    return;
                }
                if (stepDirsTuple.positive != directions || stepDirsTuple.negative != directions)
                {
                    Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Directions were not found in HashSet");
                    return;
                }

                TryAddSocket();
                return;
            }

            if (!typesToCount.TryGetValue(socketTypes,out var currentCount))
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Count not found from SocketType");
                return;
            }
            if (currentCount >= count)
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Count is maxed for SocketType");
                return;
            }
            if (socketTypes == SocketTypes.Origin && directions != Directions.None)
            {
                Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Origin sockets should be 'Directions.None'");
                return;
            }

            TryAddSocket();
            return;

            void TryAddSocket()
            {
                if (!tileInstance.TryGetCellWithTypesFromDirections(tileInstance.CenterCell,CellTypes.Air,directions,out var validCell))
                {
                    Debug.Log($"Failed to add {directions} {socketTypes} Socket -> Could not find valid Air cell in ParentTile");
                    return;
                }

                tileInstance.InitializeNewSocket(validCell,socketTypes,directions);
                Debug.Log($"Successfully added {directions} {socketTypes} Socket");
                return;
            }
        }

        public Color GetGizmoColor(GizmoTypes enabledGizmos,bool isSelected)
        {
            enabledGizmos = isSelected ? enabledGizmos | GizmoTypes.Selected : enabledGizmos | GizmoTypes.Unselected;
            if (_gizmoColors.TryGetValue(enabledGizmos,out var color)) 
            {
                return color;
            }
            Debug.Log("Failed to get Gizmo Color => returning Color.black");
            return Color.black;
        }
        public bool IsGizmoEnabled(GizmoTypes enabledGizmos,bool isSelected)
        {
            if (enabledGizmos == GizmoTypes.None) 
            {
                /// here, b/c "None will give a false positive below"
                Debug.Log("Failed get Enabled Gizmos from object => returning false");
                return false;
            }
            return isSelected ? _selectedGizmosEnabled.HasFlags(enabledGizmos) : _unselectedGizmosEnabled.HasFlags(enabledGizmos);
        }
        public void ToggleGizmosEnabled(GizmoTypes gizmosToToggle,bool isSelected)
        {
            var newSelected = _selectedGizmosEnabled;
            var newUnselected = _unselectedGizmosEnabled;
            if (isSelected) 
            {
                newSelected = newSelected.HasFlags(gizmosToToggle) ? newSelected.UnsetFlags(gizmosToToggle) : newSelected.SetFlags(gizmosToToggle);
            } 
            else 
            {
                newUnselected = newUnselected.HasFlags(gizmosToToggle) ? newUnselected.UnsetFlags(gizmosToToggle) : newUnselected.SetFlags(gizmosToToggle);
            }

            if (newSelected != _selectedGizmosEnabled)
            {
                _selectedGizmosEnabled = newSelected;
            }
            else if (newUnselected != _unselectedGizmosEnabled)
            {
                _unselectedGizmosEnabled = newUnselected;
            }

            foreach (var instance in _allInstances?.Where(x => x.GizmoTypes.HasFlags(gizmosToToggle)))
            {
                instance.UpdateGizmoEnabled();
            }

        }

        public bool IsColliderEnabled(ColliderTypes enabledColliders,bool isSelected)
        {
            if (enabledColliders == ColliderTypes.None)
            {
                /// here to catch when ColliderTypes are not set up yet
                /// -> 'ColliderTypes.None' will give a false positive below
                Debug.Log("Failed get Enabled Colliders from object => returning false");
                return false;
            }
            if (isSelected)
            {
                return _selectedCollidersEnabled.HasFlags(enabledColliders);
            }
            return _unselectedCollidersEnabled.HasFlags(enabledColliders);
        }
        public void UpdateCollidersEnabled()
        {
            _generationManager.Controller.GetEnabledCollidersValue(out var selected,out var unselected);
            if (selected == _selectedCollidersEnabled && unselected == _unselectedCollidersEnabled)
            {
                return;
            }
            var collidersToUpdate = _selectedCollidersEnabled.UnsetFlags(selected);
            collidersToUpdate |= selected.UnsetFlags(_selectedCollidersEnabled);
            collidersToUpdate |= _unselectedCollidersEnabled.UnsetFlags(unselected);
            collidersToUpdate |= unselected.UnsetFlags(_unselectedCollidersEnabled);
            if (collidersToUpdate.HasFlags(ColliderTypes.Cell))
            {
                collidersToUpdate |= ColliderTypes.AllCellValues;
            }
            if (collidersToUpdate.HasFlags(ColliderTypes.Socket))
            {
                collidersToUpdate |= ColliderTypes.AllSocketValues;
            }
            _selectedCollidersEnabled = selected;
            _unselectedCollidersEnabled = unselected;

            if (_allInstances != null && _allInstances.Count > 0)
            {
                foreach (var instance in _allInstances.Where(x => collidersToUpdate.HasFlags(x.ColliderTypes)))
                {
                    instance.UpdateCollidersEnabled();
                }
            }
        }

        public void InitializeNewBound(Vector3 position)
        {
            var spawnCell = _generationManager.CellGrid.FindCell(position);
            if (spawnCell != null)
            {
                InitializeNewBound((Vector3Int)spawnCell);
            }
        }
        public void InitializeNewBound(Vector3Int spawnCell)
        {
            var newCustomBound = new BoundInstance(this,_generationManager.Database,spawnCell,Vector3Int.one);
            TryRegisterGameObjectToInstance(newCustomBound.MainObject,newCustomBound);
            if (_selectedInstances == null || _selectedInstances.Count < 1)
            {
                TrySelectSingleInstance(newCustomBound);
            }
        }
        public void InitializeNewTile(Vector3 position)
        {
            var centerCell = _generationManager.CellGrid.FindCell(position);
            if (centerCell != null) 
            {
                StartCoroutine(InitializeNewTile((Vector3Int)centerCell));
            }
        }
        public IEnumerator InitializeNewTile(Vector3Int centerCell)
        {
            var tileInstance = new TileInstance(this,_generationManager.Database,_tileProperties.DeepClone(),centerCell);
            var coroutine = InstanceGenerator.GenerateInstanceBounds(_generationManager,tileInstance,tileInstance.CellBound);
            yield return StartCoroutine(coroutine);
            TryRegisterGameObjectToInstance(tileInstance.MainObject,tileInstance);
            if (_selectedInstances == null || _selectedInstances.Count < 1)
            {
                TrySelectSingleInstance(tileInstance);
            }
        }

        public IEnumerator LoadAllTilesInArray(TileSO[] baseTiles)
        {
            if (baseTiles == null || baseTiles.Length < 1) {
                yield break;
            }
            for (int i = 0;i < baseTiles.Length;i++) 
            {
                var instance = _gameObjectToInstance.Values
                    .Where(type => type is TileInstance)
                    .FirstOrDefault(tile => ((TileInstance)tile).OriginalTile == baseTiles[i]);
                if (instance is TileInstance tileInstance && tileInstance.OriginalTile == baseTiles[i]) 
                {
                    continue;
                }
                yield return StartCoroutine(LoadTileAsInstance(baseTiles[i]));
                if (_selectedInstances == null || _selectedInstances.Count < 1)
                {
                    TrySelectSingleInstance(instance);
                }
            } 
        }
        public IEnumerator LoadTileAsInstance(TileSO baseTile,Vector3Int? tileCenterCell = null)
        {
            if (baseTile == null) 
            {
                yield break;
            }
            StartCoroutine(LoadTileAsInstance(baseTile,tileCenterCell ?? Vector3Int.zero));
        }
        public IEnumerator LoadTileAsInstance(TileSO baseTile,Vector3Int tileCenterCell)
        {
            var properties = _tileProperties.DeepClone();
            var tileInstance = new TileInstance(this,_generationManager.Database,properties,tileCenterCell,baseTile);
            var coroutine = InstanceGenerator.GenerateInstanceBounds(_generationManager,tileInstance,tileInstance.CellBound);
            yield return StartCoroutine(coroutine);
            TryRegisterGameObjectToInstance(tileInstance.MainObject,tileInstance);
            TrySelectSingleInstance(tileInstance);
            if (_selectedInstances == null || _selectedInstances.Count < 1)
            {
                TrySelectSingleInstance(tileInstance);
            }
        }

        public void RegenerateInstanceBounds(TileInstance tileInstance,CubeBound generationBound)
        {
            StartCoroutine(InstanceGenerator.RegenerateInstanceBounds(_generationManager,tileInstance,generationBound));
        }

        public bool TryRegisterGameObjectToInstance(GameObject objectKey,BaseGenerationInstance instanceValue)
        {
            _gameObjectToInstance ??= new();
            if (objectKey == null || instanceValue == null) 
            {
                return false;
            }
            return _gameObjectToInstance.TryAdd(objectKey,instanceValue);
        }
        public bool TryRegisterInstance(BaseGenerationInstance instance)
        {
            _allInstances ??= new ();
            if (instance == null) 
            {
                return false;
            }
            if (!_allInstances.Contains(instance))
            {
                _allInstances.Add(instance);
                return true;
            }
            return false;
        }
        public bool TryUnregisterGameObjectToInstance(GameObject objectKey)
        {
            if (_gameObjectToInstance == null || objectKey == null)
            {
                return false;
            }
            return _gameObjectToInstance.Remove(objectKey);
        }
        public bool TryUnregisterInstance(BaseGenerationInstance instance)
        {
            if (_allInstances == null || instance == null) 
            {
                return false;
            }
            return _allInstances.Remove(instance);
        }

        public bool TryGetInstanceFromGameObject(GameObject gameObject,out BaseGenerationInstance instance)
        {
            if (gameObject == null || _gameObjectToInstance == null)
            {
                instance = null;
                return false;
            }
            if (_gameObjectToInstance.TryGetValue(gameObject,out instance))
            {
                return true;
            }
            return false;
        }
        public BaseGenerationInstance GetLastSelectedInstance()
        {
            return (_selectedInstances == null || _selectedInstances.Count < 1) ? null : _selectedInstances[^1];
        }
        public bool TryGetFirstSelected(Type type,out BaseGenerationInstance instance) => TryGetFirstSelected(type,out instance,out _);
        public bool TryGetFirstSelected(Type[] types,out BaseGenerationInstance instance)
        {
            instance = null;
            if (types == null || types.Length < 1)
            {
                return false;
            }

            int? lowestIndex = null;
            for (int i = 0; i < types.Length;i++)
            {
                if (TryGetFirstSelected(types[i],out var selectedInstance,out int index))
                {
                    if (lowestIndex == null || lowestIndex > index)
                    {
                        lowestIndex = index;
                        instance = selectedInstance;
                    }
                }
            }
            return instance != null;
        }
        public bool TryGetFirstSelected(Type type,out BaseGenerationInstance instance,out int index)
        {
            if (_selectedInstances != null || _selectedInstances.Count > 0)
            {
                for (int i = 0;i < _selectedInstances.Count;i++)
                {
                    if (_selectedInstances[i].GetType() == type)
                    {
                        instance = _selectedInstances[i];
                        index = i;
                        return instance != null;
                    }
                }
            }
            instance = null;
            index = -1;
            return false;
        }
        public bool TryGetAllSelected(Type type,out BaseGenerationInstance[] instances)
        {
            instances = null;
            if (_selectedInstances == null || _selectedInstances.Count < 1)
            {
                return false;
            }
            var instancesHashSet = new HashSet<BaseGenerationInstance>();
            foreach (var instance in _selectedInstances.Where(i => i.GetType() == type))
            {
                instancesHashSet.Add(instance);
            }
            if (instancesHashSet.Count < 1)
            {
                instances = null;
                return false;
            }
            instances = instancesHashSet.ToArray();
            return true;
        }

        public void TrySelectSingleInstance(BaseGenerationInstance instance)
        {
            if (instance == null) 
            {
                return;
            }

            UnselectAllInstances();
            SelectInstance(instance);
            _selectedInstances = new() { instance };
        }
        public void TrySelectMultipleInstance(BaseGenerationInstance instance)
        {
            if (instance == null) 
            {
                return;
            }

            _selectedInstances ??= new ();
            if (_selectedInstances.Contains(instance))
            {
                UnselectInstance(instance);
                _selectedInstances.Remove(instance);
            }
            else
            {
                _selectedInstances.Add(instance);
                SelectInstance(instance);
            }
            UpdateCollidersEnabled();
        }
        public void SelectChildInstance(BaseGenerationInstance instance)
        {
            if (instance == null || instance.Parent == null || _selectedInstances == null|| !_selectedInstances.Contains(instance.Parent))
            {
                return;
            }

            if (_selectedInstances.Contains(instance))
            {
                _selectedInstances.Remove(instance);
                UnselectInstance(instance);
            }
            else
            {
                SelectInstance(instance);
                _selectedInstances.Add(instance);
            }
            UpdateCollidersEnabled();
        }
        private void SelectInstance(BaseGenerationInstance instance)
        {
            instance.IsSelected = true;
            if (instance.Gizmo != null) 
            {
                instance.UpdateGizmoColor();
                instance.UpdateGizmoEnabled();
            }
            if (instance.Colliders != null) 
            {
                instance.UpdateCollidersEnabled();
            }
        }
        private void UnselectInstance(BaseGenerationInstance instance)
        {
            instance.IsSelected = false;
            if (instance.Gizmo != null)
            {
                instance.UpdateGizmoColor();
                instance.UpdateGizmoEnabled();
            }
            if (instance.Colliders != null) 
            {
                instance.UpdateCollidersEnabled();
            }
        }
        private void UnselectAllInstances()
        {
            if (_selectedInstances == null)
            {
                return;
            }
            for (int i = 0;i < _selectedInstances.Count;i++)
            {
                UnselectInstance(_selectedInstances[i]);
            }
        }

        public void ClearSelectedInstances()
        {
            for (int i = _selectedInstances.Count - 1;i > -1;i--)
            {
                _selectedInstances.RemoveAt(i);
                ClearInstance(_selectedInstances[i]);
            }
        }
        public void ClearFirstSelectedInstance(Type instanceType)
        {
            TryGetFirstSelected(instanceType, out var instance, out var index);
            _selectedInstances.RemoveAt(index);
            ClearInstance(instance);
        }
        public void ClearAllInstances()
        {
            var allInstances = _allInstances.ToArray();
            for (int i = allInstances.Length - 1;i > -1;i--)
            {
                ClearInstance(allInstances[i]);
            }
            _allInstances = null;
            _selectedInstances = null;
            _gameObjectToInstance = null;
        }
        public void ClearInstance(BaseGenerationInstance instance)
        {
            if (instance == null || instance is BaseSocketInstance || instance is CellInstance)
            {
                return;
            }
            instance.TryUnregisterAndDestroy();
        }

    }
}
