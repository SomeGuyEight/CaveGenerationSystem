using UnityEngine;

namespace SlimeGame
{
    public class OriginSocketInstance : BaseSocketInstance
    {
        public OriginSocketInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int cell,Directions directions,TileInstance parentTile) 
            : base (manager,database,cell,directions,parentTile)
        {

        }

        public override SocketTypes SocketTypes => SocketTypes.Origin;
        public override ColliderTypes ColliderTypes => ColliderTypes.OriginSocket;
        public override GizmoTypes GizmoTypes => GizmoTypes.OriginSocket;

        public override BaseSocket GetTypedSocket(Vector3Int? offset = null)
        {
            var localCell = Cell + (offset ?? Vector3Int.zero);
            return new OriginSocket(localCell);
        }
        public override BaseSocketInstance DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new OriginSocketInstance(Manager,Database,cell,Directions,ParentTile);
        }
    }
}