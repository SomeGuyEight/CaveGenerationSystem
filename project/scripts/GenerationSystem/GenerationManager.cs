using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Sylves;
using Tessera;

namespace SlimeGame
{
    [ShowOdinSerializedPropertiesInInspector]
    public class GenerationManager : SerializedMonoBehaviour
    {
        [OdinSerialize,FoldoutGroup("References")]
        private readonly MCTileDatabaseSO _database;

        [OdinSerialize,FoldoutGroup("References")]
        private readonly ControllerStateMachine _controller;

        [OdinSerialize,FoldoutGroup("References")]
        private readonly InstanceManager _instanceManager;

        [OdinSerialize,FoldoutGroup("References")]
        private readonly InstanceGenerator _instanceGenerator;

        [Title("General")]
        [OdinSerialize,FoldoutGroup("Options")]
        private readonly Vector3Int _cellSize = Vector3Int.one;

        [OdinSerialize,FoldoutGroup("Options")]
        private readonly Material _bigTileMaterial;

        [Title("Debugging")]
        [OdinSerialize,FoldoutGroup("Options")]
        private readonly GenerationDebugMode _debugMode;

        [OdinSerialize,FoldoutGroup("Options")]
        private readonly int _maxConstraintDebugToSave;

        [OdinSerialize,ReadOnly,FoldoutGroup("Options")]
        private Queue<GameObject> _constraintDebugInstances = new ();

        [OdinSerialize,ReadOnly,FoldoutGroup("Options")]
        private List<FullGenerationStats> _fullGenerationStats = new();

        [Title("Folder Paths")]
        [OdinSerialize,FoldoutGroup("Options")]
        [LabelWidth(150),FolderPath(RequireExistingPath = true)]
        private readonly string _newTilesSOFolderPath;

        [OdinSerialize,FoldoutGroup("Options")]
        [LabelWidth(150),FolderPath(RequireExistingPath = true)]
        private readonly string _sampleSOPath;

        [OdinSerialize,FoldoutGroup("Options")]
        [LabelWidth(150),FolderPath(AbsolutePath = true,RequireExistingPath = true)]
        private readonly string _samplePrefabPath;

        [OdinSerialize,FoldoutGroup("Options")]
        [LabelWidth(150),FolderPath(AbsolutePath = true,RequireExistingPath = true)]
        private readonly string _bigTileFolderPath;

        [OdinSerialize,FoldoutGroup("Options")]
        [LabelWidth(150),FolderPath(ParentFolder = "Assets",RequireExistingPath = true)]
        private readonly  string _fullGenStatsFolderPath;

        [Title("Current Surface Generator")]
        [OdinSerialize,HideLabel,FoldoutGroup("Surface Generators")]
        private TesseraGenerator _surfaceGenerator;

        [OdinSerialize,ReadOnly,FoldoutGroup("Surface Generators")]
        private SurfaceGeneratorType _currentGeneratorType = SurfaceGeneratorType.Default;

        [HorizontalGroup("Surface Generators/GeneratorButtons")]
        [Button("Default",ButtonSizes.Medium),GUIColor("#37FFE5")]
        public void SetSurfaceGeneratorToDefault() => ChangeSurfaceGeneratorType(SurfaceGeneratorType.Default);

        [HorizontalGroup("Surface Generators/GeneratorButtons")]
        [Button("Big Tile",ButtonSizes.Medium),GUIColor("#37FFE5")]
        public void SetSurfaceGeneratorToBigTile() => ChangeSurfaceGeneratorType(SurfaceGeneratorType.BigTile);

        [HorizontalGroup("Surface Generators/GeneratorButtons")]
        [Button("Adjancent Model",ButtonSizes.Medium),GUIColor("#37FFE5")]
        public void SetSurfaceGeneratorToAdjacentModel() => ChangeSurfaceGeneratorType(SurfaceGeneratorType.AdjacentModel);

#pragma warning disable
        [HorizontalGroup("Surface Generators/GeneratorButtons2")]
        [Button("Previous",ButtonSizes.Medium),GUIColor("#37FFE5")]
        public void SetToPreviousGenerator()
        {
            if (_generatorTypeToGenerators.TryGetValue(_currentGeneratorType,out var tuple) && tuple.generators != null)
            {
                tuple.index = tuple.index - 1 < 0 ? tuple.generators.Length - 1 : tuple.index - 1;
                _surfaceGenerator = tuple.generators[tuple.index];
                _generatorTypeToGenerators[_currentGeneratorType] = tuple;
                return;
            }
            _surfaceGenerator = null;
        }

        [HorizontalGroup("Surface Generators/GeneratorButtons2")]
        [Button("Next",ButtonSizes.Medium),GUIColor("#37FFE5")]
        public void SetToNextGenerator()
        {
            if (_generatorTypeToGenerators.TryGetValue(_currentGeneratorType,out var tuple) && tuple.generators != null)
            {
                tuple.index = tuple.index + 1 >= tuple.generators.Length ? 0 : tuple.index + 1;
                _surfaceGenerator = tuple.generators[tuple.index];
                _generatorTypeToGenerators[_currentGeneratorType] = tuple;
                return;
            }
            _surfaceGenerator = null;
        }
#pragma warning disable


