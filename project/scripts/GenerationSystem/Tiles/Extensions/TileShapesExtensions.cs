
namespace SlimeGame
{
    public static class TileShapesExtensions
    {
        public static bool IsNone(this TileShapes a) => a == 0;
        public static bool IsSingleFlag(this TileShapes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this TileShapes a,TileShapes b) => (a & b) == b;
        public static TileShapes SharedFlags(this TileShapes a,TileShapes b) => a & b;
        public static TileShapes ToggleFlags(this TileShapes a,TileShapes b) => a ^= b;
        public static TileShapes SetFlags   (this TileShapes a,TileShapes b) => a | b;
        public static TileShapes UnsetFlags (this TileShapes a,TileShapes b) => a & (~b);
    }
}
