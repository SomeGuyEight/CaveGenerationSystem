using System;

namespace SlimeGame
{
    [Flags]
    public enum CellTypes
    {
        None        = 0,            //  0

        Void        = 1,            //  1
        Surface     = 2,            //  2
        Air         = 4,            //  3
        Origin      = 8,            //  4

        Floor       = 1024,         // 11
        Wall        = 2048,         // 12
        Ceiling     = 4096,         // 13

        Transition  = 524288,       // 20

        AirOrigin   = Air | Origin,

        FloorSurface = Floor | Surface,
        WallSurface = Wall | Surface,
        CeilingSurface = Ceiling | Surface,

        TransitionFloorSurface = Transition | Floor | Surface,
        TransitionWallSurface =  Transition | Wall | Surface,
        TransitionCeilingSurface =  Transition | Ceiling | Surface,
    }   
}