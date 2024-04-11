using System;

namespace SlimeGame
{
    [Flags]
    public enum GenerationDebugMode
    {
        None                        = 0,
        ConstraintDebugOnFail       = 1,
        ConstraintDebugOnSuccess    = 2,
        SaveGenerationStats         = 4,
    }
}
