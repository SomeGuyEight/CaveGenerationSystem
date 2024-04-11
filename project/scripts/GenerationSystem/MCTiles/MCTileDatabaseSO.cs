using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Tessera;

namespace SlimeGame
{
    [CreateAssetMenu(fileName = "new MC Tile Database",menuName = "Slime Game/MC Tile Database",order = 0)]
    public class MCTileDatabaseSO : SerializedScriptableObject
    {
        public void InitializeInstance(TileSO basicTile,Dictionary<SurfaceGeneratorType,TesseraGenerator[]> surfaceGenerators,Dictionary<CellTypes,TesseraTileBase[]> allTesseraTiles,Dictionary<CellTypes,GameObject[]> allMeshObjects,Dictionary<CellTypes,GameObject[]> allSampleObjects)
        {
            _basicTile = basicTile;
            surfaceGenerators.TryGetValue(SurfaceGeneratorType.Default,out var defaultGenerators);
            surfaceGenerators.TryGetValue(SurfaceGeneratorType.BigTile,out var bigTileGenerators);
            surfaceGenerators.TryGetValue(SurfaceGeneratorType.AdjacentModel,out var adjacentModelSurfaceGenerators);
            _defaultSurfaceGenerator = defaultGenerators?.FirstOrDefault();
            _surfaceGenerators = defaultGenerators;
            _bigTileSurfaceGenerators = bigTileGenerators;
            _adjacentModelSurfaceGenerators = adjacentModelSurfaceGenerators;
            foreach (var (cellTypes, tileBases) in allTesseraTiles)
            {
                switch (cellTypes)
                {
                    case CellTypes.Surface:
                        _defaultTileBases = tileBases;
                        break;
                    case CellTypes.FloorSurface:
                        _floorTileBases = tileBases;
                        break;
                    case CellTypes.WallSurface:
                        _wallTileBases = tileBases;
                        break;
                    case CellTypes.CeilingSurface:
                        _ceilingTileBases = tileBases;
                        break;
                }
            }
            _airOnlyTiles = new()
            {
                _defaultTileBases.FirstOrDefault(x => x != null && x.name.ToString().Substring(0,13) == MCTile._80_1111_1111.ToString())
            };
            foreach (var (cellTypes, meshObjects) in allMeshObjects)
            {
                switch (cellTypes)
                {
                    case CellTypes.Surface:
                        _defaultMeshObjects = meshObjects;
                        break;
                    case CellTypes.FloorSurface:
                        _floorMeshObjects = meshObjects;
                        break;
                    case CellTypes.WallSurface:
                        _wallMeshObjects = meshObjects;
                        break;
                    case CellTypes.CeilingSurface:
                        _ceilingMeshObjects = meshObjects;
                        break;
                }
            }
            foreach (var (cellTypes, sampleObjects) in allSampleObjects)
            {
                switch (cellTypes)
                {
                    case CellTypes.Surface:
                        _defaultSampleObjects = sampleObjects;
                        break;
                    case CellTypes.FloorSurface:
                        _floorSampleObjects = sampleObjects;
                        break;
                    case CellTypes.WallSurface:
                        _wallSampleObjects = sampleObjects;
                        break;
                    case CellTypes.CeilingSurface:
                        _ceilingSampleObjects = sampleObjects;
                        break;
                }
            }
            UpdateAllCachedData();
        }

        [FoldoutGroup("Default Tile",order:0)]
        [OdinSerialize,LabelWidth(100)]
        private TileSO _basicTile;

        #region { Tile Options & Tiles}

        [FoldoutGroup("Tile Options",order:0)]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        private TesseraGenerator _defaultSurfaceGenerator;

        [FoldoutGroup("Tile Options")]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        private TesseraGenerator[] _surfaceGenerators = new TesseraGenerator[0];

        [FoldoutGroup("Tile Options")]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        private TesseraGenerator[] _bigTileSurfaceGenerators = new TesseraGenerator[0];

        [FoldoutGroup("Tile Options")]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        private TesseraGenerator[] _adjacentModelSurfaceGenerators = new TesseraGenerator[0];

