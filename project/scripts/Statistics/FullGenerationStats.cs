using UnityEngine;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class FullGenerationStats
    {
        public FullGenerationStats() { }
        public FullGenerationStats(string name,CubeBound bound) { }
        public string Name;
        public Vector3Int Min;
        public Vector3Int Size;
        public int Volume => Size.x * Size.y * Size.z;
        public int VoidCells;
        public int SurfaceCells;
        public int CoreCells;
        public TesseraStats Stats;
        public TesseraCompletion Completion;
    }
}
