
namespace SlimeGame
{
    public static class DirectionTypesExtensions
    {
        public static bool IsNone(this DirectionTypes a) => a == 0;
        public static bool IsSingleFlag(this DirectionTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this DirectionTypes a,DirectionTypes b) => (a & b) == b;
        public static DirectionTypes SharedFlags(this DirectionTypes a,DirectionTypes b) => a & b;
        public static DirectionTypes ToggleFlags(this DirectionTypes a,DirectionTypes b) => a ^= b;
        public static DirectionTypes SetFlags   (this DirectionTypes a,DirectionTypes b) => a | b;
        public static DirectionTypes UnsetFlags (this DirectionTypes a,DirectionTypes b) => a & (~b);
    }
}
