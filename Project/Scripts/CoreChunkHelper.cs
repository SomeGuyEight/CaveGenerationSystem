using System.Collections;
using System.Collections.Generic;
using Sylves;
using UnityEngine;

namespace Tessera.CaveGeneration {
    public enum CoreChunkStatus {
        // Never generate this chunk
        OutOfBounds,
        // Chunk is ready for generating
        Pending,
        // Chunk failed to generate after a successful setup
        FailedGeneration,
        // Chunk is running the generator in another thread
        Generating,
        // Generation finished successfully
        Generated,
        // Generation has finished, no tiles currently instantiated
        // TODO: Find better method than reinstantiating etc.
        // set up save/caching
        Memoized,
    }
    /// <summary>
    /// This is the base Chunk size for the Cave Generator
    /// > 16^3 cube of SubChunks
    /// </summary>
    public class CoreChunk {
        public string name { get; set; }
        public CoreChunkStatus status { get; set; }
        public Sylves.Cell cell { get; set; }
        public GameObject gameObject { get; set; }
        public TRS gridTransform { get; set; }
        internal float lastWatch { get; set; }
        public List<TesseraCompletion> completionList { get; set; }
        public Sylves.IGrid grid { get; set; }
        public Dictionary<Vector3Int, SubChunk> subChunkDictionary { get; set; }
        /// <summary>
        /// Each Chunk has it's own constraint builder attached to it's transform & grid 
        /// > this gives each chunk access to methods for creating constraints without placing tiles in the world
        /// </summary>
        public TesseraInitialConstraintBuilder constraintBuilder { get; set; }
        /// <summary>
        /// String should be $"{ChunkConstraintDirection.Right {int}"
        /// > Use the appropriate direction & int to save each indiviual constraint seperately for pulling later
        /// </summary>
        public Dictionary<CubeDir, List<Vector3Int>> anchorDictionary { get; set; }
        public Dictionary<CubeDir, List<ITesseraInitialConstraint>> anchorConstraintDictionary { get; set; }
        /// for collapsing SubChunks
        public List<SubChunk> voidTransitions { get; set; }
        public List<SubChunk> paths { get; set; }

    }
    public class CoreChunkHelper {

    }
    /// <summary>
    /// BackDownLeft[0] == *0,0,0*,
    /// BackDownRight[1],
    /// BackUpLeft[2],
    /// BackUpRight[3],
    /// ForwardDownLeft[4],
    /// ForwardDownRight[5],
    /// ForwardUpLeft[6],
    /// ForwardUpRight[7]
    /// </summary>
}