        [FoldoutGroup("Tile Bases")]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        private List<TesseraTileBase> _airOnlyTiles = new ();

        [FoldoutGroup("Tile Bases")]
        [OdinSerialize,OnValueChanged("UpdateTilePalette")]
        // ?? Change to HashSet ??
        // -> will be looking up by object not index like the single cell tiles
        private TesseraTileBase[] _bigTesseraTiles = new TesseraTileBase[0];

        [FoldoutGroup("Tile Bases")]
        [OdinSerialize,OnValueChanged("OnTileBaseChange")]
        private TesseraTileBase[] _defaultTileBases = new TesseraTileBase[0];

        [OdinSerialize,FoldoutGroup("Tile Bases")]
        private TesseraTileBase[] _floorTileBases = new TesseraTileBase[0];

        [OdinSerialize,FoldoutGroup("Tile Bases")]
        private TesseraTileBase[] _wallTileBases = new TesseraTileBase[0];

        [OdinSerialize,FoldoutGroup("Tile Bases")]
        private TesseraTileBase[] _ceilingTileBases = new TesseraTileBase[0];

        #endregion
        #region { Mesh Objects }

        [FoldoutGroup("Mesh Objects",order:0)]
        [OdinSerialize,OnValueChanged("OnDefaultMeshObjectsChange")]
        private GameObject[] _defaultMeshObjects = new GameObject[0];

        [FoldoutGroup("Mesh Objects")]
        [OdinSerialize,OnValueChanged("OnFloorMeshObjectsChange")]
        private GameObject[] _floorMeshObjects = new GameObject[0];

        [FoldoutGroup("Mesh Objects")]
        [OdinSerialize,OnValueChanged("OnWallMeshObjectsChange")]
        private GameObject[] _wallMeshObjects = new GameObject[0];

        [FoldoutGroup("Mesh Objects")]
        [OdinSerialize,OnValueChanged("OnCeilingMeshObjectsChange")]
        private GameObject[] _ceilingMeshObjects = new GameObject[0];

        #endregion
        #region { Sample Objects }

        [FoldoutGroup("Sample Objects",order:0)]
        [OdinSerialize,OnValueChanged("OnDefaultSampleObjectsChange")]
        private GameObject[] _defaultSampleObjects = new GameObject[0];

        [FoldoutGroup("Sample Objects")]
        [OdinSerialize,OnValueChanged("OnFloorSampleObjectsChange")]
        private GameObject[] _floorSampleObjects = new GameObject[0];

        [FoldoutGroup("Sample Objects")]
        [OdinSerialize,OnValueChanged("OnWallSampleObjectsChange")]
        private GameObject[] _wallSampleObjects = new GameObject[0];

        [FoldoutGroup("Sample Objects")]
        [OdinSerialize,OnValueChanged("OnCeilingSampleObjectsChange")]
        private GameObject[] _ceilingSampleObjects = new GameObject[0];

        #endregion
        #region { Read Only in Inspector Properties}

        [Header("Tiles"),FoldoutGroup("Read Only in Inspector Properties",order:0)]
        [OdinSerialize,ReadOnly,OnValueChanged("UpdateVoidAndAirTileFaceDetails")]
        private TesseraTileBase _00_VoidTile;

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize,ReadOnly,OnValueChanged("UpdateVoidAndAirTileFaceDetails")]
        private TesseraTileBase _80_AirTile;

        [Header("Palette"),FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize,ReadOnly]
        [InfoBox("Tile Palette is not consistant across all tiles",InfoMessageType = InfoMessageType.Warning,VisibleIf = "_isTilePaletteInvalid")]
        private TesseraPalette _palette;

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize,ReadOnly]
        [InfoBox("Void Corner Paint Not consistent in VoidTile",InfoMessageType = InfoMessageType.Warning,VisibleIf = "_isVoidPaintInvalid")]
        private int _voidPaint = -1;

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize,ReadOnly]
        [InfoBox("Air Corner Paint Not consistent in AirTile",InfoMessageType = InfoMessageType.Warning,VisibleIf = "_isAirPaintInvalid")]
        private int _airPaint = -1;

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize]
        private Dictionary<MCTile,TesseraTileBase> _mcTileToTileBase = new ();

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize]
        private Dictionary<TesseraTileBase,MCTile> _tileBaseToMCTile = new ();

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize]
        private Dictionary<(MCTile mCTile,CellTypes cellTypes),GameObject> _meshObjects = new ();

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize]
        private Dictionary<(MCTile mCTile,CellTypes cellTypes),GameObject> _sampleObjects = new ();

        [FoldoutGroup("Read Only in Inspector Properties")]
        [OdinSerialize]
        private Dictionary<TesseraTileBase,CellTypes> _tileBaseToCellTypes = new ();

        #endregion

