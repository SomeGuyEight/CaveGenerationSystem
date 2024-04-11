using UnityEngine;
using Sylves;

namespace SlimeGame 
{
    public abstract class BaseSocketInstance : BaseGenerationInstance 
    {
        public BaseSocketInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int cell,Directions directions,TileInstance parentTile) 
        {
            _cell = cell;
            _directions = directions;
            _parentTile = parentTile;
            BaseInitializeProperties(manager,database,cell,Vector3Int.one,parentTile);
        }

        private Vector3Int _cell;
        private readonly Directions _directions;
        private readonly TileInstance _parentTile;

        public abstract SocketTypes SocketTypes { get; }
        public abstract override ColliderTypes ColliderTypes { get; }
        public abstract override GizmoTypes GizmoTypes { get; }

        public Vector3Int Cell { get { return _cell; } }
        public Directions Directions { get { return _directions; } }
        public TileInstance ParentTile { get { return _parentTile; } }


        protected bool AreCollidersEnabled() => Manager.IsColliderEnabled(ColliderTypes,IsSelected ? IsSelected : ParentTile.IsSelected);

        public abstract BaseSocket GetTypedSocket(Vector3Int? offset = null);
        public abstract BaseSocketInstance DeepClone(Vector3Int? offset = null);

        protected override void CreateCellBound(Vector3Int spawnCell,Vector3Int size)
        {
            DefaultCreateCellBound(spawnCell,size);
        }
        protected override void CreateDisplayName()
        {
            DisplayName = $"({GetType().Name} :: {_cell} ) {SocketTypes} Socket : Directions {_directions}";
        }
        protected override void CreateMainObject() 
        {
            MainObject = new GameObject($"{DisplayName} {DateStamp} {TimeStamp}");
            MainObject.transform.position = WorldMin;
            MainObject.transform.parent = ParentTile.SocketHolder.transform;
        }
        protected override void CreateGizmo() => DefaultCreateGizmo();
        protected override void CreateColliders()
        {
            var collider = MainObject.AddComponent<BoxCollider>();
            collider.center = Vector3.Scale(CellSize,HalfVector3);
            collider.size = CellSize - new Vector3(.02f,.02f,.02f);
            collider.isTrigger = true;
            collider.enabled = AreCollidersEnabled();
            Colliders = new[] { collider };
        }
        protected override void RegisterWithInstanceManager() => DefaultRegisterInstanceWithManager();

        public override void UpdateDisplayName() 
        {
            DisplayName = $"({GetType().Name} :: {_cell} ) {SocketTypes} Socket : Directions {_directions}";
            if(MainObject != null) {
                MainObject.name = DisplayName;
            }
        }
        public override void UpdateGizmoEnabled() => DefaultUpdateGizmoEnabled();
        public override void UpdateGizmoDimensions() => DefaultUpdateGizmoDimensions();
        public override void UpdateGizmoColor() => DefaultUpdateGizmoColor();
        public override void UpdateCollidersEnabled()
        {
            if (Colliders == null)
            {
                return;
            }

            var isEnabled = AreCollidersEnabled();
            for (int i = 0;i < Colliders.Length;i++)
            {
                if (Colliders[i] != null && Colliders[i].enabled != isEnabled)
                {
                    Colliders[i].enabled = isEnabled;
                }
            }
        }

        public override bool TryRelocate(Vector3Int newCenterCell) 
        {
            if(ParentTile == null) {
                return false;
            }
            if (!ParentTile.Cells.TryGetValue((Cell)newCenterCell,out var cellInstance))
            {
                Debug.Log($"cellTypes invalid for socketInstance => {CellTypes.Void}");
                return false;
            }
            if (!cellInstance.CellTypes.HasFlags(CellTypes.Air))
            {
                Debug.Log($"cellTypes invalid for socketInstance => {cellInstance.CellTypes}");
                return false;
            }
            UpdateCell(newCenterCell);
            UpdatePosition(CellSize);
            UpdateGizmoDimensions();
            return true;
        }
        public override bool TryTranslate(Vector3Int moveVector)
        {
            var newCell = Cell + moveVector;
            return TryRelocate(newCell);
        }
        public override bool TryResize(Vector2 scrollInput,RaycastHit hit,int length = 1) 
        {
            Debug.Log("Resizing Socket Instances not implemented");
            return false;
        }

        public void UpdateBoundWithOffsetFromParent(Vector3Int cellOffset) 
        {
            var newCell = _cell + cellOffset;
            UpdateCell(newCell);
            UpdateGizmoDimensions();
        }
        public bool TryUpdateSocketPosition()
        {
            if (ParentTile == null || !ParentTile.TryGetCellWithTypesFromDirections(Cell,CellTypes.Air,Directions,out var validCell))
            {
                return false;
            }
            if ((validCell - Cell).magnitude < Manager.MinSocketUpdateDistance)
            {
                return false;
            }
            UpdateCell(validCell);
            UpdatePosition(CellSize);
            UpdateGizmoDimensions();
            return true;
        }
        private void UpdateCell(Vector3Int cell) 
        {
            _cell = cell;
            CellBound.min = cell;
            CellBound.max = cell + Vector3Int.one;
            UpdateDisplayName();
        }
        private void UpdatePosition(Vector3 cellSize)
        {
            if(MainObject == null) {
                return;
            }
            MainObject.transform.position = Vector3.Scale(cellSize,_cell);
            UpdateGizmoDimensions();
            UpdateDisplayName();
        }

        public override bool TryUnregisterAndDestroy() => DefaultTryUnregisterAndDestroy();

    }
}
