using System;

namespace SlimeGame
{
    [Flags]
    public enum TileTypes
    {
        None            = 0,

        Horizontal      = 1,
        Vertical        = 2,
        Floor           = 4,
        Ceiling         = 8,

        Seed            = 16,
        Center          = 32,
        Transition      = 64,
        Cap             = 128,

        HorizontalCenter    = Horizontal    | Center,
        HorizontalCap       = Horizontal    | Cap,
        VerticalCenter      = Vertical      | Center,
        VerticalCap         = Vertical      | Cap,
        TransitionFloor     = Transition    | Floor,
        TransitionCeiling   = Transition    | Ceiling,
    }
}