        private Dictionary<SurfaceGeneratorType,(int index,TesseraGenerator[] generators)> _generatorTypeToGenerators = new ();
        private CubeGrid _cellGrid;
        private TesseraInitialConstraintBuilder _constraintBuilder;
        private GameObject _debugHolder;

        public MCTileDatabaseSO Database { get { return _database; } }
        public ControllerStateMachine Controller { get { return _controller; } }
        public InstanceManager InstanceManager { get { return _instanceManager; } }
        public InstanceGenerator InstanceGenerator { get { return _instanceGenerator; } }
        public TesseraGenerator SurfaceGenerator { get { return _surfaceGenerator; } }
        public TesseraInitialConstraintBuilder ConstraintBuilder { get { return _constraintBuilder; } }
        public SurfaceGeneratorType GeneratorType { get { return _currentGeneratorType; } }
        public GenerationDebugMode DebugMode { get { return _debugMode; } }
        public CubeGrid CellGrid { get { return _cellGrid; } }
        public Vector3Int CellSize { get { return _cellSize; } }
        public CellTypes[,,] DefaultNeighborCellTypes { get { return _instanceManager.DefaultNeighborCellTypes; } }

        private void Awake()
        {
            _cellGrid = new(_cellSize);
            _constraintBuilder = new(transform,_cellGrid);
            _generatorTypeToGenerators = new ();
            _debugHolder = new GameObject("Debug Holder");
            if (_database != null)
            {
                _generatorTypeToGenerators.Add(SurfaceGeneratorType.Default,(0, _database.SurfaceGenerators));
                _generatorTypeToGenerators.Add(SurfaceGeneratorType.BigTile,(0, _database.BigTileSurfaceGenerators));
                _generatorTypeToGenerators.Add(SurfaceGeneratorType.AdjacentModel,(0, _database.AdjacentModelSurfaceGenerators));
            }
            switch (_currentGeneratorType)
            {
                case SurfaceGeneratorType.Default: 
                    SetSurfaceGeneratorToDefault();
                    break;
                case SurfaceGeneratorType.BigTile:
                    SetSurfaceGeneratorToBigTile();
                    break;
                case SurfaceGeneratorType.AdjacentModel:
                    SetSurfaceGeneratorToAdjacentModel();
                    break;
            }
        }

        public void ChangeSurfaceGeneratorType(SurfaceGeneratorType type)
        {
            _currentGeneratorType = type;
            if (_generatorTypeToGenerators != null && _generatorTypeToGenerators.TryGetValue(type,out var tuple))
            {
                if (tuple.generators != null && tuple.index < tuple.generators.Length)
                {
                    _surfaceGenerator = tuple.generators[tuple.index];
                    return;
                }
            }
            _surfaceGenerator = null;
        }

