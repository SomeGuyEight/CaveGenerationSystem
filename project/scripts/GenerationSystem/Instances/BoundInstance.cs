using UnityEngine;
using Sylves;

namespace SlimeGame 
{
    public class BoundInstance : BaseGenerationInstance 
    {
        public BoundInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int spawnCell,Vector3Int size)
        {
            BaseInitializeProperties(manager,database,spawnCell,size);
        }

        public override ColliderTypes ColliderTypes { get { return ColliderTypes.Bound; } }
        public override GizmoTypes GizmoTypes { get { return GizmoTypes.Bound; } }

        protected override void CreateCellBound(Vector3Int spawnCell,Vector3Int size)
        {
            DefaultCreateCellBound(spawnCell,size);
        }
        protected override void CreateDisplayName() => DefaultCreateDisplayName();
        protected override void CreateMainObject() => DefaultCreateMainObject();
        protected override void CreateGizmo() => DefaultCreateGizmo();
        protected override void CreateColliders() => DefaultCreateColliders();
        protected override void RegisterWithInstanceManager() => DefaultRegisterInstanceWithManager();

        public override void UpdateDisplayName() => DefaultUpdateDisplayName();
        public override void UpdateGizmoEnabled() => DefaultUpdateGizmoEnabled();
        public override void UpdateGizmoDimensions() => DefaultUpdateGizmoDimensions();
        public override void UpdateGizmoColor() => DefaultUpdateGizmoColor();
        public override void UpdateCollidersEnabled() => DefaultUpdateColliderEnabled();

        public override bool TryRelocate(Vector3Int newCenterCell) 
        {
            return DefaultTryRelocate(newCenterCell);
        }
        public override bool TryTranslate(Vector3Int cellOffset) 
        {
            return DefaultTryTranslate(cellOffset);
        }
        public override bool TryResize(Vector2 scrollInput,RaycastHit hit,int length = 1)
        {
            return DefaultTryResize(scrollInput,hit,length);
        }

        public override bool TryUnregisterAndDestroy()=> DefaultTryUnregisterAndDestroy();

        public void AddBound(CubeBound boundToAdd)
        {
            var min = CellBound.min;
            CellBound = CellBound.Union(boundToAdd);
            var minOffset = Vector3Int.Min(CellBound.min - min,Vector3Int.zero);
            MainObject.transform.position += Vector3.Scale(minOffset,CellSize);
            DefaultUpdateGizmoDimensions();
            DefaultUpdateColliderDimensions();
        }
    }
}