#if UNITY_EDITOR
#pragma warning disable
        [Button("Update All Cached Data"), GUIColor("#37FFE5")]
        private void UpdateAllCachedData()
        {
            UpdateTileBases();
            UpdateAllMeshObjects();
            UpdateAllSampleObjects();
        }

        [Button("Update Tile Bases"), GUIColor("#37FFE5")]
        private void UpdateTileBases() => OnTileBaseChange();

        [Button("Update Mesh Objects"), GUIColor("#37FFE5")]
        private void UpdateAllMeshObjects()
        {
            OnDefaultMeshObjectsChange();
            OnFloorMeshObjectsChange();
            OnWallMeshObjectsChange();
            OnCeilingMeshObjectsChange();
        }

        [Button("Update Sample Objects"), GUIColor("#37FFE5")]
        private void UpdateAllSampleObjects()
        {
            OnDefaultSampleObjectsChange();
            OnFloorSampleObjectsChange();
            OnWallSampleObjectsChange();
            OnCeilingSampleObjectsChange();
            UpdateTilePalette();
        }
#endif
        [OdinSerialize,HideInInspector]
        private bool _areTilePalettesTheSame = true;
        private bool _isTilePaletteInvalid => !_areTilePalettesTheSame;
        private bool _isVoidPaintInvalid => _voidPaint == -1;
        private bool _isAirPaintInvalid => _airPaint == -1;
