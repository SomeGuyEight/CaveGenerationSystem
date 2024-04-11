using UnityEngine;

namespace SlimeGame 
{
    public class StepSocket : BaseSocket
    {
        public StepSocket(Directions directions,Vector3Int cell) : base(directions,cell) 
        { 

        }

        public override SocketTypes SocketTypes => SocketTypes.Step;

        public override BaseSocket DeepClone(Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new StepSocket(Directions,cell);
        }
        public override BaseSocketInstance GetSocketInstance(InstanceManager manager,MCTileDatabaseSO database,TileInstance parentTile,Vector3Int? offset = null)
        {
            var cell = Cell + (offset ?? Vector3Int.zero);
            return new OriginSocketInstance(manager,database,cell,Directions,parentTile);
        }
    }
}