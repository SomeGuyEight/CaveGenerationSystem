using System;

namespace SlimeGame
{
    [Flags]
    public enum TileSubTypes
    {
        None        = 0,
        Edge        = 1,
        Face        = 2,
        Corner      = 4,
    }
}