#pragma warning restore

        [OdinSerialize,HideInInspector]
        private FaceDetails _voidFaceDetails;

        [OdinSerialize,HideInInspector]
        private FaceDetails _airFaceDetails;

        public TileSO BasicTile { get { return _basicTile; } }
        public TesseraGenerator DefaultSurfaceGenerator { get { return _defaultSurfaceGenerator; } }
        public TesseraGenerator[] SurfaceGenerators { get { return _surfaceGenerators; } }
        public TesseraGenerator[] BigTileSurfaceGenerators { get { return _bigTileSurfaceGenerators; } }
        public TesseraGenerator[] AdjacentModelSurfaceGenerators { get { return _adjacentModelSurfaceGenerators; } }
        public TesseraPalette Palette { get { return _palette; } }
        public List<TesseraTileBase> AirOnlyTiles { get { return _airOnlyTiles; } }
        public TesseraTileBase VoidTile { get { return _00_VoidTile; } }
        public TesseraTileBase AirTile { get { return _80_AirTile; } }
        public int VoidPaint { get { return _voidPaint; } }
        public int AirPaint { get { return _airPaint; } }
        public FaceDetails VoidFaceDetails { get { return _voidFaceDetails; } }
        public FaceDetails AirFaceDetails { get { return _airFaceDetails; } }
        public FaceDetails DefaultFaceDetails => new() { faceType = FaceType.Square };

        public bool IsVoidTile(TesseraTileBase tile) => tile.name == _00_VoidTile.name;
        public bool IsAirTile(TesseraTileBase tile) => tile.name == _80_AirTile.name;
        public bool IsBigTesseraTile(TesseraTileBase tile) => _bigTesseraTiles.Contains(tile);

        public GameObject GetMeshObject(TesseraTileBase tile,CellTypes cellTypes,out MCTile mCTile)
        {
            if (MCTileHelper.IsDefaultMCTileObject(tile.gameObject,out mCTile) && _meshObjects.TryGetValue((mCTile, cellTypes),out var meshObject))
            {
                return meshObject;
            }
            throw new Exception("Tile Name and CellSubTypes key did not have a Mesh Object value");
        }

        public bool TryGetCellTypesFromSampleTile(TesseraTileBase tileBase,out CellTypes cellTypes)
        {
            if (MCTileHelper.IsDefaultMCTile(tileBase.name,out var _) && _tileBaseToCellTypes.TryGetValue(tileBase,out cellTypes))
            {
                return true;
            }
            cellTypes = 0;
            return false;
        }
        public GameObject GetTileSampleObject(TesseraTileBase tile,CellTypes cellTypes,out MCTile mCTile,out bool areCellTypesSynced,out CellTypes actualCellTypes)
        {
            if (MCTileHelper.IsDefaultMCTileObject(tile.gameObject,out mCTile) && _sampleObjects.TryGetValue((mCTile, cellTypes),out var sampleObject))
            {
                areCellTypesSynced = AreTileAndCellTypesSynced(mCTile,cellTypes,out actualCellTypes);
                return sampleObject;
            }
            throw new Exception("Tile Name and CellSubTypes key did not have a AdjacentModel Object value");
        }
        public GameObject GetSampleObject(MCTile mCTile,CellTypes cellTypes,out bool areCellTypesSynced,out CellTypes actualCellTypes)
        {
            if (mCTile.IsDefaultMCTile() && _sampleObjects.TryGetValue((mCTile, cellTypes),out var sampleObject))
            {
                areCellTypesSynced = AreTileAndCellTypesSynced(mCTile,cellTypes,out actualCellTypes);
                return sampleObject;
            }
            throw new Exception("Tile Name and CellSubTypes key did not have a AdjacentModel Object value");
        }

        public static bool AreTileAndCellTypesSynced(TesseraTileBase tileBase,CellTypes cellTypes,out CellTypes actualCellTypes)
        {
            /// TODO: better method
            /// -> returns "out of sync" if tile is not a default tile
            /// -> ex. BigTile etc
            if (!MCTileHelper.IsDefaultMCTileObject(tileBase.gameObject,out var mcTile))
            {
                actualCellTypes = cellTypes;
                return false;
            }
            return AreTileAndCellTypesSynced(mcTile,cellTypes,out actualCellTypes);
        }
        public static bool AreTileAndCellTypesSynced(MCTile mCTile,CellTypes cellTypes,out CellTypes actualCellTypes)
        {
            if (mCTile == MCTile._00_0000_0000 && !cellTypes.HasFlags(CellTypes.Void))
            {
                actualCellTypes = CellTypes.Void;
                return false;
            }
            if (mCTile == MCTile._80_1111_1111 && !cellTypes.HasFlags(CellTypes.Air))
            {
                actualCellTypes = CellTypes.Air;
                return false;
            }
            actualCellTypes = cellTypes;
            return true;
        }

