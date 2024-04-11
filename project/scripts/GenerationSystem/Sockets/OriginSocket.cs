using UnityEngine;

namespace SlimeGame 
{
    public class OriginSocket : BaseSocket 
    {
        public OriginSocket(Vector3Int cell) : base(Directions.None,cell) 
        {

        }

        public override SocketTypes SocketTypes => SocketTypes.Origin;

        public override BaseSocket DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new OriginSocket(cell);
        }
        public override BaseSocketInstance GetSocketInstance(InstanceManager manager,MCTileDatabaseSO database,TileInstance parentTile,Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new OriginSocketInstance(manager,database,cell,Directions,parentTile);
        }
    }
}