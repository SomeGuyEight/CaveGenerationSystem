using System.Collections;
using System.Collections.Generic;
using Sylves;
using UnityEngine;

namespace Tessera.CaveGeneration {

    public enum SubChunkType {
        VoidVoid,
        VoidTransition,
        PathPath,
        PathCavern,
        PathRavine,
        PathVent,
        PathMulti,
        CavernVoid,
        CavernPath,
        CavernCavern,
        CavernRavine,
        CavernVent,
        CavernMulti,
        RavinePath,
        RavineCavern,
        RavineRavine,
        RavineVent,
        RavineMulti,
        VentPath,
        VentCavern,
        VentRavine,
        VentVent,
        VentMulti,
    }

    public enum SubChunkStatus {
        OutOfBounds,
        // Chunk is ready for generating
        Pending,
        /// Not self collapsed, but neighbor has updated
        NeighborUpdated,
        /// Has done its own neighbor check
        CollapsedOnce,
        CollapsedTwice,
        CollapsedThrice,
        /// Collapsed but do not change its mesh/geometry
        CollapsedProtected,
        /// Mesh Generated -> CAN edited/manipulate
        MeshGenerated,
        /// Mesh Generated -> can NOT edit/manipulate
        MeshGeneratedProtected,
        /// Instance created -> CAN edited/manipulate
        Instantiated,
        /// Instance created -> can NOT edit/manipulate
        InstantiatedProtected,
        /// Generation fully complete -> CAN edited/manipulate
        Finalized,
        /// Generation fully complete -> can NOT edit/manipulate
        FinalizedProtected,
        // Chunk is running the generator in another thread
        Generating,
        // Generation and instantiating finished successfully
        Generated,
    }

    public class SubChunk {
        public GameObject gameObject { get; set; }
        public TRS gridTransform { get; set; }
        public SubChunkStatus status { get; set; }
        public SubChunkType type { get; set; }
        public CoreChunk parentCoreChunk { get; set; }
        public Sylves.Cell coreCell { get; set; }
        public Sylves.IGrid grid { get; set; }
        public SubChunk[] neighborArray { get; set; }
        public Dictionary<int,Vector3[]> vertPositions { get; set; }


        internal float lastWatch { get; set; }

        public int pathIntersectCount { get; set; }
        public int voidNeighborCount { get; set; }
        public int pathNeighborCount { get; set; }
        public int cavernNeighborCount { get; set; }
        public int ravineNeighborCount { get; set; }
        public int ventNeighborCount { get; set; }
    }

    public class SubChunkHelper {

    }
}
