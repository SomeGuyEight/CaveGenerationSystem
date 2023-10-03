using System;
using System.Linq;
using Sylves;
using UnityEngine;

namespace Tessera.CaveGeneration {
    public static class CaveGenUtils {

        /// <summary>
        /// Sets the random Vector3Int representing the Anchor location just outside the Current CoreChunks border & just inside the neighboring chunks border.
        /// </summary>
        internal static Vector3Int HandleNewAnchorOffset(CubeDir cubeDir, Vector3Int chunkSizeInCells, int borderGenerationBoundry) {
            // create random anchor positions in this direction
            var maxX = chunkSizeInCells.x - borderGenerationBoundry;
            var maxY = chunkSizeInCells.y - borderGenerationBoundry;
            var maxZ = chunkSizeInCells.z - borderGenerationBoundry;
            var v3Int = new Vector3Int(UnityEngine.Random.Range(0 + borderGenerationBoundry, maxX), UnityEngine.Random.Range(0 + borderGenerationBoundry, maxY), UnityEngine.Random.Range(0 + borderGenerationBoundry, maxZ));

            // set the right axis value based on direction
            switch (cubeDir) {
                case CubeDir.Left: return new Vector3Int(-1, v3Int.y, v3Int.z);
                case CubeDir.Right: return new Vector3Int(chunkSizeInCells.x, v3Int.y, v3Int.z);
                case CubeDir.Up: return new Vector3Int(v3Int.x, chunkSizeInCells.y, v3Int.z);
                case CubeDir.Down: return new Vector3Int(v3Int.x, -1, v3Int.z);
                case CubeDir.Forward: return new Vector3Int(v3Int.x, v3Int.y, chunkSizeInCells.z);
                case CubeDir.Back: return new Vector3Int(v3Int.x, v3Int.y, -1);
            }
            Debug.Log($"Failed to add NEW AnchorOffset for cubeDir > {cubeDir}");
            throw new Exception();
        }
        /// <summary>
        /// Returns the Vector3Int representing the Anchor location just outside the Current CoreChunks border & just inside the neighboring chunks border.
        /// </summary>
        internal static Vector3Int HandleAnchorOffset(CubeDir cubeDir, Vector3Int chunkSizeInCells, Vector3Int anchorPositionToOffset) {
            switch (cubeDir) {
                case CubeDir.Left: return new Vector3Int(-1, anchorPositionToOffset.y, anchorPositionToOffset.z);
                case CubeDir.Right: return new Vector3Int(chunkSizeInCells.x, anchorPositionToOffset.y, anchorPositionToOffset.z);
                case CubeDir.Up: return new Vector3Int(anchorPositionToOffset.x, chunkSizeInCells.y, anchorPositionToOffset.z);
                case CubeDir.Down: return new Vector3Int(anchorPositionToOffset.x, -1, anchorPositionToOffset.z);
                case CubeDir.Forward: return new Vector3Int(anchorPositionToOffset.x, anchorPositionToOffset.y, chunkSizeInCells.z);
                case CubeDir.Back: return new Vector3Int(anchorPositionToOffset.x, anchorPositionToOffset.y, -1);
            }
            Debug.Log($"Failed to add Neighbor offset {anchorPositionToOffset} to AnchorOffset for cubeDir > {cubeDir}");
            throw new Exception();
        }
        /// <summary>
        /// Gets specific CubeCell neighbors adjacent to the given Cell & IGrid
        /// > CellDir[] should contain the new CellDir[6] with the CellDir checking in the proper index
        /// </summary>
        internal static Sylves.Cell[] GetSpecificCubeCellNeighbors(Sylves.Cell coreChunkCell, IGrid grid, CellDir[] cellDirArray) {
            Sylves.Cell[] coreChunkCellArray = new Sylves.Cell[6];
            foreach (var cellDir in grid.GetCellDirs(coreChunkCell)) {
                if (cellDirArray.Contains(cellDir)) {
                    if (grid.TryMove(coreChunkCell, cellDir, out var neighbor, out var _, out var _)) {
                        coreChunkCellArray[(int)cellDir] = neighbor;
                    }
                }
            }
            return coreChunkCellArray;
        }
        internal static int InvertCubeDirInt(int cubeDir) {
            var inverseDir = -1;
            switch (cubeDir) {
                case 0: inverseDir = 1; break;
                case 1: inverseDir = 0; break;
                case 2: inverseDir = 3; break;
                case 3: inverseDir = 2; break;
                case 4: inverseDir = 5; break;
                case 5: inverseDir = 4; break;
            }
            // check if switch failed to update inverseDir
            if (inverseDir == -1) {
                // failed inversion
                Debug.Log($"Failed to get inverse CubeDir of {(CubeDir) cubeDir}");
                throw new Exception();
            } else {
                // successful inversion
                return inverseDir;
            }
        }
        internal static bool TestIfVoidType(int dir, SubChunk subChunk) {
            if (subChunk.neighborArray[dir].type != SubChunkType.VoidVoid || subChunk.neighborArray[dir].type != SubChunkType.VoidTransition) {
                // non-void chunks share a non-void neighbor (index)dir
                return true;
            } else {
                // non-void chunks share a void neighbor in (index)dir
                return false;
            }
        }
        #region Not currently needed or redundant
        /// <summary>
        /// send the void chunk to validate >>
        /// send the two valid directions to test if the edge chunk is non-void
        /// >> confirm both neighbors are non-void
        /// >> if confirmed >> transition two neighbor chunks
        /// </summary>
        /// <param name="neighborDir0">neighbor direction adjacent to neighborDir1</param>
        /// <param name="neighborDir1">neighbor direction adjacent to neighborDir0</param>
        /// <param name="voidChunk"></param>
        /// <returns true>if transition IS valid</returns>
        /// <returns false>if transition is NOT valid</returns>
        internal static bool ValidateTransitionChunk(int neighborDir0, int neighborDir1, SubChunk voidChunk) {
            // check if edge chunk is valid non-void chunk
            if (!TestIfVoidType(neighborDir0, voidChunk.neighborArray[neighborDir1])) {
                // edge chunk is NOT a void chunk
                // invalid transition
                return false;
            }
            // edge chunk IS a valid non-void chunk
            // check if both neighbors are valid non-void
            if (!TestIfVoidType(neighborDir1, voidChunk) && !TestIfVoidType(neighborDir0, voidChunk)) {
                // at least one neighbor chunk is a void chunk 
                // invalid transition
                return false;
            }
            // both non-void => transition is valid 
            return true;
        }