#if UNITY_EDITOR
        private void OnFloorMeshObjectsChange() => OnObjectsChange(ref _meshObjects,_floorMeshObjects,CellTypes.FloorSurface);
        private void OnFloorSampleObjectsChange() => OnSampleObjectsChange(ref _sampleObjects,_floorSampleObjects,CellTypes.FloorSurface);
        private void OnWallMeshObjectsChange() => OnObjectsChange(ref _meshObjects,_wallMeshObjects,CellTypes.WallSurface);
        private void OnWallSampleObjectsChange() => OnSampleObjectsChange(ref _sampleObjects,_wallSampleObjects,CellTypes.WallSurface);
        private void OnCeilingMeshObjectsChange() => OnObjectsChange(ref _meshObjects,_ceilingMeshObjects,CellTypes.CeilingSurface);
        private void OnCeilingSampleObjectsChange() => OnSampleObjectsChange(ref _sampleObjects,_ceilingSampleObjects,CellTypes.CeilingSurface);
        private void OnTileBaseChange()
        {
            _mcTileToTileBase ??= new();
            _tileBaseToMCTile ??= new();
            foreach (var tileBase in _defaultTileBases.Where(x => x != null))
            {
                var mCTileName = tileBase.name.Substring(0,13);
                if (!MCTileHelper.IsDefaultMCTile(mCTileName,out var mCTile))
                {
                    continue;
                }
                _mcTileToTileBase.Remove(mCTile);
                var oldKVP = _tileBaseToMCTile.FirstOrDefault(kvp => kvp.Value == mCTile);
                if (oldKVP.Key != null)
                {
                    _tileBaseToMCTile.Remove(oldKVP.Key);
                }
                _mcTileToTileBase[mCTile] = tileBase;
                _tileBaseToMCTile[tileBase] = mCTile;
                switch (mCTile)
                {
                    case MCTile._00_0000_0000:
                        _00_VoidTile = tileBase;
                        break;
                    case MCTile._80_1111_1111:
                        _80_AirTile = tileBase;
                        break;
                }
            }
            UpdateVoidAndAirTileFaceDetails();
            UpdateTilePalette();
        }
        private void UpdateVoidAndAirTileFaceDetails()
        {
            if (_00_VoidTile != null)
            {
                _voidPaint = _00_VoidTile.sylvesFaceDetails.DoAllCornerPaintsMatch(out var paintIndex) ? paintIndex : -1;
                _voidFaceDetails = TesseraUtils.GetMatchingCornerPaint(_voidPaint);
            }
            else
            {
                _voidPaint = -1;
                _voidFaceDetails = null;
            }
            if (_80_AirTile != null)
            {
                _airPaint = _80_AirTile.sylvesFaceDetails.DoAllCornerPaintsMatch(out var paintIndex) ? paintIndex : -1;
                _airFaceDetails = TesseraUtils.GetMatchingCornerPaint(_airPaint);
            }
            else
            {
                _airPaint = -1;
                _airFaceDetails = null;
            }
        }
        private void OnDefaultMeshObjectsChange()
        {
            OnObjectsChange(ref _meshObjects,_defaultMeshObjects,CellTypes.Surface);
            UpdateVoidAndAirObjects(ref _meshObjects,_defaultMeshObjects);
        }
        private void OnDefaultSampleObjectsChange()
        {
            OnSampleObjectsChange(ref _sampleObjects,_defaultSampleObjects,CellTypes.Surface);
            UpdateVoidAndAirObjects(ref _sampleObjects,_defaultSampleObjects);
            HandleVoidAndAirSampleObjects();
        }
        private void HandleVoidAndAirSampleObjects()
        {
            HandleCellTypes(MCTile._00_0000_0000,CellTypes.Surface,CellTypes.Void);
            HandleCellTypes(MCTile._00_0000_0000,CellTypes.FloorSurface,CellTypes.Void);
            HandleCellTypes(MCTile._00_0000_0000,CellTypes.WallSurface,CellTypes.Void);
            HandleCellTypes(MCTile._00_0000_0000,CellTypes.CeilingSurface,CellTypes.Void);
            HandleCellTypes(MCTile._80_1111_1111,CellTypes.Surface,CellTypes.Air);
            HandleCellTypes(MCTile._80_1111_1111,CellTypes.FloorSurface,CellTypes.Air);
            HandleCellTypes(MCTile._80_1111_1111,CellTypes.WallSurface,CellTypes.Air);
            HandleCellTypes(MCTile._80_1111_1111,CellTypes.CeilingSurface,CellTypes.Air);

            void HandleCellTypes(MCTile mCTile,CellTypes toSearch,CellTypes toSetAs)
            {
                if (_sampleObjects.TryGetValue((mCTile, toSearch),out var sampleObject))
                {
                    RemoveAndAdd(sampleObject.GetComponent<TesseraPinned>().tile,toSetAs);
                }
            }
            void RemoveAndAdd(TesseraTileBase tileBase,CellTypes cellTypes)
            {
                _tileBaseToCellTypes.Remove(tileBase);
                _tileBaseToCellTypes.Add(tileBase,cellTypes);
            }
        }
        private void OnSampleObjectsChange(ref Dictionary<(MCTile, CellTypes),GameObject> dictionary,GameObject[] gameObjects,CellTypes cellTypes)
        {
            OnObjectsChange(ref dictionary,gameObjects,cellTypes);
            RemoveInvalidPins(ref dictionary);
            UpdateTileBaseToCellTypes();
        }
        private void RemoveInvalidPins(ref Dictionary<(MCTile, CellTypes),GameObject> dictionary)
        {
            foreach (var ((mCTile, cellTypes), go) in dictionary.ToArray())
            {
                var pinnedTile = go.GetComponent<TesseraPinned>();
                if (!mCTile.IsDefaultMCTile() || pinnedTile == null)
                {
                    dictionary.Remove((mCTile, cellTypes));
                }
                else if (pinnedTile.tile == null || pinnedTile.pinType != PinType.Pin)
                {
                    dictionary.Remove((mCTile, cellTypes));
                }
                else if (!MCTileHelper.IsDefaultMCTile(pinnedTile.gameObject.name))
                {
                    dictionary.Remove((mCTile, cellTypes));
                }
            }
        }
        private void UpdateTileBaseToCellTypes()
        {
            _tileBaseToCellTypes = new();
            foreach (var ((mCTiles, cellTypes), go) in _sampleObjects)
            {
                var pin = go.GetComponent<TesseraPinned>();
                if (pin == null || pin.tile == null)
                {
                    continue;
                }
                if (cellTypes == CellTypes.Void || cellTypes == CellTypes.Air)
                {
                    continue;
                }
                if (mCTiles == MCTile._00_0000_0000)
                {
                    _tileBaseToCellTypes.Add(pin.tile,CellTypes.Void);
                    continue;
                }
                if (mCTiles == MCTile._80_1111_1111)
                {
                    _tileBaseToCellTypes.Add(pin.tile,CellTypes.Air);
                    continue;
                }
                _tileBaseToCellTypes.Add(pin.tile,cellTypes);
            }
        }
        private void OnObjectsChange(ref Dictionary<(MCTile, CellTypes),GameObject> dictionary,GameObject[] objects,CellTypes cellTypes)
        {
            dictionary ??= new();
            foreach (var (key, _) in dictionary.ToArray().Where(kvp => kvp.Key.Item2 == cellTypes))
            {
                dictionary.Remove(key);
            }
            if (objects == null || objects.Length < 1)
            {
                return;
            }
            for (int i = 0;i < objects.Length;i++)
            {
                if (objects[i] == null)
                {
                    continue;
                }
                var mCTileName = objects[i].name;
                if (MCTileHelper.IsDefaultMCTile(mCTileName,out var mCTile))
                {
                    dictionary.Add((mCTile, cellTypes),objects[i]);
                }
            }
        }
        private void UpdateVoidAndAirObjects(ref Dictionary<(MCTile, CellTypes),GameObject> dictionary,GameObject[] objects)
        {
            dictionary ??= new();
            var voidKey = (MCTile._00_0000_0000, CellTypes.Void);
            dictionary.Remove(voidKey);
            var airKey = (MCTile._80_1111_1111, CellTypes.Air);
            dictionary.Remove(airKey);
            if (objects == null)
            {
                return;
            }

            var voidName = MCTile._00_0000_0000.ToString();
            var voidObject = objects.FirstOrDefault(x => x != null && x.name.Substring(0,13) == voidName);
            if (voidObject != null)
            {
                dictionary.Add(voidKey,voidObject);
            }
            var airName = MCTile._80_1111_1111.ToString();
            var airObject = objects.FirstOrDefault(x => x != null && x.name.Substring(0,13) == airName);
            if (airObject != null)
            {
                dictionary.Add(airKey,airObject);
            }
        }
