
namespace SlimeGame
{
    public static class GenerationDebugModesExtensions
    {
        public static bool IsNone(this GenerationDebugMode a) => a == 0;
        public static bool IsSingleFlag(this GenerationDebugMode a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this GenerationDebugMode a,GenerationDebugMode b) => (a & b) == b;
        public static GenerationDebugMode SharedFlags(this GenerationDebugMode a,GenerationDebugMode b) => a & b;
        public static GenerationDebugMode ToggleFlags(this GenerationDebugMode a,GenerationDebugMode b) => a ^= b;
        public static GenerationDebugMode SetFlags   (this GenerationDebugMode a,GenerationDebugMode b) => a | b;
        public static GenerationDebugMode UnsetFlags (this GenerationDebugMode a,GenerationDebugMode b) => a & (~b);
    }
}
