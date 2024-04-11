using UnityEngine;

namespace SlimeGame
{
    public class SeedSocketInstance : BaseSocketInstance
    {
        public SeedSocketInstance(InstanceManager manager,MCTileDatabaseSO database,Vector3Int cell,Directions directions,TileInstance parentTile)
            : base(manager,database,cell,directions,parentTile) 
        { 

        }

        public override SocketTypes SocketTypes => SocketTypes.Seed;
        public override ColliderTypes ColliderTypes => ColliderTypes.SeedSocket;
        public override GizmoTypes GizmoTypes => GizmoTypes.SeedSocket;

        public override BaseSocket GetTypedSocket(Vector3Int? offset = null)
        {
            var localCell = Cell + (offset ?? Vector3Int.zero);
            return new SeedSocket(Directions,localCell);
        }
        public override BaseSocketInstance DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new SeedSocketInstance(Manager,Database,cell,Directions,ParentTile);
        }
    }
}
