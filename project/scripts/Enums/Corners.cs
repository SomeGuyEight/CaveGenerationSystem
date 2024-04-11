using System;

namespace SlimeGame
{
    [Flags]
    public enum Corners
    {
        None        = 0,
        LDB         = 1,    /// LeftDownBack 
        RDB         = 2,    /// RightDownBack
        LUB         = 4,    /// LeftUpBack   
        RUB         = 8,    /// RightUpBack  
        LDF         = 16,   /// LeftDownFwd  
        RDF         = 32,   /// RightDownFwd 
        LUF         = 64,   /// LeftUpFwd    
        RUF         = 128,  /// RightUpFwd   

        All                 = LDB | RDB | LUB | RUB | LDF | RDF | LUF | RUF,
        Right               = RDB | RUB | RDF | RUF,
        Left                = LDB | LUB | LDF | LUF,
        Up                  = LUB | RUB | LUF | RUF,
        Down                = LDB | RDB | LDF | RDF,
        Fwd                 = LDF | RDF | LUF | RUF,
        Back                = LDB | RDB | LUB | RUB,
        RightUp             = RUB | RUF,
        RightDown           = RDB | RDF,
        RightFwd            = RDF | RUF,
        RightBack           = RDB | RUB,
        LeftUp              = LUB | LUF,
        LeftDown            = LDB | LDF,
        LeftFwd             = LDF | LUF,
        LeftBack            = LDB | LUB,
        UpFwd               = LUF | RUF,
        UpBack              = LUB | RUB,
        DownFwd             = LDF | RDF,
        DownBack            = LDB | RDB,
    }
}