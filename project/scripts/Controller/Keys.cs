using System;

namespace SlimeGame
{
    [Flags]
    public enum Keys
    {
        None        = 0,
        num0        = 1,
        num1        = 2,
        num2        = 4,
        num3        = 8,
        num4        = 16,
        num5        = 32,
        num6        = 64,
        num7        = 128,
        num8        = 256,
        num9        = 512,
        tab         = 1024,
        esc         = 2048,
        caps        = 4096,
        shift       = 8192,
        ctrl        = 16384,
        space       = 32768,
        enter       = 65536,
        rightArrow  = 131072,
        leftArrow   = 262144,
        upArrow     = 524288,
        downArrow   = 1048576,
    }
}
