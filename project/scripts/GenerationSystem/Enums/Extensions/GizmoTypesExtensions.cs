
namespace SlimeGame
{
    public static class GizmoTypesExtensions
    {
        public static bool IsNone(this GizmoTypes a) => a == 0;
        public static bool IsSingleFlag(this GizmoTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this GizmoTypes a,GizmoTypes b) => (a & b) == b;
        public static GizmoTypes SharedFlags(this GizmoTypes a,GizmoTypes b) => a & b;
        public static GizmoTypes ToggleFlags(this GizmoTypes a,GizmoTypes b) => a ^= b;
        public static GizmoTypes SetFlags   (this GizmoTypes a,GizmoTypes b) => a | b;
        public static GizmoTypes UnsetFlags (this GizmoTypes a,GizmoTypes b) => a & (~b);
    }
}