#pragma warning disable
        private void UpdateTilePalette()
        {
            _palette = null;
            _areTilePalettesTheSame = true;
            if (_00_VoidTile != null && !IsPalatteSameAsMain(_00_VoidTile.palette))
            {
                _areTilePalettesTheSame = false;
            }
            if (_80_AirTile != null && !IsPalatteSameAsMain(_80_AirTile.palette))
            {
                _areTilePalettesTheSame = false;
            }
            if (_airOnlyTiles != null && !ArePalettesSameAsMain(_airOnlyTiles.ToArray()))
            {
                _areTilePalettesTheSame = false;
            }
            if (_defaultTileBases != null && !ArePalettesSameAsMain(_defaultTileBases))
            {
                _areTilePalettesTheSame = false;
            }
            if (_bigTesseraTiles != null && !ArePalettesSameAsMain(_bigTesseraTiles))
            {
                _areTilePalettesTheSame = false;
            }
            if (_defaultSurfaceGenerator != null && !AreTileEntriesPalettesSameAsMain(_defaultSurfaceGenerator.tiles))
            {
                _areTilePalettesTheSame = false;
            }
            HandleGeneratorArrays(_surfaceGenerators);
            HandleGeneratorArrays(_bigTileSurfaceGenerators);
            HandleGeneratorArrays(_adjacentModelSurfaceGenerators);

            bool IsPalatteSameAsMain(TesseraPalette palette)
            {
                if (_palette == null)
                {
                    _palette = palette;
                }
                return _palette == palette;
            }
            bool ArePalettesSameAsMain(TesseraTileBase[] tileArray)
            {
                if (tileArray == null)
                {
                    return true;
                }
                for (int i = 0;i < tileArray.Length;i++)
                {
                    if (tileArray[i] != null && !IsPalatteSameAsMain(tileArray[i].palette))
                    {
                        return false;
                    }
                }
                return true;
            }
            bool AreTileEntriesPalettesSameAsMain(List<TileEntry> tileEntries)
            {
                if (tileEntries == null)
                {
                    return true;
                }
                for (int i = 0;i < tileEntries.Count;i++)
                {
                    if (tileEntries[i] != null && tileEntries[i].tile != null && !IsPalatteSameAsMain(tileEntries[i].tile.palette))
                    {
                        return false;
                    }
                }
                return true;
            }
            void HandleGeneratorArrays(TesseraGenerator[] tesseraGenerators)
            {
                if (tesseraGenerators == null || tesseraGenerators.Length < 1)
                {
                    return;
                }
                for (int i = 0;i < tesseraGenerators.Length;i++)
                {
                    if (tesseraGenerators[i] != null && !AreTileEntriesPalettesSameAsMain(tesseraGenerators[i].tiles))
                    {
                        _areTilePalettesTheSame = false;
                    }
                }
            }

        }
