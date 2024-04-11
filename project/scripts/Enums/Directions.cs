using System;

namespace SlimeGame 
{
    [Flags]
    public enum Directions 
    {
        None            = 0,
        Right           = 1,
        Left            = 2,
        Up              = 4,
        Down            = 8,
        Fwd             = 16,
        Back            = 32,

        RightUp         = Right | Up,
        RightDown       = Right | Down,
        RightFwd        = Right | Fwd,
        RightBack       = Right | Back,
        LeftUp          = Left  | Up,
        LeftDown        = Left  | Down,
        LeftFwd         = Left  | Fwd,
        LeftBack        = Left  | Back,
        UpFwd           = Up    | Fwd,
        UpBack          = Up    | Back,
        DownFwd         = Down  | Fwd,
        DownBack        = Down  | Back,

        RightUpFwd      = Right | Up    | Fwd,
        RightUpBack     = Right | Up    | Back,
        RightDownFwd    = Right | Down  | Fwd,
        RightDownBack   = Right | Down  | Back,
        LeftUpFwd       = Left  | Up    | Fwd,
        LeftUpBack      = Left  | Up    | Back,
        LeftDownFwd     = Left  | Down  | Fwd,
        LeftDownBack    = Left  | Down  | Back,

        RightLeft       = Right | Left,
        UpDown          = Up | Down,
        FwdBack         = Fwd | Back,

        All = Right | Left | Up | Down | Fwd | Back,
    }
}