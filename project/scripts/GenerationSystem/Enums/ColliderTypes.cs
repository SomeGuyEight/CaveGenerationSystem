using System;

namespace SlimeGame
{
    [Flags]
    public enum ColliderTypes
    {
        None            = 0,
        Selected        = 1,
        Unselected      = 2,

        Bound           = 4,
        Tile            = 8,
        TileSet         = 16,
        Socket          = 32,
        Origin          = 64,
        Seed            = 128,
        Step            = 256,
        Split           = 512,
        Cell            = 1024,
        Void            = 2048,
        Air             = 4096,
        Surface         = 8192,

        VoidCell        = Void  | Cell,
        AirCell         = Air   | Cell,
        SurfaceCell     = Surface | Cell,
        CellTypes       = Void  | Air | Surface,
        AllCellValues   = Cell  | CellTypes,

        OriginSocket    = Origin | Socket,
        SeedSocket      = Seed | Socket,
        StepSocket      = Step | Socket,
        SplitSocket     = Split | Socket,
        SocketTypes     = Origin | Seed | Step | Split,
        AllSocketValues = Socket | SocketTypes,

        SelectedUnselected = Unselected | Selected,
        AllTypes = Bound | Tile | TileSet | AllSocketValues | AllCellValues,
        AllFlags = AllTypes | SelectedUnselected,
    }
}
