using System;

namespace SlimeGame
{
    [Flags]
    public enum ControllerStates
    {
        None        = 1,
        Default     = 2,
        Place       = 4,
        Mine        = 8,
        Paint       = 16,
        Move        = 32,
        Select      = 64,
        Regenerate  = 128,
    }
}
