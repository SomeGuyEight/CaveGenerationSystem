
namespace SlimeGame
{
    public static class LettersExtensions
    {
        public static bool IsNone(this Letters a) => a == 0;
        public static bool IsSingleFlag(this Letters a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this Letters a,Letters b) => (a & b) == b;
        public static Letters SharedFlags(this Letters a,Letters b) => a & b;
        public static Letters ToggleFlags(this Letters a,Letters b) => a ^= b;
        public static Letters SetFlags   (this Letters a,Letters b) => a | b;
        public static Letters UnsetFlags (this Letters a,Letters b) => a & (~b);
    }
}
