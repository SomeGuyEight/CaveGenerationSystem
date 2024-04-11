using System;

namespace SlimeGame
{
    [Flags]
    public enum Planes 
    {
        None        = 0,
        ZY          = 1,
        XZ          = 2,
        XY          = 4,
        Positive    = 8,    /// => Right || Up   || Fwd
        Negative    = 16,   /// => Left  || Down || Back

        ZYPositive  = ZY | Positive ,
        ZYNegative  = ZY | Negative ,
        XZPositive  = XZ | Positive ,
        XZNegative  = XZ | Negative ,
        XYPositive  = XY | Positive ,
        XYNegative  = XY | Negative ,

        ZYXZ        = ZY | XZ       ,
        ZYXY        = ZY | XY       ,
        XZXY        = XZ | XY       ,
        AllPlanes   = ZY | XZ | XY  ,
    }
}
