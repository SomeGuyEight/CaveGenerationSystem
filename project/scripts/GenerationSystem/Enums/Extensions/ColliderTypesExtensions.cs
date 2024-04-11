
namespace SlimeGame
{
    public static class ColliderTypesExtensions
    {
        public static bool IsNone(this ColliderTypes a) => a == 0;
        public static bool IsSingleFlag(this ColliderTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this ColliderTypes a,ColliderTypes b) => (a & b) == b;
        public static ColliderTypes SharedFlags(this ColliderTypes a,ColliderTypes b) => a & b;
        public static ColliderTypes ToggleFlags(this ColliderTypes a,ColliderTypes b) => a ^= b;
        public static ColliderTypes SetFlags   (this ColliderTypes a,ColliderTypes b) => a | b;
        public static ColliderTypes UnsetFlags (this ColliderTypes a,ColliderTypes b) => a & (~b);
    }
}
