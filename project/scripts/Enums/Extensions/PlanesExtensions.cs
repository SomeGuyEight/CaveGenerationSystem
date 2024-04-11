
namespace SlimeGame
{
    public static class PlanesExtensions
    {
        public static bool IsNone(this Planes a) => a == 0;
        public static bool IsSingleFlag(this Planes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this Planes a,Planes b) => (a & b) == b;
        public static Planes SharedFlags(this Planes a,Planes b) => a & b;
        public static Planes ToggleFlags(this Planes a,Planes b) => a ^= b;
        public static Planes SetFlags(this Planes a,Planes b) => a | b;
        public static Planes UnsetFlags(this Planes a,Planes b) => a & (~b);

        public static Planes OnlyPlanes(this Planes a) => a.SharedFlags(Planes.AllPlanes);
    }
}