#pragma warning restore


        public Dictionary<CellTypes,GameObject[]> GetAllMeshObjects()
        {
            return new()
            {
                { CellTypes.Surface         ,CopyArray(_defaultMeshObjects) },
                { CellTypes.FloorSurface    ,CopyArray(_floorMeshObjects  ) },
                { CellTypes.WallSurface     ,CopyArray(_wallMeshObjects   ) },
                { CellTypes.CeilingSurface  ,CopyArray(_ceilingMeshObjects) },
            };
        }
        public Dictionary<CellTypes,GameObject[]> GetAllTileObjects()
        {
            return new()
            {
                { CellTypes.Surface         ,CopyArray(_defaultTileBases) },
                { CellTypes.FloorSurface    ,CopyArray(_floorTileBases  ) },
                { CellTypes.WallSurface     ,CopyArray(_wallTileBases   ) },
                { CellTypes.CeilingSurface  ,CopyArray(_ceilingTileBases) },
            };
        }
        public GameObject[] GetBigTileObjects()
        {
            return CopyArray(_bigTesseraTiles);
        }
        private GameObject[] CopyArray(GameObject[] originalArray)
        {
            if (originalArray == null || originalArray.Length < 1)
            {
                return new GameObject[0];
            }
            var newArray = new GameObject[originalArray.Length];
            for (int i = 0;i < originalArray.Length;i++)
            {
                newArray[i] = originalArray[i];
            }
            return newArray;
        }
        private GameObject[] CopyArray(TesseraTileBase[] originalArray)
        {
            if (originalArray == null || originalArray.Length < 1)
            {
                return new GameObject[0];
            }
            var newArray = new GameObject[originalArray.Length];
            for (int i = 0;i < originalArray.Length;i++)
            {
                newArray[i] = originalArray[i] != null ? originalArray[i].gameObject : null;
            }
            return newArray;
        }
#endif



    }
}
