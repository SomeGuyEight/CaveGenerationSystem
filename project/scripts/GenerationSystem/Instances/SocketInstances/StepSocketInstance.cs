using UnityEngine;

namespace SlimeGame
{
    public class StepSocketInstance : BaseSocketInstance
    {
        public StepSocketInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int cell,Directions directions,TileInstance parentTile)
            : base(manager,database,cell,directions,parentTile)
        {

        }

        public override SocketTypes SocketTypes => SocketTypes.Step;
        public override ColliderTypes ColliderTypes => ColliderTypes.StepSocket;
        public override GizmoTypes GizmoTypes => GizmoTypes.StepSocket;

        public override BaseSocket GetTypedSocket(Vector3Int? offset = null)
        {
            var localCell = Cell + (offset ?? Vector3Int.zero);
            return new StepSocket(Directions,localCell);
        }
        public override BaseSocketInstance DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new StepSocketInstance(Manager,Database,cell,Directions,ParentTile);
        }
    }
}
