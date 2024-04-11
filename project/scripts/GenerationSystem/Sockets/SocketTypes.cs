using System;

namespace SlimeGame
{
    [Flags]
    public enum SocketTypes
    {
        None    = 0,
        Origin  = 1,
        Seed    = 2,
        Step    = 4,
        Split   = 8,
    }
}
