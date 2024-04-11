using System;

namespace SlimeGame
{
    public static class CornersExtensions
    {
        public static bool IsNone(this Corners a) => a == 0;
        public static bool IsSingleFlag(this Corners a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this Corners a,Corners b) => (a & b) == b;
        public static Corners SharedFlags(this Corners a,Corners b) => a & b;
        public static Corners ToggleFlags(this Corners a,Corners b) => a ^= b;
        public static Corners SetFlags   (this Corners a,Corners b) => a | b;
        public static Corners UnsetFlags (this Corners a,Corners b) => a & (~b);
        public static Corners Invert(this Corners corners) => corners.ToggleFlags(Corners.All);

        public static Directions DirectionsFromCorner(this Corners corners)
        {
            return corners switch
            {
                Corners.LDB => Directions.LeftDownBack,
                Corners.RDB => Directions.RightDownBack,
                Corners.LUB => Directions.LeftUpBack,
                Corners.RUB => Directions.RightUpBack,
                Corners.LDF => Directions.LeftDownFwd,
                Corners.RDF => Directions.RightDownFwd,
                Corners.LUF => Directions.LeftUpFwd,
                Corners.RUF => Directions.RightUpFwd,
                _ => throw new Exception("failed to get directions from corners"),
            };
        }
        public static bool DoCornersMaskDirections(this Corners flaggedCorners,Directions directions)
        {
            flaggedCorners = flaggedCorners.UnsetFlags(directions.Invert().ToCorners());
            return directions.ToCorners().HasFlags(flaggedCorners);
        }
    }
}
