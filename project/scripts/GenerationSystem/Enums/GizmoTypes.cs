using System;

namespace SlimeGame 
{
    [Flags]
    public enum GizmoTypes 
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

        VoidCell        = Void | Cell,
        AirCell         = Air | Cell,
        SurfaceCell     = Surface | Cell,
        CellSubTypes    = Void | Air | Surface,
        AllCellValues   = Cell | CellSubTypes,

        OriginSocket    = Origin | Socket,
        SeedSocket      = Seed | Socket,
        StepSocket      = Step | Socket,
        SplitSocket     = Split | Socket,
        SocketSubTypes  = Origin | Seed | Step | Split,
        AllSocketTypes  = Socket | SocketSubTypes,

        SelectedUnselected = Unselected | Selected,
        AllTypes = Bound | Tile | TileSet | Socket | Origin | Seed | Step | Split | Cell | Void | Air | Surface,
        AllFlags = AllTypes | SelectedUnselected,
    }
}
