using UnityEngine;

namespace SlimeGame
{
    public static class SocketTypesExtensions
    {
        public static bool IsNone(this SocketTypes a) => a == 0;
        public static bool IsSingleFlag(this SocketTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this SocketTypes a,SocketTypes b) => (a & b) == b;
        public static SocketTypes SharedFlags(this SocketTypes a,SocketTypes b) => a & b;
        public static SocketTypes ToggleFlags(this SocketTypes a,SocketTypes b) => a ^= b;
        public static SocketTypes SetFlags(this SocketTypes a,SocketTypes b) => a | b;
        public static SocketTypes UnsetFlags(this SocketTypes a,SocketTypes b) => a & (~b);

        public static BaseSocket NewSocket(this SocketTypes types,Directions directions = Directions.None,Vector3Int? offset = null)
        {
            var cell = offset ?? Vector3Int.one;
            return types switch
            {
                SocketTypes.Origin  => new OriginSocket (cell),
                SocketTypes.Seed    => new SeedSocket   (directions,cell),
                SocketTypes.Step    => new StepSocket   (directions,cell),
                SocketTypes.Split   => new SplitSocket  (directions,cell),
                _                   => null,
            };
        }
        public static BaseSocketInstance NewSocketInstance(this SocketTypes types,InstanceManager manager, MCTileDatabaseSO database,TileInstance parentTile,Directions directions = Directions.None,Vector3Int? cell = null)
        {
            return types switch
            {
                SocketTypes.Origin  => new OriginSocketInstance (manager,database,cell ?? Vector3Int.one,directions,parentTile),
                SocketTypes.Seed    => new SeedSocketInstance   (manager,database,cell ?? Vector3Int.one,directions,parentTile),
                SocketTypes.Step    => new StepSocketInstance   (manager,database,cell ?? Vector3Int.one,directions,parentTile),
                SocketTypes.Split   => new SplitSocketInstance  (manager,database,cell ?? Vector3Int.one,directions,parentTile),
                _                   => null,
            };
        }
    }
}
