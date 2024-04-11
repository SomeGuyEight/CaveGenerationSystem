using System;

namespace SlimeGame
{
    [Flags]
    public enum TileShapes
    {
        None        = 0,
        Default     = 1,
        Wide        = 2,
        Tall        = 4,
        Diagonal    = 8,

        All = Default | Wide | Tall | Diagonal,
    }
}