        public void AddGenerationStats(TesseraStats stats,TesseraCompletion completion,CubeBound generationBound)
        {
            /// TODO: track cell types in generated area
            var date = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}";
            var time = $" ({DateTime.Now.Hour};{DateTime.Now.Minute};{DateTime.Now.Second})";
            _fullGenerationStats.Add(new FullGenerationStats()
            {
                Name = date + time,
                Min = generationBound.min,
                Size = generationBound.size,
                /// VoidCells = ,
                /// AirCells = ,
                /// SurfaceCells = ,
                Stats = stats,
                Completion = completion,
            });
        }
        public void EnqueueConstraintDebugObject(GameObject debugObject)
        {
            debugObject.transform.parent = _debugHolder.transform;
            _constraintDebugInstances ??= new();
            _constraintDebugInstances.Enqueue(debugObject);
            if (_constraintDebugInstances.Count >= _maxConstraintDebugToSave)
            {
                var pastDebugObject = _constraintDebugInstances.Dequeue();
                Destroy(pastDebugObject);
            }
        }

#if UNITY_EDITOR
#pragma warning disable
        [FoldoutGroup("Buttons")]
        [Button("Save As New Tile",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void SaveAsNewTileButton()
        {
            if (_instanceManager.TryGetFirstSelected(typeof(TileInstance),out var instance) && instance is TileInstance tileInstance)
            {
                SaveAsNewTile(tileInstance);
            }
        }

        [FoldoutGroup("Buttons")]
        [Button("Override Original Tile",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void OverrideOriginalTileSaveButton()
        {
            if (_instanceManager.TryGetFirstSelected(typeof(TileInstance),out var instance) && instance is TileInstance tileInstance)
            {
                OverrideOriginalTile(tileInstance);
            }          
        }

        [FoldoutGroup("Buttons")]
        [Button("Generate & Save New Sample",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void SaveNewSampleButton()
        {
            if (_instanceManager.TryGetFirstSelected(typeof(TileInstance),out var tileInstance))
            {
                SaveNewSample((TileInstance)tileInstance);
            }
        }

        [FoldoutGroup("Buttons")]
        [Button("Save As New Big Tile",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void SaveNewBigTileButton() => SaveNewBigTile();

        [FoldoutGroup("Buttons")]
        [Button("Save Full Gen Stats To Excel",ButtonSizes.Medium), GUIColor("#37FFE5")]
        public void SaveFullGenerationStatsToExcel()
        {
            StatsCSVWriter.SaveCompletionAndStatsCSV(_fullGenerationStats.ToArray(),_fullGenStatsFolderPath,$"Full Gen Stats",true);
            _fullGenerationStats.Clear();
            _fullGenerationStats = new List<FullGenerationStats>();
        }
#pragma warning restore

        public TileSO SaveAsNewTile(TileInstance tileInstance)
        {
            var size = tileInstance.CellBound.size;
            var tileOrigin = tileInstance.CellBound.min;
            var properties = new TileProperties
            {
                Types = tileInstance.Properties.Types,
                SubTypes = tileInstance.Properties.SubTypes,
                Sizes = tileInstance.Properties.Sizes,
                Shapes = tileInstance.Properties.Shapes,
            };

            var cellTypesArrays = new CellTypes[size.x,size.y,size.z];
            for (int x = 0;x < size.x;x++)
            {
                for (int y = 0;y < size.y;y++)
                {
                    for (int z = 0;z < size.z;z++)
                    {
                        var offsetCell = tileOrigin + new Cell(x,y,z);
                        if (!tileInstance.Cells.TryGetValue(offsetCell,out var cellInstance))
                        {
                            cellTypesArrays[x,y,z] = CellTypes.Void;
                            continue;
                        }
                        cellTypesArrays[x,y,z] = cellInstance.CellTypes;
                    }
                }
            }

            tileInstance.GetTypedSockets(out var sockets);
            var newTile = TileSO.GetNewInstance(properties,cellTypesArrays,sockets);
            string path = _newTilesSOFolderPath + $"/new {newTile.Name} {newTile.DateTimeStamp}.asset";
            AssetDatabase.CreateAsset(newTile,path);
            AssetDatabase.SaveAssets();

            tileInstance.UpdateOriginalTile(newTile);
            return newTile;
        }
        public void OverrideOriginalTile(TileInstance instance)
        {
            if (instance.OriginalTile == null)
            {
                Debug.Log("No original tile to override");
                return;
            }

            var size = instance.CellBound.size;
            var tileOrigin = instance.CellBound.min;
            var properties = new TileProperties
            {
                Types = instance.Properties.Types,
                SubTypes = instance.Properties.SubTypes,
                Sizes = instance.Properties.Sizes,
                Shapes = instance.Properties.Shapes,
            };

            var cellTypesArrays = new CellTypes[size.x,size.y,size.z];
            for (int x = 0;x < size.x;x++)
            {
                for (int y = 0;y < size.y;y++)
                {
                    for (int z = 0;z < size.z;z++)
                    {
                        var offsetCell = tileOrigin + new Cell(x,y,z);
                        if (!instance.Cells.TryGetValue(offsetCell,out var tileCell))
                        {
                            cellTypesArrays[x,y,z] = CellTypes.Void;
                            continue;
                        }
                        cellTypesArrays[x,y,z] = tileCell.CellTypes;
                    }
                }
            }

            var tileRef = instance.OriginalTile;
            instance.GetTypedSockets(out var sockets);
            tileRef.UpdateTile(properties,cellTypesArrays,sockets);
            ///// TODO: Figure out... 
            ///// ( !! ) the two lines below rename the actual script, not the scriptable object asset
            //////// var path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(tileRef));
            //////// AssetDatabase.RenameAsset(path,tileRef.Name);
            EditorUtility.SetDirty(tileRef);
            AssetDatabase.SaveAssets();
        }
        private void SaveNewSample(TileInstance tileInstance)
        {
            if (tileInstance == null)
            {
                return;
            }
            var cellTypesArrays = tileInstance.GetCellTypesArray();
            SampleSO.SaveNewSOAndSamplePrefab(_sampleSOPath,_samplePrefabPath,_database,_database.DefaultSurfaceGenerator,tileInstance.Properties,cellTypesArrays);
        }
        private void SaveNewBigTile()
        {
            if (!_instanceManager.TryGetAllSelected(typeof(CellInstance),out var instances) || instances == null)
            {
                return;
            }
            var cellInstances = new CellInstance[instances.Length];
            for (int i = 0; i < cellInstances.Length;i++)
            {
                cellInstances[i] = (CellInstance)instances[i];
            }
            BigTileGeneratorOptions bigTileOptions = new (_database,cellInstances,_cellSize,_bigTileMaterial,true,RotationGroupType.XZ);
            BigTileGenerator.SaveAsNewBigTile(bigTileOptions,_bigTileFolderPath);
        }
#endif
    }
}
