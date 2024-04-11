
namespace SlimeGame
{
    public static class KeysExtensions
    {
        public static bool IsNone(this Keys a) => a == 0;
        public static bool IsSingleFlag(this Keys a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this Keys a,Keys b) => (a & b) == b;
        public static Keys SharedFlags(this Keys a,Keys b) => a & b;
        public static Keys ToggleFlags(this Keys a,Keys b) => a ^= b;
        public static Keys SetFlags   (this Keys a,Keys b) => a | b;
        public static Keys UnsetFlags (this Keys a,Keys b) => a & (~b);
    }
}
