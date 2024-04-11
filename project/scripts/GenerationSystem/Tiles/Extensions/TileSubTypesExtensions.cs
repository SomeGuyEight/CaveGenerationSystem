
namespace SlimeGame
{
    public static class TileSubTypesExtensions
    {
        public static bool IsNone(this TileSubTypes a) => a == 0;
        public static bool IsSingleFlag(this TileSubTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this TileSubTypes a,TileSubTypes b) => (a & b) == b;
        public static TileSubTypes SharedFlags(this TileSubTypes a,TileSubTypes b) => a & b;
        public static TileSubTypes ToggleFlags(this TileSubTypes a,TileSubTypes b) => a ^= b;
        public static TileSubTypes SetFlags   (this TileSubTypes a,TileSubTypes b) => a | b;
        public static TileSubTypes UnsetFlags (this TileSubTypes a,TileSubTypes b) => a & (~b);
    }
}
