#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class MCTileDatabaseGenerator : SerializedMonoBehaviour
    {
        private enum MainFolderType
        {
            Databases,
            TilesSOs,
            Samples,
            Generators,
            Palettes,
            BigTiles,
            TileBases,
            MeshObjects,
            SampleObjects,
        }
        private enum SubFolderType
        {
            DefaultDatabases,

            BasicTileSOs,
            NewTileSOs,

            SampleSOs,
            SamplePrefabs,

            DefaultGenerators,
            BigTileGenerators,
            AdjacentPaintGenerators,

            DefaultPalettes,

            DefaultBigTiles,
            NewBigTiles,

            DefaultTileBases,
            FloorTileBases,
            WallTileBases,
            CeilingTileBases,

            DefaultMeshObjects,
            FloorMeshObjects,
            WallMeshObjects,
            CeilingMeshObjects,

            DefaultSampleObjects,
            FloorSampleObjects,
            WallSampleObjects,
            CeilingSampleObjects,
        }

        private readonly static MainFolderType[] _mainPathTypeFolders = new[]
        {
            MainFolderType.Databases     ,
            MainFolderType.TilesSOs      ,
            MainFolderType.Samples       ,
            MainFolderType.Generators    ,
            MainFolderType.Palettes      ,
            MainFolderType.BigTiles      ,
            MainFolderType.TileBases     ,
            MainFolderType.MeshObjects   ,
            MainFolderType.SampleObjects ,
        };
        private readonly static Dictionary<MainFolderType,(SubFolderType, SubFolderType)> _subFolderFirstAndLastValues = new()
        {
            { MainFolderType.Databases     ,(SubFolderType.DefaultDatabases     ,SubFolderType.DefaultDatabases        ) },
            { MainFolderType.TilesSOs       ,(SubFolderType.BasicTileSOs           ,SubFolderType.NewTileSOs                ) },
            { MainFolderType.Samples       ,(SubFolderType.SampleSOs            ,SubFolderType.SamplePrefabs           ) },
            { MainFolderType.Palettes      ,(SubFolderType.DefaultPalettes      ,SubFolderType.DefaultPalettes         ) },
            { MainFolderType.Generators    ,(SubFolderType.DefaultGenerators    ,SubFolderType.AdjacentPaintGenerators ) },
            { MainFolderType.BigTiles      ,(SubFolderType.DefaultBigTiles      ,SubFolderType.NewBigTiles             ) },
            { MainFolderType.TileBases     ,(SubFolderType.DefaultTileBases     ,SubFolderType.CeilingTileBases        ) },
            { MainFolderType.MeshObjects   ,(SubFolderType.DefaultMeshObjects   ,SubFolderType.CeilingMeshObjects      ) },
            { MainFolderType.SampleObjects ,(SubFolderType.DefaultSampleObjects ,SubFolderType.CeilingSampleObjects    ) },
        };
        private readonly static Dictionary<MainFolderType,Dictionary<SubFolderType,CellTypes>> _folderTypesToCellTypes = new ()
        {
            {
                MainFolderType.TileBases, new ()
                {
                    { SubFolderType.DefaultTileBases      , CellTypes.Surface         },
                    { SubFolderType.FloorTileBases        , CellTypes.FloorSurface    },
                    { SubFolderType.WallTileBases         , CellTypes.WallSurface     },
                    { SubFolderType.CeilingTileBases      , CellTypes.CeilingSurface  },
                }
            },
            {
                MainFolderType.MeshObjects, new ()
                {
                    { SubFolderType.DefaultMeshObjects    , CellTypes.Surface         },
                    { SubFolderType.FloorMeshObjects      , CellTypes.FloorSurface    },
                    { SubFolderType.WallMeshObjects       , CellTypes.WallSurface     },
                    { SubFolderType.CeilingMeshObjects    , CellTypes.CeilingSurface  },
                }
            },
            {
                MainFolderType.SampleObjects, new ()
                {
                    { SubFolderType.DefaultSampleObjects  , CellTypes.Surface         },
                    { SubFolderType.FloorSampleObjects    , CellTypes.FloorSurface    },
                    { SubFolderType.WallSampleObjects     , CellTypes.WallSurface     },
                    { SubFolderType.CeilingSampleObjects  , CellTypes.CeilingSurface  },
                }
            },
        };
        private readonly static Dictionary<SubFolderType,SubFolderType> _tileTypeToMeshType = new ()
        {
            { SubFolderType.DefaultTileBases , SubFolderType.DefaultMeshObjects },
            { SubFolderType.FloorTileBases   , SubFolderType.FloorMeshObjects   },
            { SubFolderType.WallTileBases    , SubFolderType.WallMeshObjects    },
            { SubFolderType.CeilingTileBases , SubFolderType.CeilingMeshObjects },
        };
        private readonly static Dictionary<SubFolderType,SubFolderType> _sampleTypeToTileType = new ()
        {
            { SubFolderType.DefaultSampleObjects   , SubFolderType.DefaultTileBases   },
            { SubFolderType.FloorSampleObjects     , SubFolderType.FloorTileBases     },
            { SubFolderType.WallSampleObjects      , SubFolderType.WallTileBases      },
            { SubFolderType.CeilingSampleObjects   , SubFolderType.CeilingTileBases   },
        };
        private readonly static SubFolderType[] _tileFoldersForAdjacentModelGeneratorType = new SubFolderType []
        {
            SubFolderType.FloorTileBases,
            SubFolderType.WallTileBases,
            SubFolderType.CeilingTileBases,
        };

        private static readonly int _defaultVoidPaint = 1;
        private static readonly int _defaultAirPaint = 2;
        private static readonly int _defaultVoidTileWeight = 1;
        private static readonly int _defaultAirTileWeight = 10;
        private static readonly int _defaultSurfaceTileWeight = 1;

        private static Dictionary<(int color1, int color2),bool> DefaultMatchOverrides => new()
        {
            { (0,0), true  },{ (1,0), false },{ (2,0), false },{ (3,0), true  },{ (4,0), true  },
            { (0,1), false },{ (1,1), true  },{ (2,1), false },{ (3,1), false },{ (4,1), false },
            { (0,2), false },{ (1,2), false },{ (2,2), true  },{ (3,2), false },{ (4,2), false },
            { (0,3), true  },{ (1,3), false },{ (2,3), false },{ (3,3), false },{ (4,3), true  },
            { (0,4), true  },{ (1,4), false },{ (2,4), false },{ (3,4), true  },{ (4,4), false },
        };
        private static List<PaletteEntry> DefaultPaletteEntries => new()
        {
            new () { name = "Empty"          , color = new (0, 0, 0, 0)                      },
            new () { name = "Void"           , color = new (136/255f , 3/255f   , 252/255f ) },
            new () { name = "Air"            , color = new (0/255f   , 253/255f , 180/255f ) },
            new () { name = "SelfExclusion1" , color = new (204/255f , 172/255f , 224/255f ) },
            new () { name = "SelfExclusion2" , color = new (187/255f , 241/255f , 220/255f ) },
        };
        private static Dictionary<CellTypes,Material> DefaultCellTypesToMaterial => new()
        {
            { CellTypes.Surface         , null },
            { CellTypes.FloorSurface    , null },
            { CellTypes.WallSurface     , null },
            { CellTypes.CeilingSurface  , null },
        };


        [Header("Database Options")]
        [OdinSerialize]
        private readonly string _newDatabaseName;

        [OdinSerialize,FolderPath(RequireExistingPath = true)]
        private readonly string _folderPath;

        [OdinSerialize]
        private readonly GameObject[] _defaultMeshObjects;

        [OdinSerialize,VerticalGroup("Materials")]
        private Dictionary<CellTypes,Material> _cellTypesToMaterial = DefaultCellTypesToMaterial;
#pragma warning disable
        [Button("Set To Default"),VerticalGroup("Materials")]
        private void SetMaterialsToDefault() => _cellTypesToMaterial = DefaultCellTypesToMaterial;
#pragma warning restore

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private TesseraPalette _palette;

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private int _voidPaint = _defaultVoidPaint;

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private int _airPaint = _defaultAirPaint;

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private int _voidTileWeight = _defaultVoidTileWeight;

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private int _airTileWeight = _defaultAirTileWeight;

        [OdinSerialize,FoldoutGroup("Tile Options")]
        private int _surfaceTileWeight = _defaultSurfaceTileWeight;

#pragma warning disable
        [Button("Set To Default"),FoldoutGroup("Tile Options")]
        private void SetTileOptionsToDefault()
        {
            _voidPaint = _defaultVoidPaint;
            _airPaint = _defaultAirPaint;
            _voidTileWeight = _defaultVoidTileWeight;
            _airTileWeight = _defaultAirTileWeight;
            _surfaceTileWeight = _defaultSurfaceTileWeight;
        }
#pragma warning restore

        #region { Tessera Generator }

        /// <summary>
        /// The area of generation.
        /// Setting this will cause the size to be rounded to a multiple of <see cref="_cellSize"/>
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private Bounds _bounds;

        /// <summary>
        /// The stride between each cell in the generation.
        /// "big" tiles may occupy a multiple of this cell size.
        /// </summary>
        [Tooltip("The stride between each cell in the generation.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private Vector3 _cellSize = Vector3.one;

        /// TODO: implement method to use void or air as skybox
        ///
        /// -> for now just use void b/c that is the only one that works with the system
        /// /// <summary>
        /// /// If set, this tile is used to define extra initial constraints for the boundary.
        /// /// </summary>
        /// [Tooltip("If set, this tile is used to define extra initial constraints for the boundary.")]
        /// [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        /// private TesseraTileBase _skyBox = null;
        /// TODO: ?? Figure out more flexible method ??
        /// -> for now just use default => 10 for void/air & 1 for the rest
        /// /// <summary>
        /// /// The list of tiles eligible for generation.
        /// /// </summary>
        /// [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        /// [Tooltip("The list of tiles eligible for generation.")]
        /// private List<TileEntry> _tiles = new ();

        /// <summary>
        /// Controls the algorithm used internally for Wave Function Collapse.
        /// </summary>
        [Tooltip("Controls the algorithm used internally for Wave Function Collapse.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private TesseraWfcAlgorithm _algorithm = TesseraWfcAlgorithm.Ac4;

        /// <summary>
        /// Fixes the seed for random number generator.
        /// If the value is zero, the seed is taken from Unity.Random 
        /// </summary>
        [Tooltip("Fixes the seed for random number generator.\n If the value is zero, the seed is taken from Unity.Random.")]
       [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private int _seed = 0;

        /// <summary>
        /// Records undo/redo when run by pressing the Generate button in the Inspector.
        /// </summary>
        [Tooltip("Records undo/redo when run in the editor.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private bool _recordUndo = false;

        /// <summary>
        /// If set, backtracking will be used during generation.
        /// Backtracking can find solutions that would otherwise be failures,
        /// but can take a long time.
        /// </summary>
        [Tooltip("If set, backtracking will be used during generation.\nBacktracking can find solutions that would otherwise be failures, but can take a long time.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private bool _backtrack = true;

        /// <summary>
        /// How many steps to take before retrying from the start.
        /// </summary>
        [Tooltip("How many steps to take before retrying from the start.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private int _stepLimit = 88;

        /// <summary>
        /// If backtracking is off, how many times to retry generation if a solution
        /// cannot be found.
        /// </summary>
        [Tooltip("How many times to retry generation if a solution cannot be found.")]
        [OdinSerialize,FoldoutGroup("Tessera Generator Options")]
        private int _retries = 5;

        /// <summary>
        /// Sets which sort of model the generator uses.
        /// The model dictates how nearby tiles relate to each other.
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Model Type")]
        private ModelType _modelType = ModelType.AdjacentPaint;

        /// <summary>
        /// The size of the overlap parameter for the overlapping model.
        /// <see cref="ModelType.Overlapping"/>
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Model Type"),ShowIf("DisplayOverlapSize")]
        private Vector3Int _overlapSize = new (3, 3, 3);
        private bool DisplayOverlapSize() => _modelType == ModelType.Overlapping;

        /// <summary>
        /// For overlapping models, a list of objects to use as input samples.
        /// Each one will have its children inspected and read out.
        /// <see cref="ModelType.Overlapping"/>
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Model Type"),ShowIf("DisplaySamples")]
        private List<GameObject> _samples = new List<GameObject>();
        private bool DisplaySamples() => _modelType == ModelType.Overlapping || _modelType == ModelType.Adjacent;

        /// <summary>
        /// Controls what is output when the generation fails.
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Failure Mode")]
        private FailureMode _failureMode = FailureMode.Cancel;
        private bool ShowUncertantyOptions() => _failureMode != FailureMode.Cancel;

        /// <summary>
        /// Game object to show in cells that have yet to be fully solved.
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Failure Mode"),ShowIf("ShowUncertantyOptions")]
        private GameObject _uncertaintyTile;

        /// <summary>
        /// Game object to show in cells that cannot be solved.
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Failure Mode"),ShowIf("ShowUncertantyOptions")]
        private GameObject _contradictionTile;

        /// <summary>
        /// If true, the uncertainty tiles shrink as the solver gets more certain.
        /// </summary>
        [OdinSerialize,FoldoutGroup("Tessera Generator Options/Failure Mode"),ShowIf("ShowUncertantyOptions")]
        private bool _scaleUncertainyTile = true;

        #endregion

        [OdinSerialize,FoldoutGroup("Cached Values")]
        private Dictionary<MainFolderType,string> _mainFolderPaths = new();

        [OdinSerialize,FoldoutGroup("Cached Values")]
        private Dictionary<SubFolderType,string> _subFolderPaths = new();

        [OdinSerialize,FoldoutGroup("Cached Values")]
        private Dictionary<SubFolderType,Dictionary<MCTile,GameObject>> _databaseObjects = new();

        [OdinSerialize,FoldoutGroup("Cached Values")]
        private TileSO _basicTile;

        [OdinSerialize,FoldoutGroup("Cached Values")]
        private Dictionary<SurfaceGeneratorType,TesseraGenerator[]> _surfaceGenerators = new();

        private Vector3 Scale => VectorUtils.Divide(_cellSize,Vector3.one);
        private TesseraPalette Palette => _palette != null ? _palette : _palette = CreateNewPallete();
        private int VoidPaint => _voidPaint;
        private int AirPaint => _airPaint;
        private Dictionary<MCTile,GameObject> DefaultMCTileToTileBase => _databaseObjects[SubFolderType.DefaultTileBases];

        private TesseraTile GetDefaultTile(MCTile mCTile)
        {
            var (_, voidTileObject) = _databaseObjects[SubFolderType.DefaultTileBases]
                .FirstOrDefault(x => x.Key == mCTile);
            return voidTileObject.GetComponent<TesseraTile>();
        }

#pragma warning disable
        [Title("Generate Full New Database"),Button]
        private void GenerateNewFullDatabase(bool clearCachedData = true,bool createNewPalette = true)
        {
            if (clearCachedData)
            {
                ClearCachedValues();
            }
            CreateFolders();
            CreateBasicTile();
            CreateMeshObjects();
            if (createNewPalette)
            {
                _palette = CreateNewPallete();
            }
            CreateTesseraTiles();
            CreateSampleObjects();
            CreateSurfaceGenerators();
            CreateMCTileDatabase();
        }
#pragma warning restore

        [Title("Individual Actions","Some actions will need others to proceed => for example folders are needed for every step"), Button]
        private void CreateFolders()
        {
            _mainFolderPaths ??= new();
            _subFolderPaths ??= new();
            var rootFolderPath = _newDatabaseName + " " + SGUtils.DateTimeStamp();
            AssetDatabase.CreateFolder(_folderPath,rootFolderPath);
            foreach (var mainFolderType in _mainPathTypeFolders)
            {
                string databaseGuid = AssetDatabase.CreateFolder(_folderPath + "/" + rootFolderPath, mainFolderType.ToString());
                _mainFolderPaths.Add(mainFolderType,AssetDatabase.GUIDToAssetPath(databaseGuid));
            }
            foreach (var (mainFolderType, (firstType, lastType)) in _subFolderFirstAndLastValues)
            {
                string mainFolderPath = _mainFolderPaths[mainFolderType];
                var max = (int)lastType + 1;
                for (int i = (int)firstType;i < max;i++)
                {
                    var subFolderType = (SubFolderType)i;
                    string databaseGuid = AssetDatabase.CreateFolder(mainFolderPath,subFolderType.ToString());
                    _subFolderPaths.Add(subFolderType,AssetDatabase.GUIDToAssetPath(databaseGuid));
                }
            }
        }

        [Button]
        private void CreateBasicTile()
        {
            if (!_subFolderPaths.TryGetValue(SubFolderType.BasicTileSOs,out var path))
            {
                Debug.Log("No valid path to save BasicTile found -> skipping");
                return;
            }
            _basicTile = TileSO.GenericTile;
            AssetDatabase.CreateAsset(_basicTile,path + $"/new {_basicTile.Name} {_basicTile.DateTimeStamp}.asset");
            AssetDatabase.SaveAssets();
        }

        [Button]
        private void CreateMeshObjects()
        {
            _databaseObjects ??= new();
            var scale = Scale;
            var subFolderTypeToCellTypes = _folderTypesToCellTypes[MainFolderType.MeshObjects];
            foreach (var (subFolderType, cellTypes) in subFolderTypeToCellTypes)
            {
                var subFolderPath = _subFolderPaths[subFolderType];
                Dictionary<MCTile,GameObject> mCTileToMeshObjects = new (_defaultMeshObjects.Length);
                foreach (var meshObject in _defaultMeshObjects)
                {
                    if (MCTileHelper.IsDefaultMCTileObject(meshObject,out var mCTile))
                    {
                        var tempObject = Instantiate(meshObject,Vector3.zero,Quaternion.identity);
                        tempObject.transform.localScale = scale;
                        tempObject.name = meshObject.name;

                        if (_cellTypesToMaterial.TryGetValue(cellTypes,out var material))
                        {
                            var meshRenderers = tempObject.GetComponents<MeshRenderer>();
                            for (int i = 0;i < meshRenderers.Length;i++)
                            {
                                meshRenderers[i].material = material;
                            }
                             meshRenderers = tempObject.GetComponentsInChildren<MeshRenderer>();
                            for (int i = 0;i < meshRenderers.Length;i++)
                            {
                                meshRenderers[i].material = material;
                            }                      
                        }
                        else
                        {
                            Debug.Log("No valid material was found for Mesh Objects");
                        }

                        var newPrefab = PrefabUtility.SaveAsPrefabAsset(tempObject,subFolderPath + $"/{(MCTileName)mCTile} ( {cellTypes} Mesh Object ) ({_newDatabaseName}).prefab");
                        DestroyImmediate(tempObject);
                        mCTileToMeshObjects.Add(mCTile,newPrefab);
                    }
                }
                _databaseObjects[subFolderType] = mCTileToMeshObjects;
            }
        }

#pragma warning disable
        [Button("Create New Pallete")]
        private void CreateNewPalleteButton() => CreateNewPallete();
#pragma warning restore
        private TesseraPalette CreateNewPallete()
        {
            var newPalette = TesseraUtils.GetNewTilePalette(DefaultPaletteEntries);
            newPalette.matchOverrides = DefaultMatchOverrides;
            newPalette.OnBeforeSerialize();
            AssetDatabase.CreateAsset(newPalette,_subFolderPaths[SubFolderType.DefaultPalettes] + $"/new {_newDatabaseName}.asset");
            AssetDatabase.SaveAssets();
            return newPalette;
        }

        [Button]
        private void CreateTesseraTiles()
        {
            var subFolderTypeToCellTypes = _folderTypesToCellTypes[MainFolderType.TileBases];
            foreach (var (subFolderType, cellTypes) in subFolderTypeToCellTypes)
            {
                var subFolderPath = _subFolderPaths[subFolderType];
                var mCTileToMeshObjects = _databaseObjects[_tileTypeToMeshType[subFolderType]];
                Dictionary<MCTile,GameObject> mCTileToTileBaseObjects = new (mCTileToMeshObjects.Count);
                foreach (var (mCTile, meshObject) in mCTileToMeshObjects)
                {
                    var tempTileObject = new GameObject();
                    var tesseraTile = tempTileObject.AddComponent<TesseraTile>();
                    /// ( ! ) Need to remove default face details b/c <see cref="TesseraTile"/> constructor adds six at (0,0,0)
                    tesseraTile.RemoveOffset(Vector3Int.zero);

                    var orientedFaces = ((Corners)mCTile).ToOrientedFaces(AirPaint,VoidPaint);
                    tesseraTile.OverrideOffsetsAndOrientedFaces(new() { Vector3Int.zero },orientedFaces);
                    mCTile.TryGetOrientationValues(out var rotGroup,out var isReflectable,out var isSymetric);
                    tesseraTile.cellSize = _cellSize;
                    tesseraTile.center = Vector3.zero;
                    tesseraTile.rotatable = rotGroup != RotationGroupType.None;
                    tesseraTile.rotationGroupType = rotGroup;
                    tesseraTile.reflectable = isReflectable;
                    tesseraTile.symmetric = isSymetric;
                    tesseraTile.palette = Palette;

                    var newMeshObj = Instantiate(meshObject,tempTileObject.transform);
                    newMeshObj.name = meshObject.name;

                    var newPrefab = PrefabUtility.SaveAsPrefabAsset(tempTileObject,subFolderPath + $"/{(MCTileName)mCTile} ( {cellTypes} Tile Base ) ({_newDatabaseName}).prefab");
                    DestroyImmediate(tempTileObject);
                    mCTileToTileBaseObjects.Add(mCTile,newPrefab);
                }
                _databaseObjects[subFolderType] = mCTileToTileBaseObjects;
            }
        }

        [Button]
        private void CreateSampleObjects()
        {
            var subFolderTypeToCellTypes = _folderTypesToCellTypes[MainFolderType.SampleObjects];
            foreach (var (subFolderType, cellTypes) in subFolderTypeToCellTypes)
            {
                /// relative ( MCTile , TileBase )
                var tileBases = _databaseObjects[_sampleTypeToTileType[subFolderType]];
                var subFolderPath = _subFolderPaths[subFolderType];
                Dictionary<MCTile,GameObject> mCTileToSampleObjects = new (tileBases.Count);
                foreach (var (mCTile, tileBase) in tileBases)
                {
                    var tempSampleObj = new GameObject();
                    var pin = tempSampleObj.AddComponent<TesseraPinned>();
                    pin.pinType = PinType.Pin;
                    pin.tile = tileBase.GetComponent<TesseraTile>();
                    var newPrefab = PrefabUtility.SaveAsPrefabAsset(tempSampleObj,subFolderPath + $"/{(MCTileName)mCTile} ( {cellTypes} Tile Base ) ({_newDatabaseName}).prefab");
                    DestroyImmediate(tempSampleObj);
                    mCTileToSampleObjects.Add(mCTile,newPrefab);
                }
                /// add all obj to _databaseMCTileObjects
                _databaseObjects[subFolderType] = mCTileToSampleObjects;
            }
        }

        [Button]
        private void CreateSurfaceGenerators()
        {
            var voidTile = GetDefaultTile(MCTile._00_0000_0000);
            var defaultEntries = GetDefaultTileEntrys();
            CreateSurfaceGenerators(SurfaceGeneratorType.Default,SubFolderType.DefaultGenerators,defaultEntries,voidTile);
            CreateSurfaceGenerators(SurfaceGeneratorType.BigTile,SubFolderType.BigTileGenerators,defaultEntries,voidTile);
            CreateSurfaceGenerators(SurfaceGeneratorType.AdjacentModel,SubFolderType.AdjacentPaintGenerators,defaultEntries,voidTile,"( Default Tiles-CellTypes Only )");
            CreateSurfaceGenerators(SurfaceGeneratorType.AdjacentModel,SubFolderType.AdjacentPaintGenerators,GetTileEntrys(_tileFoldersForAdjacentModelGeneratorType,true),voidTile,"( Floor, Wall, and Ceiling Tiles-CellTypes )");
        }
        private List<TileEntry> GetDefaultTileEntrys()
        {
            var defaultTileBaseObj = DefaultMCTileToTileBase;
            List<TileEntry> entries = new (defaultTileBaseObj.Count);
            foreach (var (mCTile, tileBaseObj) in defaultTileBaseObj)
            {
                entries.Add(
                    new()
                    {
                        tile = tileBaseObj.GetComponent<TesseraTileBase>(),
                        weight = GetWeight(mCTile)
                    });
            }
            return entries;
        }
        private List<TileEntry> GetTileEntrys(SubFolderType[] subFolderTypes,bool includeDefaultAirAndVoid)
        {
            List<TileEntry> entries = new ();
            foreach (var folderType in subFolderTypes)
            {
                foreach (var (mCTile, tileBaseObj) in _databaseObjects[folderType])
                {
                    Add(mCTile,tileBaseObj);
                }
            }
            if (includeDefaultAirAndVoid)
            {
                Add(MCTile._00_0000_0000,GetDefaultTile(MCTile._00_0000_0000).gameObject);
                Add(MCTile._80_1111_1111,GetDefaultTile(MCTile._80_1111_1111).gameObject);
            }
            return entries;

            void Add(MCTile mCTile,GameObject tileBaseObj)
            {
                entries.Add(
                    new()
                    {
                        tile = tileBaseObj.GetComponent<TesseraTileBase>(),
                        weight = GetWeight(mCTile),
                    });
            }
        }
        private int GetWeight(MCTile mCTile)
        {
            return mCTile.IsVoidMCTile() ? _voidTileWeight : mCTile.IsAirMCTile() ? _airTileWeight : _surfaceTileWeight;
        }
        private void CreateSurfaceGenerators(SurfaceGeneratorType generatorType,SubFolderType subFolderType,List<TileEntry> tileEntries,TesseraTile skyBox,string suffix = "")
        {
            var subFolderPath = _subFolderPaths[subFolderType];
            var tempGeneratorObj = new GameObject();

            var generator = tempGeneratorObj.AddComponent<TesseraGenerator>();
            generator.tiles                 = tileEntries;
            generator.skyBox                = skyBox;
            generator.bounds                = _bounds;
            generator.cellSize              = _cellSize;
            generator.algorithm             = _algorithm;
            generator.seed                  = _seed;
            generator.recordUndo            = _recordUndo;
            generator.backtrack             = _backtrack;
            generator.stepLimit             = _stepLimit;
            generator.retries               = _retries;
            generator.modelType             = _modelType;
            generator.overlapSize           = _overlapSize;
            generator.samples               = _samples;
            generator.failureMode           = _failureMode;
            generator.uncertaintyTile       = _uncertaintyTile;
            generator.contradictionTile     = _contradictionTile;
            generator.scaleUncertainyTile   = _scaleUncertainyTile;

            var newPrefab = PrefabUtility.SaveAsPrefabAsset(tempGeneratorObj,subFolderPath + $"/{generatorType} {suffix} ({_newDatabaseName}).prefab");
            DestroyImmediate(tempGeneratorObj);
            _surfaceGenerators ??= new();
            if (!_surfaceGenerators.TryGetValue(generatorType, out var generators)) 
            {
                _surfaceGenerators[generatorType] = new[] { newPrefab.GetComponent<TesseraGenerator>() };
                return;
            }
            var newArray = new TesseraGenerator[generators.Length + 1];
            for (int i = 0;i < generators.Length;i++)
            {
                newArray[i] = generators[i];
            }
            newArray[^1] = newPrefab.GetComponent<TesseraGenerator>();
            _surfaceGenerators[generatorType] = newArray;
        }

        [Button]
        private void CreateMCTileDatabase()
        {
            var newDatabase = ScriptableObject.CreateInstance<MCTileDatabaseSO>();
            var tileBases = GetCellTypesToTileBases(MainFolderType.TileBases);
            var meshObjects = GetCellTypesToObjects(MainFolderType.MeshObjects);
            var sampleObjects = GetCellTypesToObjects(MainFolderType.SampleObjects);
            newDatabase.InitializeInstance(_basicTile,_surfaceGenerators,tileBases,meshObjects,sampleObjects);
            AssetDatabase.CreateAsset(newDatabase,_subFolderPaths[SubFolderType.DefaultDatabases] + $"/new {_newDatabaseName}.asset");
            AssetDatabase.SaveAssets();
        }
        private Dictionary<CellTypes,GameObject[]> GetCellTypesToObjects(MainFolderType mainFolderType)
        {
            Dictionary<CellTypes,GameObject[]> cellTypesToObjects = new (_folderTypesToCellTypes[mainFolderType].Count);
            foreach (var (subFolderType, cellTypes) in _folderTypesToCellTypes[mainFolderType])
            {
                List<GameObject> objects = new (_databaseObjects[subFolderType].Count);
                foreach (var (_, gameObject) in _databaseObjects[subFolderType])
                {
                    objects.Add(gameObject);
                }
                cellTypesToObjects[cellTypes] = objects.ToArray();
            }
            return cellTypesToObjects;
        }
        private Dictionary<CellTypes,TesseraTileBase[]> GetCellTypesToTileBases(MainFolderType mainFolderType)
        {
            Dictionary<CellTypes,TesseraTileBase[]> cellTypesToObjects = new (_folderTypesToCellTypes[mainFolderType].Count);
            foreach (var (subFolderType, cellTypes) in _folderTypesToCellTypes[mainFolderType])
            {
                List<TesseraTileBase> tesseraTiles = new (_databaseObjects[subFolderType].Count);
                foreach (var (_, gameObject) in _databaseObjects[subFolderType])
                {
                    var tileBase = gameObject.GetComponent<TesseraTileBase>();
                    if (tileBase != null)
                    {
                        tesseraTiles.Add(tileBase);
                    }
                }
                cellTypesToObjects[cellTypes] = tesseraTiles.ToArray();
            }
            return cellTypesToObjects;
        }

        [Button]
        private void ClearCachedValues()
        {
            _basicTile = null;
            _mainFolderPaths = null;
            _subFolderPaths = null;
            _databaseObjects = null;
            _surfaceGenerators = null;
        }
    }
}
#endif