        // call with CaveGenUtils.GetRelativeForwardAndUpOrient( dir, ref (Relative Right Dir), ref (Relative Up Dir));
        /// <summary>
        /// </summary>
        /// <param name="cubeFaceDir">Dir of face to find relative orientations for</param>
        /// <param name="rightOrient">Ref to be changed to Relative Right Dir</param>
        /// <param name="upOrient">Ref to be changed to Relative Up Dir</param>
        /// <returns true>successfully assigned relative orientations</returns>
        /// <returns false>failed to assigned relative orientations & Log the dir, and two orientation values</returns>
        internal static bool GetRelativeForwardAndUpOrient(int cubeFaceDir, ref int rightOrient, ref int upOrient) {
            // get the two positve axis to check for this face
            // initialize as -1 to catch fails later
            rightOrient = -1;
            upOrient = -1;
            switch (cubeFaceDir) {
                case 0: // left => get +z & +y
                    rightOrient = 4; upOrient = 2; break;
                case 1: // right => get -z & +y
                    rightOrient = 5; upOrient = 2; break;
                case 2: // up => get +x & -z
                    rightOrient = 1; upOrient = 5; break;
                case 3: // down => +x & +z
                    rightOrient = 1; upOrient = 4; break;
                case 4: // forward => get +x & +y
                    rightOrient = 1; upOrient = 2; break;
                case 5: // back => get -x & +y
                    rightOrient = 0; upOrient = 2; break;
            }
            if (rightOrient == -1 || upOrient == -1) {
                Debug.Log($"Failed to get orientation for dir {cubeFaceDir}; 'right' orient value == {rightOrient} ; 'up' orient value == {upOrient}");
                return false;
            }
            return true;
        }
        internal static int[] GetVertIndex(int[] arrayToUpdate, int forwardOrient, int checkDir, int stage) {
            if (checkDir == 2) {
                // check up
                if (forwardOrient == 4) {
                    // forward always the same here
                    arrayToUpdate[0] = 2;
                    arrayToUpdate[1] = 3;
                } else {

                    if (stage == 0) {
                        // the positive y in the same chunk
                        switch (forwardOrient) {
                            case 0: // left
                                arrayToUpdate[0] = 0; arrayToUpdate[1] = 2; break;
                            case 1: // right
                                arrayToUpdate[0] = 3; arrayToUpdate[1] = 1; break;
                            case 5: // back
                                arrayToUpdate[0] = 1; arrayToUpdate[1] = 0; break;
                        }
                    } else if (stage == 1) {
                        // the same face in the chunk above
                        // same as this face bottom two index
                        arrayToUpdate[0] = 2;
                        arrayToUpdate[1] = 3;
                    } else if (stage == 2) {
                        // fully bent back
                        // negative y in edge chunk
                        switch (forwardOrient) {
                            case 0: // left
                                arrayToUpdate[0] = 1; arrayToUpdate[1] = 3; break;
                            case 1: // right
                                arrayToUpdate[0] = 0; arrayToUpdate[1] = 2; break;
                            case 5: // back
                                arrayToUpdate[0] = 1; arrayToUpdate[1] = 0; break;
                        }
                    }
                }
            } else if (checkDir == 3) {
                // check down
                if (forwardOrient == 4) {
                    // forward always the same here too
                    arrayToUpdate[0] = 0;
                    arrayToUpdate[1] = 1;
                } else {
                    if (stage == 0) {
                        // the negative y in the same chunk
                        switch (forwardOrient) {
                            case 0: // left
                                arrayToUpdate[0] = 2; arrayToUpdate[1] = 0; break;
                            case 1: // right
                                arrayToUpdate[0] = 1; arrayToUpdate[1] = 3; break;
                            case 5: // back
                                arrayToUpdate[0] = 3; arrayToUpdate[1] = 2; break;
                        }
                    } else if (stage == 1) {
                        // the same face in the chunk below
                        // same as this face upper two index
                        arrayToUpdate[0] = 0;
                        arrayToUpdate[1] = 1;
                    } else if (stage == 2) {
                        // fully bent under
                        switch (forwardOrient) {
                            case 0: // left
                                arrayToUpdate[0] = 1; arrayToUpdate[1] = 3; break;
                            case 1: // right
                                arrayToUpdate[0] = 2; arrayToUpdate[1] = 0; break;
                            case 5: // back
                                arrayToUpdate[0] = 3; arrayToUpdate[1] = 2; break;
                        }

                    }
                }
            }
            return arrayToUpdate;
        }
        /// <summary>
        /// Returns Vector3Int representing all eight cell corners in world space
        /// </summary>
        /// <param name="cellDimensions">Must be an even whole number</param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static Vector3Int[] WorldCornerPositions(Vector3Int cellDimensions, Cell cell) {

