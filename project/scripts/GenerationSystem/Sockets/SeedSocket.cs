using UnityEngine;

namespace SlimeGame 
{
    public class SeedSocket : BaseSocket 
    {
        public SeedSocket(Directions directions,Vector3Int cell) : base(directions,cell) 
        { 

        }

        public override SocketTypes SocketTypes => SocketTypes.Seed;

        public override BaseSocket DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new SeedSocket(Directions,cell);
        }
        public override BaseSocketInstance GetSocketInstance(InstanceManager manager,MCTileDatabaseSO database,TileInstance parentTile,Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new SeedSocketInstance(manager,database,cell,Directions,parentTile);
        }
    }
}