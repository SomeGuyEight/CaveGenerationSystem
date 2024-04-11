using UnityEngine;
using System;
using Sylves;

namespace SlimeGame
{
    public static class DirectionExtensions 
    {
        public static Planes GetPlane(this Direction direction) 
        {
            switch(direction)
            {
                case Direction.Right or Direction.Left: return Planes.ZY;
                case Direction.Up    or Direction.Down: return Planes.XZ;
                case Direction.Fwd   or Direction.Back: return Planes.XY;
                default: 
                    Debug.Log($"Failed to get plane from index -> returning Planes.None");
                    return Planes.None;
            }
        }
        public static int AxisPoint(this Direction direction,Vector3Int vector) 
        {
            return direction switch
            {
                Direction.Right or Direction.Left   => vector.x,
                Direction.Up    or Direction.Down   => vector.y,
                Direction.Fwd   or Direction.Back   => vector.z,
                _ => throw new Exception($"Failed to get Axis component from Directions"),
            };
        }
        public static Vector3Int AxisPosition(this Direction direction,Vector3Int position)
        {
            return direction switch
            {
                Direction.Right or Direction.Left   => new Vector3Int(position.x,0,0),
                Direction.Up    or Direction.Down   => new Vector3Int(0,position.y,0),
                Direction.Fwd   or Direction.Back   => new Vector3Int(0,0,position.z),
                _ => throw new Exception($"Failed to get World Axis Offsets {direction}"),
            };
        }
        public static Direction Invert(this Direction direction) {
            return direction switch
            {
                Direction.Right         => Direction.Left,
                Direction.Left          => Direction.Right,
                Direction.Up            => Direction.Down,
                Direction.Down          => Direction.Up,
                Direction.Fwd           => Direction.Back,
                Direction.Back          => Direction.Fwd,
                Direction.RightUp       => Direction.LeftDown,
                Direction.RightDown     => Direction.LeftUp,
                Direction.RightFwd      => Direction.LeftBack,
                Direction.RightBack     => Direction.LeftFwd,
                Direction.LeftUp        => Direction.RightDown,
                Direction.LeftDown      => Direction.RightUp,
                Direction.LeftFwd       => Direction.RightBack,
                Direction.LeftBack      => Direction.RightFwd,
                Direction.UpFwd         => Direction.DownBack,
                Direction.UpBack        => Direction.DownFwd,
                Direction.DownFwd       => Direction.UpBack,
                Direction.DownBack      => Direction.UpFwd,
                Direction.RightUpFwd    => Direction.LeftDownBack,
                Direction.RightUpBack   => Direction.LeftDownFwd,
                Direction.RightDownFwd  => Direction.LeftUpBack,
                Direction.RightDownBack => Direction.LeftUpFwd,
                Direction.LeftUpFwd     => Direction.RightDownBack,
                Direction.LeftUpBack    => Direction.RightDownFwd,
                Direction.LeftDownFwd   => Direction.RightUpBack,
                Direction.LeftDownBack  => Direction.RightUpFwd,
                _                       => Direction.None,
            };
        }
        public static Directions ToDirections(this Direction direction) {
             return direction switch
             {
                 Direction.Right         => Directions.Left,
                 Direction.Left          => Directions.Right,
                 Direction.Up            => Directions.Down,
                 Direction.Down          => Directions.Up,
                 Direction.Fwd           => Directions.Back,
                 Direction.Back          => Directions.Fwd,
                 Direction.RightUp       => Directions.LeftDown,
                 Direction.RightDown     => Directions.LeftUp,
                 Direction.RightFwd      => Directions.LeftBack,
                 Direction.RightBack     => Directions.LeftFwd,
                 Direction.LeftUp        => Directions.RightDown,
                 Direction.LeftDown      => Directions.RightUp,
                 Direction.LeftFwd       => Directions.RightBack,
                 Direction.LeftBack      => Directions.RightFwd,
                 Direction.UpFwd         => Directions.DownBack,
                 Direction.UpBack        => Directions.DownFwd,
                 Direction.DownFwd       => Directions.UpBack,
                 Direction.DownBack      => Directions.UpFwd,
                 Direction.RightUpFwd    => Directions.LeftDownBack,
                 Direction.RightUpBack   => Directions.LeftDownFwd,
                 Direction.RightDownFwd  => Directions.LeftUpBack,
                 Direction.RightDownBack => Directions.LeftUpFwd,
                 Direction.LeftUpFwd     => Directions.RightDownBack,
                 Direction.LeftUpBack    => Directions.RightDownFwd,
                 Direction.LeftDownFwd   => Directions.RightUpBack,
                 Direction.LeftDownBack  => Directions.RightUpFwd,
                 _                       => Directions.None,
             };
        }
        public static CellDir ToCellDir(this Direction direction) {
            return direction switch
            {
                Direction.Right => (CellDir)CubeDir.Right,
                Direction.Left  => (CellDir)CubeDir.Left,
                Direction.Up    => (CellDir)CubeDir.Up,
                Direction.Down  => (CellDir)CubeDir.Down,
                Direction.Fwd   => (CellDir)CubeDir.Forward,
                Direction.Back  => (CellDir)CubeDir.Back,
                _ => throw new Exception($"Failed to convert Dir -> CellDir"),
            };
        }
        public static CubeDir ToCubeDir(this Direction direction) {
            return direction switch
            {
                Direction.Right => CubeDir.Right,
                Direction.Left  => CubeDir.Left,
                Direction.Up    => CubeDir.Up,
                Direction.Down  => CubeDir.Down,
                Direction.Fwd   => CubeDir.Forward,
                Direction.Back  => CubeDir.Back,
                _ => throw new Exception($"Failed to convert Dir -> CubeDir"),
            };
        }
        public static Direction ToDirection(this CellDir dir) {
            return (CubeDir)dir switch
            {
                CubeDir.Right   => Direction.Right,
                CubeDir.Left    => Direction.Left,
                CubeDir.Up      => Direction.Up,
                CubeDir.Down    => Direction.Down,
                CubeDir.Forward => Direction.Fwd,
                CubeDir.Back    => Direction.Back,
                _ => throw new Exception($"Failed to convert Dir -> CellDir"),
            };
        }
        public static Direction ToDirection(this CubeDir dir) {
            return dir switch
            {
                CubeDir.Right   => Direction.Right,
                CubeDir.Left    => Direction.Left,
                CubeDir.Up      => Direction.Up,
                CubeDir.Down    => Direction.Down,
                CubeDir.Forward => Direction.Fwd,
                CubeDir.Back    => Direction.Back,
                _ => throw new Exception($"Failed to convert Dir -> CellDir"),
            };
        }
        public static Vector3Int ToVector(this Direction direction) {
            switch(direction) {
                case Direction.None:                return Vector3Int.zero;
                case Direction.Right:               return Vector3Int.right;
                case Direction.Left:                return Vector3Int.left;
                case Direction.Up:                  return Vector3Int.up;
                case Direction.Down:                return Vector3Int.down;
                case Direction.Fwd:                 return Vector3Int.forward;
                case Direction.Back:                return Vector3Int.back;
                case Direction.RightUp:             return Vector3Int.right + Vector3Int.up;
                case Direction.RightDown:           return Vector3Int.right + Vector3Int.down;
                case Direction.RightFwd:            return Vector3Int.right + Vector3Int.forward;
                case Direction.RightBack:           return Vector3Int.right + Vector3Int.back;
                case Direction.LeftUp:              return Vector3Int.left  + Vector3Int.up;
                case Direction.LeftDown:            return Vector3Int.left  + Vector3Int.down;
                case Direction.LeftFwd:             return Vector3Int.left  + Vector3Int.forward;
                case Direction.LeftBack:            return Vector3Int.left  + Vector3Int.back;
                case Direction.UpFwd:               return Vector3Int.up    + Vector3Int.forward;
                case Direction.UpBack:              return Vector3Int.up    + Vector3Int.back;
                case Direction.DownFwd:             return Vector3Int.down  + Vector3Int.forward;
                case Direction.DownBack:            return Vector3Int.down  + Vector3Int.back;
                case Direction.RightUpFwd:          return Vector3Int.right + Vector3Int.up     + Vector3Int.forward;
                case Direction.RightUpBack:         return Vector3Int.right + Vector3Int.up     + Vector3Int.back;
                case Direction.RightDownFwd:        return Vector3Int.right + Vector3Int.down   + Vector3Int.forward;
                case Direction.RightDownBack:       return Vector3Int.right + Vector3Int.down   + Vector3Int.back;
                case Direction.LeftUpFwd:           return Vector3Int.left  + Vector3Int.up     + Vector3Int.forward;
                case Direction.LeftUpBack:          return Vector3Int.left  + Vector3Int.up     + Vector3Int.back;
                case Direction.LeftDownFwd:         return Vector3Int.left  + Vector3Int.down   + Vector3Int.forward;
                case Direction.LeftDownBack:        return Vector3Int.left  + Vector3Int.down   + Vector3Int.back;
                default:
                    Debug.Log($"Failed to get Vector3Int from Directions -> returning Vector3Int.zero");
                    return Vector3Int.zero;
            }
        }      
        public static Vector3Int RelativeRight(this Direction direction) {
            return direction switch
            {
                Direction.Right         => Vector3Int.back,
                Direction.Left          => Vector3Int.forward,
                Direction.Up            => Vector3Int.right,
                Direction.Down          => Vector3Int.right,
                Direction.Fwd           => Vector3Int.right,
                Direction.Back          => Vector3Int.left,
                Direction.RightUp       => Vector3Int.back,
                Direction.RightDown     => Vector3Int.back,
                Direction.RightFwd      => new Vector3Int(1,0,-1),
                Direction.RightBack     => new Vector3Int(-1,0,-1),
                Direction.LeftUp        => Vector3Int.forward,
                Direction.LeftDown      => Vector3Int.forward,
                Direction.LeftFwd       => new Vector3Int(1,0,1),
                Direction.LeftBack      => new Vector3Int(-1,0,1),
                Direction.UpFwd         => Vector3Int.right,
                Direction.UpBack        => Vector3Int.left,
                Direction.DownFwd       => Vector3Int.right,
                Direction.DownBack      => Vector3Int.left,
                Direction.RightUpFwd    => new Vector3Int(1,0,-1),
                Direction.RightUpBack   => new Vector3Int(-1,0,-1),
                Direction.RightDownFwd  => new Vector3Int(1,0,-1),
                Direction.RightDownBack => new Vector3Int(-1,0,-1),
                Direction.LeftUpFwd     => new Vector3Int(1,0,1),
                Direction.LeftUpBack    => new Vector3Int(-1,0,1),
                Direction.LeftDownFwd   => new Vector3Int(1,0,1),
                Direction.LeftDownBack  => new Vector3Int(-1,0,1),
                _                       => Vector3Int.zero,
            };
        }
        public static DirectionTypes GetDirectionType(this Direction direction) {
            return (int)direction switch
            {
                0    => DirectionTypes.None,
                < 7  => DirectionTypes.Face,
                < 19 => DirectionTypes.Edge,
                < 27 => DirectionTypes.Corner,
                _    => DirectionTypes.None,
            };
        }
        public static Vector3Int[] AllRelativeVectors(this Direction direction) {
            switch(direction) {
                case Direction.Right:   return AllRightRelativeVectors();
                case Direction.Left:    return AllLeftRelativeVectors();
                case Direction.Up:      return AllUpRelativeVectors();
                case Direction.Down:    return AllDownRelativeVectors();
                case Direction.Fwd:     return AllForwardRelativeVectors();
                case Direction.Back:    return AllBackRelativeVectors();
                default:
                    Debug.Log($"Failed to get Vector3Int[] with AllTypes Relative Vectors from Directions -> throwing");
                    throw new Exception();
            }
        }
        public static Vector3Int[] AllRightRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.forward,
                    Vector3Int.back,   
                    Vector3Int.up,
                    Vector3Int.down,
                    Vector3Int.right,
                    Vector3Int.left,
            };
        }
        public static Vector3Int[] AllLeftRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.forward,
                    Vector3Int.back,
                    Vector3Int.up,
                    Vector3Int.down,
                    Vector3Int.left,
                    Vector3Int.right,
            };
        }
        public static Vector3Int[] AllUpRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.right,
                    Vector3Int.left, 
                    Vector3Int.forward,
                    Vector3Int.back,
                    Vector3Int.up,
                    Vector3Int.down,
            };
        }
        public static Vector3Int[] AllDownRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.forward,
                    Vector3Int.back,
                    Vector3Int.down,
                    Vector3Int.up,
            };
        }
        public static Vector3Int[] AllForwardRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.down,
                    Vector3Int.forward,
                    Vector3Int.back,
            };
        }
        public static Vector3Int[] AllBackRelativeVectors() {
            return new Vector3Int[] {
                    Vector3Int.right,
                    Vector3Int.left, 
                    Vector3Int.up,
                    Vector3Int.down,
                    Vector3Int.back,
                    Vector3Int.forward,
            };
        }
    }
}
