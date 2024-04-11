using System;

namespace SlimeGame
{
    [Flags]
    public enum TileSizes
    {
        None        = 0,
        Small       = 1,
        Medium      = 2,
        Large       = 4,
        ExtraLarge  = 8,

        All = Small | Medium | Large | ExtraLarge,
    }
}