            var cornerPositions = new Vector3Int[8];
            var halfDimensions = cellDimensions / 2;
            var worldPositionCellCenter = (((Vector3Int) cell) * cellDimensions) + halfDimensions;

            for (var i = 0; i < 8; i++) {
                cornerPositions[i] = worldPositionCellCenter + (halfDimensions * CornerOffset(i));
            }
            // returns a v3Int that represents the local corner with a scale of 1
            Vector3Int CornerOffset(int i) {
                switch (i) {
                    case 0: return new Vector3Int(-1, -1, -1);
                    case 1: return new Vector3Int(1, -1, -1);
                    case 2: return new Vector3Int(-1, 1, -1);
                    case 3: return new Vector3Int(1, 1, -1);
                    case 4: return new Vector3Int(-1, -1, 1);
                    case 5: return new Vector3Int(1, -1, 1);
                    case 6: return new Vector3Int(-1, 1, 1);
                    case 7: return new Vector3Int(1, 1, 1);
                }
                Debug.Log($"Returned Vector3Int.Zero, b/c there was no case for int <{i}>");
                return Vector3Int.zero;
            }
            //Debug.Log($"CornerPositions {cell} {cornerPositions[0]} == [0] -> {cornerPositions[1]} == [1] -> {cornerPositions[2]} == [2])");
            return cornerPositions;
        }
        #endregion
    }
}