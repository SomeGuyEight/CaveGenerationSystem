using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sylves;
using Tessera;

namespace SlimeGame 
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    public class TileInstance : BaseGenerationInstance, ICellInstanceCollection
    {
        public TileInstance(InstanceManager manager,MCTileDatabaseSO database,TileProperties properties,Vector3Int spawnCell,TileSO originalTile = null,BaseGenerationInstance parent = null)
        {
            _originalTile = originalTile;
            _properties = properties;
            _cells = new ();
            _sockets = new ();
            Vector3Int size;
            if (originalTile != null) {
                size = originalTile.ArraySize;
            }
            else
            {
                size = manager.DefaultNeighborsArraySize;
            }
            BaseInitializeProperties(manager,database,spawnCell,size,parent);
            CreateCellHolder();
            CreateCellInstances();
            CreateSocketHolder();
            CreateSockets();
        }

        private TileSO _originalTile;
        private TileProperties _properties;
        private Dictionary<Cell,CellInstance> _cells; 
        private List<BaseSocketInstance> _sockets;
        private GameObject _cellHolder;
        private GameObject _socketHolder;

        public override ColliderTypes ColliderTypes { get { return ColliderTypes.Tile; } }
        public override GizmoTypes GizmoTypes { get { return GizmoTypes.Tile; } }

        public TileSO OriginalTile { get { return _originalTile; } }
        public TileProperties Properties { get { return _properties; } }
        public Dictionary<Cell,CellInstance> Cells { get { return _cells; } }
        public List<BaseSocketInstance> Sockets { get { return _sockets; } }
        public GameObject CellHolder { get { return _cellHolder; } }
        public GameObject SocketHolder { get { return _socketHolder; } }

        private ICellInstanceCollection AsCellCollection => this;
        private string PropertiesString => $"( {_properties.Types}{_properties.SubTypes}; {_properties.Sizes} Sizes; {_properties.Shapes} Shapes )";
        public TesseraInitialConstraintBuilder Builder => Manager.Builder;
        public CubeBound ConstraintBound => new (CellBound.min - Vector3Int.one,CellBound.max + Vector3Int.one);
        public bool TryGetCellInstance(Vector3Int cell,out CellInstance cellInstance,out Vector3Int localCell)
        {
            localCell = AsCellCollection.GetLocalCell(cell);
            return _cells.TryGetValue((Cell)cell,out cellInstance);
        }

        protected override void CreateCellBound(Vector3Int spawnCell,Vector3Int size) => DefaultCreateCellBound(spawnCell,size);
        protected override void CreateMainObject()
        {
            MainObject = new GameObject($"{DisplayName} {DateStamp} {TimeStamp}");
            MainObject.transform.position = Vector3.Scale(CellBound.min,CellSize);
        }
        protected override void CreateDisplayName()
        {
            if (OriginalTile != null) 
            {
                DisplayName = $"(Cloned) {OriginalTile.name} {"{"} Size {CellBound.size} ( Min {CellBound.min} :: Max {CellBound.max} ) {"}"}";
            }
            else 
            {
                DisplayName = $"(New) {PropertiesString} {"{"} Size {CellBound.size} ( Min {CellBound.min} :: Max {CellBound.max} ) {"}"}";
            }
        }
        protected override void CreateGizmo() => DefaultCreateGizmo();
        protected override void CreateColliders() => DefaultCreateColliders();
        protected override void RegisterWithInstanceManager() => DefaultRegisterInstanceWithManager();
        private void CreateCellHolder()
        {
            _cellHolder = new GameObject("Cell Holder");
            _cellHolder.transform.position = MainObject.transform.position;
            _cellHolder.transform.SetParent(MainObject.transform);
        }
        private void CreateCellInstances()
        {
            CellTypes[,,] cellTypesArrays;
            if (_originalTile != null)
            {
                cellTypesArrays = _originalTile.CellTypesArrays;
            }
            else
            {
                cellTypesArrays = Manager.DefaultNeighborCellTypes;
            }
            var min = CellBound.min;
            var size = new Vector3Int(cellTypesArrays.GetLength(0),cellTypesArrays.GetLength(1),cellTypesArrays.GetLength(2));

            for (int x = 0;x < size.x;x++)
            {
                for (int y = 0;y < size.y;y++)
                {
                    for (int z = 0;z < size.z;z++)
                    {
                        if (cellTypesArrays[x,y,z].HasFlags(CellTypes.Void))
                        {
                            continue;
                        }
                        var cell = new Cell(min.x + x,min.y + y,min.z + z);
                        InitializeCellInstance(cell,cellTypesArrays[x,y,z]);
                    }
                }
            }
        }
        private void CreateSocketHolder()
        {
            _socketHolder = new GameObject("Socket Holder");
            _socketHolder.transform.position = MainObject.transform.position;
            _socketHolder.transform.SetParent(MainObject.transform);
        }
        private void CreateSockets()
        {
            if (OriginalTile != null)
            {
                InitializeTileSockets();
                return;
            }
            InitializeDefaultSockets();
        }
        private void InitializeTileSockets()
        {
            for (int i = 0;i < OriginalTile.Sockets.Length;i++)
            {
                var socket = OriginalTile.Sockets[i];
                var cell = socket.Cell + CellBound.min;
                InitializeNewSocket(cell,socket.SocketTypes,socket.Directions);
            }
        }
        private void InitializeDefaultSockets()
        {
            var defaultSockets = TileHelper.GetDefaultSocketValues(_properties.Types);
            if (defaultSockets == null)
            {
                return;
            }
            for (int i = 0;i < defaultSockets.Length;i++)
            {
                var (socketTypes, directions) = defaultSockets[i];
                if (!TryGetCellWithTypesFromDirections(CenterCell,CellTypes.Air,directions,out var validCell))
                {
                    Debug.Log($"Failed to find valid Air cell in TileInstance for {directions} {socketTypes} Socket");
                }
                InitializeNewSocket(validCell,socketTypes,directions);
            }
        }

        public override void UpdateDisplayName() 
        {
            if(OriginalTile != null) 
            {
                DisplayName = $"(Cloned) {OriginalTile.name} {"{"} Size {CellBound.size} ( Min {CellBound.min} :: Max {CellBound.max} ) {"}"}";
            }
            else 
            {
                DisplayName = $"(New) {PropertiesString} {"{"} Size {CellBound.size} ( Min {CellBound.min} :: Max {CellBound.max} ) {"}"}";
            }

            if(MainObject != null) 
            {
                MainObject.name = $"{DisplayName} {DateStamp} {TimeStamp}";
            }
        }
        public override void UpdateGizmoEnabled() => DefaultUpdateGizmoEnabled();
        public override void UpdateGizmoDimensions() => DefaultUpdateGizmoDimensions();
        public override void UpdateGizmoColor() => DefaultUpdateGizmoColor();
        public override void UpdateCollidersEnabled() => DefaultUpdateColliderEnabled();
        public void UpdateOriginalTile(TileSO tileSO) 
        {
            _originalTile = tileSO;
            _properties = tileSO.Properties;
            UpdateDisplayName();
        }

        public override bool TryRelocate(Vector3Int newCenterCell) 
        {
            var centerCell = CenterCell;
            if(newCenterCell == centerCell)
            {
                return false;
            }
            var cellOffset = newCenterCell - centerCell;
            UpdateAllCellsWithOffset(cellOffset);
            UpdateSocketsWithParentOffset(cellOffset);
            return DefaultTryTranslate(cellOffset);
        }
        public override bool TryTranslate(Vector3Int cellOffset)
        {
            if(cellOffset == Vector3Int.zero) 
            {
                return false;
            }
            UpdateAllCellsWithOffset(cellOffset);
            UpdateSocketsWithParentOffset(cellOffset);
            return DefaultTryTranslate(cellOffset);
        }
        public override bool TryResize(Vector2 scrollInput,RaycastHit hit,int length = 1)
        {
            Debug.Log("Resizing Tile Instances not implemented");
            return false;
        }
        private bool UpdateAllCellsWithOffset(Vector3Int cellOffset)
        {
            if (_cells == null || _cells.Count < 1)
            {
                return false;
            }
            Dictionary<Cell,CellInstance> updatedCells = new ();
            foreach (var cellInstance in _cells.Values)
            {
                var newCell = cellInstance.Cell + cellOffset;
                cellInstance.UpdateWithOffsetFromParent((Vector3Int)newCell);
                updatedCells.Add(newCell,cellInstance);
            }
            _cells = null;
            _cells = updatedCells;
            return true;
        }
        private bool UpdateSocketsWithParentOffset(Vector3Int cellOffset)
        {
            if (_sockets == null || _sockets.Count < 1)
            {
                return false;
            }
            for (int i = 0;i < _sockets.Count;i++)
            {
                _sockets[i]?.UpdateBoundWithOffsetFromParent(cellOffset);
            }
            return true;
        }

        public void AddNewBoundAndUpdateTile(CubeBound newBound) 
        {
            newBound = CellBound.Union(newBound);
            var boundMinOffset = newBound.min - CellBound.min;
            CellBound = newBound;
            
            HandleBoundMinChange();
            UpdateGizmoDimensions();
            DefaultUpdateColliderDimensions();

            void HandleBoundMinChange()
            {
                if (boundMinOffset == Vector3Int.zero)
                {
                    return;
                }
                var worldOffset = Vector3.Scale(boundMinOffset,CellSize);
                MainObject.transform.Translate(worldOffset);
                /// offset each Cell & Socket object 
                /// >> translate the object by the (-) min offset to compensate for the parent offset
                worldOffset = -worldOffset;

                foreach (var cellInstance in _cells.Values)
                {
                    if (cellInstance?.MainObject != null)
                    {
                        cellInstance.MainObject.transform.Translate(worldOffset);
                    }
                }
                for (int i = 0;i < _sockets.Count;i++)
                {
                    if (_sockets[i]?.MainObject != null)
                    {
                        _sockets[i].MainObject.transform.Translate(worldOffset);
                    }
                }
            }
        }
        public CellInstance InitializeCellInstance(Cell cell,CellTypes types = 0)
        {
            CellInstance cellInstance = new (Manager,Database,this,types,cell,true);
            _cells[cell] = cellInstance;
            return cellInstance;
        }
        public BaseSocketInstance InitializeNewSocket(Vector3Int cell,SocketTypes socketTypes,Directions directions)
        {
            _sockets ??= new();
            if (!IsValidSocketCell())
            {
                Debug.Log($"cell sent for socketInstance was invalid -> {cell}");
                return null;
            }
            if (_sockets.Count > 0 && socketTypes.HasFlags(SocketTypes.Origin))
            {
                if (IsDuplicateSocket())
                {
                    return null;
                }
            }
            else if (_sockets.Count > 0 && socketTypes.HasFlags(SocketTypes.Step))
            {
                if (IsDuplicateSocket())
                {
                    return null;
                }
            }

            var newSocketInstace = socketTypes.NewSocketInstance(Manager,Database,this,directions,cell);
            _sockets.Add(newSocketInstace);
            return newSocketInstace;

            bool IsValidSocketCell()
            {
                if (!_cells.TryGetValue((Cell)cell,out var cellInstance))
                {
                    Debug.Log($"no tile cell located at {cell}");
                    return false;
                }
                if (cellInstance.CellTypes.HasFlags(CellTypes.Air) == false)
                {
                    Debug.Log($"cell at {cell} CellTypesArrays is not a core cell => not valid socketInstance cell {cellInstance.CellTypes}");
                    return false;
                }
                return true;
            }
            bool IsDuplicateSocket()
            {
                for (int i = 0;i < _sockets.Count;i++)
                {
                    var socket = _sockets[i];
                    if (socket != null && socket.SocketTypes == socketTypes && socket.Directions == directions)
                    {
                        Debug.Log($"Tile already has a {socketTypes} socketInstance with {directions} directions ( located at {cell} )");
                        return true;
                    }
                }
                return false;
            }
        }
        public CellTypes[,,] GetCellTypesArray() 
        {
            var size = CellBound.size;
            var tileOrigin = CellBound.min;
            var cellTypesArrays = new CellTypes[size.x,size.y,size.z];
            for(int x = 0; x < size.x; x++) 
            {
                for(int y = 0; y < size.y; y++)
                {
                    for(int z = 0; z < size.z; z++) 
                    {
                        var offsetCell = tileOrigin + new Cell(x,y,z);
                        if(!_cells.TryGetValue(offsetCell,out var tileCell)) {
                            cellTypesArrays[x,y,z] = CellTypes.Void;
                            continue;
                        }
                        cellTypesArrays[x,y,z] = tileCell.CellTypes;
                    }
                }
            }
            return cellTypesArrays;
        }
        public void TryUpdateAllSocketPositions()
        {
            if (!Manager.AutoUpdateSockets || _sockets == null || _sockets.Count < 1)
            {
                return;
            }

            for (int i = 0;i < _sockets.Count;i++)
            {
                _sockets[i]?.TryUpdateSocketPosition();
            }
        }
        /// <summary>
        /// ?? Move to <see cref="ICellInstanceCollection"/> ??
        /// </summary>
        public bool TryGetCellWithTypesFromDirections(Vector3Int currentCell,CellTypes cellTypes,Directions directions,out Vector3Int validCell)
        {
            HashSet<Cell> validCells;
            var cells = Cells;
            foreach (var cubeBounds in HollowBoundUtils.SearchFromDirections(CellBound,directions,1))
            {
                validCells = new();
                for (int i = 0;i < cubeBounds.Bounds.Length;i++)
                {
                    var bound = cubeBounds.Bounds[i];
                    foreach (var cell in bound)
                    {
                        if (!cells.TryGetValue(cell,out var cellInstance) || cellInstance == null)
                        {
                            continue;
                        }
                        if (cellInstance.CellTypes.HasFlags(cellTypes))
                        {
                            validCells.Add(cell);
                        }
                    }
                }

                if (validCells.Count > 0)
                {
                    validCell = GetValidCell(cubeBounds.TargetPosition);
                    return true;
                }
            }

            Debug.Log($"Could not find valid cell for socket -> returning current cell {currentCell}");
            validCell = currentCell;
            return false;

            Vector3Int GetValidCell(Vector3? targetPosition)
            {
                if (targetPosition == null)
                {
                    return (Vector3Int)validCells.ToArray()[UnityEngine.Random.Range(0,validCells.Count)];
                }
                var target = (Vector3)targetPosition;
                List<Cell> nearCells = new (0);
                float? minDistance = null;
                foreach (var cell in validCells)
                {
                    var newDistance = (target - (Vector3Int)cell).magnitude;
                    if (minDistance == null || minDistance > newDistance)
                    {
                        minDistance = newDistance;
                        nearCells = new List<Cell> { cell };
                    }
                    else if (minDistance == newDistance)
                    {
                        nearCells.Add(cell);
                    }
                }
                return (Vector3Int)nearCells[UnityEngine.Random.Range(0,nearCells.Count)];
            }
        }
        public bool GetTypedSockets(out BaseSocket[] typedSockets)
        {
            if (_sockets != null && _sockets.Count > 0)
            {
                List<BaseSocket> tempSocketList = new ();
                var offset = -CellBound.min;
                for (int i = 0;i < _sockets.Count;i++)
                {
                    if (_sockets[i] != null)
                    {
                        tempSocketList.Add(_sockets[i].GetTypedSocket(offset));
                    }
                }
                typedSockets = tempSocketList.ToArray();
                return true;
            }
            typedSockets = null;
            return false;
        }
        public Dictionary<SocketTypes,int> GetCurrentSocketTypesCounts(out (Directions one,Directions two) stepDirs)
        {
            stepDirs = new (Directions.None,Directions.None);
        
            if (_sockets == null || _sockets.Count < 1)
            {
                return new(0);
            }
            Dictionary<SocketTypes,int> typesToCount = new()
            {
                { SocketTypes.None   , 0 },
                { SocketTypes.Origin , 0 },
                { SocketTypes.Seed   , 0 },
                { SocketTypes.Step   , 0 },
                { SocketTypes.Split  , 0 },
            };
            for (int i = 0;i < _sockets.Count;i++)
            {
                var socket = _sockets[i];
                if (socket != null && typesToCount.TryGetValue(socket.SocketTypes,out var count))
                {
                    typesToCount[socket.SocketTypes] = count + 1;
                    if (socket.SocketTypes == SocketTypes.Step)
                    {
                        if (stepDirs.one == Directions.None)
                        {
                            stepDirs.one = socket.Directions;
                        }
                        else if (stepDirs.two == Directions.None)
                        {
                            stepDirs.two = socket.Directions;
                        }
                    }
                }
            }
            return typesToCount;
        }

        public override bool TryUnregisterAndDestroy()
        {
            foreach (var socket in _sockets)
            { 
                socket?.TryUnregisterAndDestroy();
            }
            foreach (var cell in _cells.Values)
            {
                cell?.TryUnregisterAndDestroy();
            }
            return DefaultTryUnregisterAndDestroy();
        }
    }
}