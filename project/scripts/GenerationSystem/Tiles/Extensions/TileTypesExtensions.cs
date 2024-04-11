
namespace SlimeGame
{
    public static class TileTypesExtensions
    {
        public static bool IsNone(this TileTypes a) => a == 0;
        public static bool IsSingleFlag(this TileTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this TileTypes a,TileTypes b) => (a & b) == b;
        public static TileTypes SharedFlags(this TileTypes a,TileTypes b) => a & b;
        public static TileTypes ToggleFlags(this TileTypes a,TileTypes b) => a ^= b;
        public static TileTypes SetFlags   (this TileTypes a,TileTypes b) => a | b;
        public static TileTypes UnsetFlags (this TileTypes a,TileTypes b) => a & (~b);
    }
}
