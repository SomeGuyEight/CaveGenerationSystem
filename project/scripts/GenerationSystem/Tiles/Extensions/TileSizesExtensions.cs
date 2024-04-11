
namespace SlimeGame
{
    public static class TileSizesExtensions
    {
        public static bool IsNone(this TileSizes a) => a == 0;
        public static bool IsSingleFlag(this TileSizes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this TileSizes a,TileSizes b) => (a & b) == b;
        public static TileSizes SharedFlags(this TileSizes a,TileSizes b) => a & b;
        public static TileSizes ToggleFlags(this TileSizes a,TileSizes b) => a ^= b;
        public static TileSizes SetFlags   (this TileSizes a,TileSizes b) => a | b;
        public static TileSizes UnsetFlags (this TileSizes a,TileSizes b) => a & (~b);
    }
}
