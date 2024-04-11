using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame
{

    public static class DirectionsExtensions
    {
        public static bool  IsNone(this Directions a) => a == 0;
        public static bool  IsSingleFlag(this Directions a) => a != 0 && (a & (a - 1)) == 0;
        public static bool  HasFlags(this Directions a,Directions b) => (a & b) == b;
        public static Directions SharedFlags(this Directions a,Directions b) => a & b;
        public static Directions ToggleFlags(this Directions a,Directions b) => a ^= b;
        public static Directions SetFlags   (this Directions a,Directions b) => a | b;
        public static Directions UnsetFlags (this Directions a,Directions b) => a & (~b);

        public static Directions Invert(this Directions directions)
        {
            if (directions.HasFlags(Directions.Right) || directions.HasFlags(Directions.Left))
            {
                directions = directions.ToggleFlags(Directions.RightLeft);
            }
            if (directions.HasFlags(Directions.Up) || directions.HasFlags(Directions.Down))
            {
                directions = directions.ToggleFlags(Directions.UpDown);
            }
            if (directions.HasFlags(Directions.Fwd) || directions.HasFlags(Directions.Back))
            {
                directions = directions.ToggleFlags(Directions.FwdBack);
            }
            return directions;
        }

        public static bool IsValidType(this Directions directions)
        {
            return !directions.HasFlags(Directions.Right | Directions.Left) && !directions.HasFlags(Directions.Up | Directions.Down) && !directions.HasFlags(Directions.Fwd | Directions.Back);
        }
        public static bool IsFaceDir(this Directions directions)
        {
            return directions switch
            {
                Directions.Right or Directions.Left or Directions.Up or Directions.Down or Directions.Fwd or Directions.Back => true,
                _ => false,
            };
        }

        public static Directions[] GetAllFlags(this Directions directions,bool includeNone)
        {
            List<Directions> flaggedDirs = includeNone ? new (27) { Directions.None } : new (26);
            foreach (var dirs in DirectionsHelper.AllValidDirections)
            {
                if (directions.HasFlags(dirs))
                {
                    flaggedDirs.Add(dirs);
                }
            }
            return flaggedDirs.ToArray();
        }
        public static Vector3Int ToVector(this Directions directions)
        {
            var x = directions.HasFlags(Directions.Right) ? 1 : directions.HasFlags(Directions.Left) ? -1 : 0;
            var y = directions.HasFlags(Directions.Up   ) ? 1 : directions.HasFlags(Directions.Down) ? -1 : 0;
            var z = directions.HasFlags(Directions.Fwd  ) ? 1 : directions.HasFlags(Directions.Back) ? -1 : 0;
            return new(x,y,z);
        }
        /// <summary>
        /// ( ! ) needs to be a valid Directions -> no inverse pairs flagged ( eg. "Right" &amp; "Left" flagged)
        /// </summary>
        public static Direction ToDirection(this Directions directions)
        {
            return directions switch
            {
                Directions.Right            => Direction.Right,
                Directions.Left             => Direction.Left,
                Directions.Up               => Direction.Up,
                Directions.Down             => Direction.Down,
                Directions.Fwd              => Direction.Fwd,
                Directions.Back             => Direction.Back,
                Directions.RightUp          => Direction.RightUp,
                Directions.RightDown        => Direction.RightDown,
                Directions.RightFwd         => Direction.RightFwd,
                Directions.RightBack        => Direction.RightBack,
                Directions.LeftUp           => Direction.LeftUp,
                Directions.LeftDown         => Direction.LeftDown,
                Directions.LeftFwd          => Direction.LeftFwd,
                Directions.LeftBack         => Direction.LeftBack,
                Directions.UpFwd            => Direction.UpFwd,
                Directions.UpBack           => Direction.UpBack,
                Directions.DownFwd          => Direction.DownFwd,
                Directions.DownBack         => Direction.DownBack,
                Directions.RightUpFwd       => Direction.RightUpFwd,
                Directions.RightUpBack      => Direction.RightUpBack,
                Directions.RightDownFwd     => Direction.RightDownFwd,
                Directions.RightDownBack    => Direction.RightDownBack,
                Directions.LeftUpFwd        => Direction.LeftUpFwd,
                Directions.LeftUpBack       => Direction.LeftUpBack,
                Directions.LeftDownFwd      => Direction.LeftDownFwd,
                Directions.LeftDownBack     => Direction.LeftDownBack,
                _                           => Direction.None,
            };
        }

        /// <summary>
        /// ( ! ) needs to be a Face Direction input ( eg. "Right")
        /// </summary>
        public static CellDir ToCellDir(this Directions directions)
        {
            return directions switch
            {
                Directions.Right => (CellDir)CubeDir.Right,
                Directions.Left  => (CellDir)CubeDir.Left,
                Directions.Up    => (CellDir)CubeDir.Up,
                Directions.Down  => (CellDir)CubeDir.Down,
                Directions.Fwd   => (CellDir)CubeDir.Forward,
                Directions.Back  => (CellDir)CubeDir.Back,
                _ => throw new Exception("failed to get CellDir from Direcitons in switch"),
            };
        }        
        
        public static Corners ToCorners(this Directions directions)
        {
            return directions switch
            {
                Directions.None             => Corners.All,
                Directions.Right            => Corners.Right,
                Directions.Left             => Corners.Left,
                Directions.Up               => Corners.Up,
                Directions.Down             => Corners.Down,
                Directions.Fwd              => Corners.Fwd,
                Directions.Back             => Corners.Back,
                Directions.RightUp          => Corners.RightUp,
                Directions.RightDown        => Corners.RightDown,
                Directions.RightFwd         => Corners.RightFwd,
                Directions.RightBack        => Corners.RightBack,
                Directions.LeftUp           => Corners.LeftUp,
                Directions.LeftDown         => Corners.LeftDown,
                Directions.LeftFwd          => Corners.LeftFwd,
                Directions.LeftBack         => Corners.LeftBack,
                Directions.UpFwd            => Corners.UpFwd,
                Directions.UpBack           => Corners.UpBack,
                Directions.DownFwd          => Corners.DownFwd,
                Directions.DownBack         => Corners.DownBack,
                Directions.LeftDownBack     => Corners.LDB,
                Directions.RightDownBack    => Corners.RDB,
                Directions.LeftUpBack       => Corners.LUB,
                Directions.RightUpBack      => Corners.RUB,
                Directions.LeftDownFwd      => Corners.LDF,
                Directions.RightDownFwd     => Corners.RDF,
                Directions.LeftUpFwd        => Corners.LUF,
                Directions.RightUpFwd       => Corners.RUF,
                _ => throw new Exception("failed to get corners mask from direcitons switch"),
            };
        }
    }
}
