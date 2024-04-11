using UnityEngine;

namespace SlimeGame
{
    public class SplitSocketInstance : BaseSocketInstance
    {
        public SplitSocketInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int cell,Directions directions,TileInstance parentTile)
            : base(manager,database,cell,directions,parentTile) 
        {

        }

        public override SocketTypes SocketTypes => SocketTypes.Split;
        public override ColliderTypes ColliderTypes => ColliderTypes.SplitSocket;
        public override GizmoTypes GizmoTypes => GizmoTypes.SplitSocket;

        public override BaseSocket GetTypedSocket(Vector3Int? offset = null)
        {
            var localCell = Cell + (offset ?? Vector3Int.zero);
            return new SplitSocket(Directions,localCell);
        }
        public override BaseSocketInstance DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new SplitSocketInstance(Manager,Database,cell,Directions,ParentTile);
        }
    }
}
