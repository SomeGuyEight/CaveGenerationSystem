using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sylves;
using System.Linq;

namespace Tessera.CaveGeneration {
    #region Future Additions
    /*
    // Attribute with resolved action string.
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class OnClickActionAttribute : Attribute {
       public string ActionString { get; private set; }

       public OnClickActionAttribute(string actionString) {
          this.ActionString = actionString;
       }
    }
    */
    #endregion
    
    public class CaveGenManager : MonoBehaviour {
        [Header("Scanning")]
        /// <summary>    
        /// Player GameObject to moniter
        /// </summary>
        [SerializeField] GameObject[] _playerObjects;
        /// <summary>
        /// Number of frames between scans to detect if new chunks need generating
        /// </summary>
        [SerializeField] int _scanInterval = 0;
        /// <summary>    
        /// Distance from player to scan for chunk generation
        /// TODO: set how many coreChunks out the scan should look
        /// </summary>
        [SerializeField] int _scanRadius = 1;
        /// <summary>
        /// Number of chunks that can be generated concurrently
        /// </summary>
        [SerializeField] int _parallelism = 1;
        /// <summary>
        /// Infinite generation boundries
        /// </summary>
        [SerializeField] bool infiniteX = true;
        [SerializeField] bool infiniteY = true;
        [SerializeField] bool infiniteZ = true;

        [SerializeField] int minXChunk = 0;
        [SerializeField] int maxXChunk = 0;
        [SerializeField] int minYChunk = 0;
        [SerializeField] int maxYChunk = 0;
        [SerializeField] int minZChunk = 0;
        [SerializeField] int maxZChunk = 0;

        [Header("Chunk Dimensions")]
        /// <summary>
        /// The quantity of CoreChunkCells each direction
        /// > eg. 16^3
        /// Must be an even number!!!!
        /// </summary>
        [SerializeField] Vector3Int _coreChunkSizeInCells;
        /// <summary>
        /// The size of each CoreChunkCell (handled relative to worldspace)
        /// > eg. SubChunks that are (16,16,16) will occupy 16^3 Unity based units (FBX?) 
        /// </summary>
        [SerializeField] Vector3Int _subChunkSize;
        /// <summary>
        /// Needed for calcs later b/c working with int and it is easier to just store this for now
        /// </summary>
        private Vector3Int _coreChunkDimensions;

        [Header("Reference Tiles")]
        [SerializeField] TesseraTileBase _voidTilePlaceholder;
        [SerializeField] TesseraTileBase _voidPinnedTile;
        [SerializeField] TesseraTileBase _anchorTileBase;
        [SerializeField] TesseraPinned _pathAnchorPinned;

        [Header("Objects")]
        [SerializeField] GameObject _voidTile;
        [SerializeField] GameObject _pathTile;
        [SerializeField] GameObject _pathDebugObj;
        [SerializeField] GameObject[] _knotDebugObjs = new GameObject[6];
        [SerializeField] Material _debugMaterial;
        /// <summary>
        /// Generators used to get structure
        /// </summary>
        [Header("Generation")]
        [SerializeField] bool _doCollapse = true;
        [SerializeField] int _borderGenBoundry = 1;
        [SerializeField] int _branchQuantity = 3;
        [SerializeField] TesseraGenerator[] _branchGeneratorArray = new TesseraGenerator[3];

        /// <summary>    
        /// The distance a knot is offset when setting VoidTransition knot offsets
        /// </summary>
        [SerializeField] float _transitionOffset = .1f;
        /// <summary>    
        /// The distance a knot is offset when setting VoidTransition knot offsets
        /// </summary>
        [SerializeField] float _knotPerpendicularOffset = .25f;

        /// <summary>    
        /// Maximum number of tiles to instantiate objects for per update. Negative means unbounded.
        /// </summary>
        [SerializeField] int _maxGameObjectInstantiatePerUpdate = 100;

        private Sylves.CellRotation _defaultCellRotation;
        /// <summary>
        /// Number of chunks current scan intercal count currently
        /// </summary>
        private int _scanIntervalCount = 0;
        /// <summary>
        /// Number of chunks generating currently
        /// </summary>
        private int _generatingCount = 0;
        /// <summary>
        /// CoreChunk references
        /// </summary>
        private Sylves.IGrid _coreChunkGrid;
        private Dictionary<Sylves.Cell, CoreChunk> _coreChunks = new Dictionary<Sylves.Cell, CoreChunk>();

        #region Monobehavior related
        private void Start() {
            /// <summary>
            /// Store grid dimensions for later calculations
            /// > Create a Grid with a cellSize scaled to the dimensions of a CoreChunk in worldspace
            /// </summary>
            _coreChunkDimensions = Vector3Int.Scale(_coreChunkSizeInCells, _subChunkSize);
            _coreChunkGrid = new Sylves.CubeGrid(Vector3.Scale(_coreChunkSizeInCells, _subChunkSize));

            // store a ref for _defaultCellRotation for later calcs
            _coreChunkGrid.FindCell(Vector3.one, out Cell cell);
            _defaultCellRotation = _coreChunkGrid.GetCellType(cell).GetIdentity();
        }

        private void Update() {
            if (_generatingCount < _parallelism && _scanIntervalCount > _scanInterval) {
                Scan();
            } 
        }
        private void FixedUpdate() {
            _scanIntervalCount++;
        }
        #endregion

        #region Chunk Scanning Methods
        private void Scan() {
            /* TODO: add check to make sure no two neghbors are generating at the same time */
            if (_generatingCount < _parallelism) {
                var coreChunkToGenerate = GetReadyChunks().FirstOrDefault();
                if (coreChunkToGenerate == null) {
                    Debug.Log($"Scan returned CoreChunks in range are generated");
                    return;
                }
                StartCoroutine(HandleChunkGeneration(coreChunkToGenerate));
                Debug.Log($"Scan Successful {coreChunkToGenerate.gameObject.name} sent for Generation");
            }
        }
        // Recommends closest CoreChunk that has not been generated
        private IEnumerable<CoreChunk> GetReadyChunks() {
            foreach (var coreChunk in GetWatchedChunks()) {
                // Only interested in pending chunks that need to be generated
                // and memoized chunks (which we can re-instantiate)
                if (coreChunk.status != CoreChunkStatus.Pending) continue;

                // Two adjacent chunks cannot be generating at the same time.
                if (_parallelism > 1) {
                    if (GetAllNeighbourChunks(coreChunk.cell).Any(c => c.status == CoreChunkStatus.Generating)) {
                        continue;
                    }
                }
                Debug.Log($"{coreChunk} with {coreChunk.status} status sent as ready chunk");
                yield return coreChunk;
            }
        }
        // Recommends pending chunks to generate, in order of priority.
        private IEnumerable<CoreChunk> GetWatchedChunks() {
            var nearChunks = new Dictionary<Sylves.Cell, float>();
            void Add(Sylves.Cell chunkCell, float distance) {
                if (nearChunks.TryGetValue(chunkCell, out var currentDistance)) {
                    if (currentDistance < distance)
                        return;
                }
                nearChunks[chunkCell] = distance;
            }
            foreach (var _playerObject in _playerObjects) {
                var playerCellLocation = _coreChunkGrid.FindCell(_playerObject.transform.position);
                var scanBounds = new Bounds((Vector3Int) playerCellLocation, Vector3.one * _scanRadius);
                var localBounds = GeometryUtils.Multiply(_playerObject.transform.worldToLocalMatrix, scanBounds);

                foreach (var chunkCell in _coreChunkGrid.GetCellsIntersectsApprox(localBounds.min, localBounds.max)) {
                    var cellCenter = _coreChunkGrid.GetCellCenter(chunkCell);
                    Add(chunkCell, (scanBounds.center - cellCenter).magnitude);
                }
            }
            return nearChunks.OrderBy(x => x.Value).Select(x => x.Key).Select(GetCoreChunkFromCell);
        }
        /// <summary>
        /// Gets the chunks adjacent to the given chunk.
        /// </summary>
        private IEnumerable<CoreChunk> GetAllNeighbourChunks(Sylves.Cell chunkCell) {
            foreach (var cellDir in _coreChunkGrid.GetCellDirs(chunkCell)) {
                if (_coreChunkGrid.TryMove(chunkCell, cellDir, out var neighbour, out var _, out var _)) {
                    yield return GetCoreChunkFromCell(neighbour);
                }
            }
        }
        private CoreChunk GetCoreChunkFromCell(Sylves.Cell coreChunkCell) {
            if (_coreChunks.TryGetValue(coreChunkCell, out var coreChunk)) {
                return coreChunk;
            } else { return ChunkInitialization(coreChunkCell); }
        }
        #endregion

        #region Generation
        private IEnumerator HandleChunkGeneration(CoreChunk coreChunk) {
            coreChunk.status = CoreChunkStatus.Generating;
            coreChunk.lastWatch = Time.unscaledTime;
            _generatingCount++;
            Debug.Log($"Started generating chunk ({coreChunk.cell})");

            // confirm chunk is set up properly for generation
            CoreChunkSetup(coreChunk);

            // set up initial constraints
            var constraintSuccess = GetInitialConstraints(coreChunk);
            yield return constraintSuccess;

            if (!constraintSuccess) { /* Setup failed */
                _generatingCount--;
                coreChunk.status = CoreChunkStatus.FailedGeneration;
                Debug.Log($"Failed to fully set up initialConstraints for {coreChunk.name} > canceling generation");
                yield break;
            }
            // SetUp fully complete > send to be generated
            yield return StartCoroutine(StartChunkGenerators(coreChunk, _branchGeneratorArray));
            // Finalize Generation
            yield return StartCoroutine(FinalizeCoreChunk(coreChunk));;

            if (_doCollapse) {
                yield return StartCoroutine(ManageCollapse(coreChunk));
            }

            coreChunk.status = CoreChunkStatus.Generated;
            _generatingCount--;
            Debug.Log($"{coreChunk.name} with {coreChunk.status} status Generation Finalized");
        }

        /// <summary>
        /// Called when scanning -> sets up the Gameobject, Name, Status, Grid, CornerPositions, and is added to the _coreChunks dictionary
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private CoreChunk ChunkInitialization(Sylves.Cell cell) {
            var inBounds = (infiniteX || (minXChunk <= cell.x && cell.x <= maxXChunk)) && (infiniteY || (minYChunk <= cell.y && cell.y <= maxYChunk)) && (infiniteZ || (minZChunk <= cell.z && cell.z <= maxZChunk));

            CoreChunk coreChunk = new CoreChunk {
                name = $"CoreChunk {cell}",
                status = inBounds ? CoreChunkStatus.Pending : CoreChunkStatus.OutOfBounds,
                cell = cell,
                gameObject = inBounds ? new GameObject($"CoreChunk {cell}") : null,
                grid = new Sylves.CubeGrid(Vector3.one),
                gridTransform = new TRS((Vector3Int)cell, Quaternion.identity, Vector3Int.one),
            };
            if (coreChunk.status != CoreChunkStatus.OutOfBounds) {
                coreChunk.gameObject.transform.parent = transform;
                coreChunk.gameObject.transform.localRotation = Quaternion.identity;
                coreChunk.gameObject.transform.localScale = Vector3.one;
                coreChunk.gameObject.transform.position = Vector3.Scale(_coreChunkDimensions, new Vector3(cell.x + .5f, cell.y + .5f, cell.z + .5f));
                coreChunk.gridTransform = new TRS(coreChunk.gameObject.transform.position);
                _coreChunks.Add(cell, coreChunk);
            }
            return coreChunk;
        }
        /// <summary>
        /// called before initial constraints estabolished
        /// > sets the ConstraintBuilder, ConstraintDictionary, & AnchorDictionary if they are null
        /// </summary>
        /// <param name="coreChunk"></param>
        private void CoreChunkSetup(CoreChunk coreChunk) {
            // make sure the current chunk has an ConstraintBuilder
            if (coreChunk.constraintBuilder == null) {
                coreChunk.constraintBuilder = new TesseraInitialConstraintBuilder(coreChunk.gameObject.transform, coreChunk.grid);
            }
            if (coreChunk.anchorConstraintDictionary == null) {
                coreChunk.anchorConstraintDictionary = new Dictionary<CubeDir, List<ITesseraInitialConstraint>>();
            }
            // make sure the current chunk has an AnchorDictionary
            if (coreChunk.anchorDictionary == null) {
                coreChunk.anchorDictionary = new Dictionary<CubeDir, List<Vector3Int>>();
            }
        }
        #endregion

        #region First Pass Generation
        /// <summary>
        /// Called by HandleChunkGeneration -> failure here will set status to failed & chunk will not be generated -> This failsafe is just meant for trouble shooting
        /// </summary>
        /// <param name="coreChunk">Chunk to create constraints for</param>
        /// <returns></returns>
        private bool GetInitialConstraints(CoreChunk coreChunk) {
            var success = true;
            // Check neighbors
            foreach (var cellDir in _coreChunkGrid.GetCellDirs(coreChunk.cell)) {
                List<Vector3Int> tempAnchorList = new List<Vector3Int>();
                List<ITesseraInitialConstraint> tempConstraintList = new List<ITesseraInitialConstraint>();
                if (_coreChunkGrid.TryMove(coreChunk.cell, cellDir, out var neighborCell, out var _, out var _)) {
                    // see if the neighbor cell has a chunk created for it
                    if (_coreChunks.TryGetValue(neighborCell, out var neighborCoreChunk)) {
                        if (neighborCoreChunk.anchorDictionary != null) {
                            // get the anchor locations from the neighborChunks if they have any
                            if (neighborCoreChunk.anchorDictionary.TryGetValue(((CubeDir)cellDir).Inverted(), out List<Vector3Int> anchorList)) {
                                foreach (Vector3Int v3Int in anchorList) {
                                    // offset the anchor positions
                                    Vector3Int offsetV3 = CaveGenUtils.HandleAnchorOffset((CubeDir) cellDir, _coreChunkSizeInCells, v3Int);
                                    // add the offset values to this chunks list
                                    tempAnchorList.Add(offsetV3);
                                }
                                if (tempAnchorList.Count() == 0 ) {
                                    success = false;
                                    Debug.Log($"CoreChunk {coreChunk.name} found the AnchorDictionary from {neighborCoreChunk.name}, but no values retrieved");
                                }
                            } else {
                                success = false;
                                Debug.Log($"CoreChunk {coreChunk.name} could not get the AnchorDictionary from {neighborCoreChunk.name}");
                            }
                        } else {
                            success = false;
                            Debug.Log($"{neighborCoreChunk.name}.AnchorDictionary == null");
                            break;
                        }
                    } else {
                        // make newConstraints positions for each Anchor
                        for (var i = 0; i < _branchQuantity; i++) {
                            var v3Int = CaveGenUtils.HandleNewAnchorOffset((CubeDir) cellDir, _coreChunkDimensions, _borderGenBoundry);
                            tempAnchorList.Add(v3Int);
                        }
                        if (tempAnchorList.Count() == 0) {
                            success = false;
                            Debug.Log($"CoreChunk {coreChunk.name} failed to generate random Anchors");
                        }
                    }
                    // offset each anchor and create ITesseraInitialConstrants to be used later
                    foreach (Vector3Int localCellPosition in tempAnchorList) {
                        coreChunk.grid.Unbounded.FindCell(localCellPosition, out Cell cell);

                        // use the pinned VoidTile & Matrix4x4 to create a new "ITesseraInitialConstraint" to pass to generators later
                        ITesseraInitialConstraint newConstraint = coreChunk.constraintBuilder.GetInitialConstraint(_anchorTileBase, cell, _defaultCellRotation);

                        // add the constraint to the tempConstraint list
                        tempConstraintList.Add(newConstraint);
                    }
                    // after the loop, add the lists to their respective dictionay, keyed by the correct direction
                    coreChunk.anchorDictionary.Add((CubeDir)cellDir, tempAnchorList);
                    coreChunk.anchorConstraintDictionary.Add((CubeDir)cellDir, tempConstraintList);
                } else {
                    success = false;
                    Debug.Log($"Failed to move {coreChunk.name} {cellDir} during neighbor check");
                    break;
                }
            }
            return success;
        }

        /// <summary>
        /// Carry out the actual generation -> generators called from within here
        /// </summary>
        /// <param name="coreChunk"></param>
        private IEnumerator StartChunkGenerators(CoreChunk coreChunk, TesseraGenerator[] genArray) {
            // get the two sets of branch constraints
            ITesseraInitialConstraint[] icArray1 = new ITesseraInitialConstraint[_branchQuantity];
            ITesseraInitialConstraint[] icArray2 = new ITesseraInitialConstraint[_branchQuantity];
            var genQuantity = genArray.Length;
            // handle branch generation
            for (var currentIndex = 0; currentIndex < genQuantity; currentIndex++)
                {
                // set the directions for the generator to get the right constraints
                // x axis branches -> if (i == 0), etc
                CubeDir[] cubeDirections = new CubeDir[2]; 
                if (currentIndex == 0){
                    cubeDirections[0] = CubeDir.Left; cubeDirections[1] = CubeDir.Right;
                } else if (currentIndex == 1) {
                    cubeDirections[0] = CubeDir.Up; cubeDirections[1] = CubeDir.Down;
                } else if (currentIndex == 2) {
                    cubeDirections[0] = CubeDir.Forward; cubeDirections[1] = CubeDir.Back;
                }
                if (coreChunk.anchorConstraintDictionary.TryGetValue(cubeDirections[0], out List<ITesseraInitialConstraint> list1)) {
                    icArray1 = list1.ToArray();
                }
                if (coreChunk.anchorConstraintDictionary.TryGetValue(cubeDirections[1], out List<ITesseraInitialConstraint> list2)) {
                    icArray2 = list2.ToArray();
                }
                for (var branchesGeneratedCount = 0; branchesGeneratedCount < _branchQuantity; branchesGeneratedCount++) {
                    // get a new set of border constraints for the next branch
                    var initialConstraints = new List<ITesseraInitialConstraint> { icArray1[branchesGeneratedCount], icArray2[branchesGeneratedCount] };

                    // start a new Coroutine for the next branch
                    yield return genArray[currentIndex].StartGenerate(new TesseraGenerateOptions {
                        onComplete = c => AddCompletionData(coreChunk, c),
                        initialConstraints = initialConstraints
                    });
                }
            }
        }
        void AddCompletionData(CoreChunk coreChunk, TesseraCompletion c) {
            if (c.success) {
                coreChunk.completionList = new List<TesseraCompletion> { };
                coreChunk.completionList.Add(c);
                
                if (coreChunk.subChunkDictionary == null) {
                    coreChunk.subChunkDictionary = new Dictionary<Vector3Int, SubChunk>();          
                    foreach (KeyValuePair<Vector3Int, ModelTile> keyValuePair in c.tileData) {
                        SubChunk subChunk = new SubChunk();
                        coreChunk.subChunkDictionary.Add(keyValuePair.Key, subChunk);
                        coreChunk.grid.FindCell(keyValuePair.Key, out Cell cell);
                        subChunk.coreCell = cell;
                        /// store for off thread & position calcs later
                        subChunk.gridTransform = new TRS(keyValuePair.Key, Quaternion.identity, Vector3Int.one);
                        // not adding gameObject here
                        // adding later only if the subChunk != void
                        subChunk.status = SubChunkStatus.Pending;
                    }
                }
                foreach (KeyValuePair<Vector3Int, ModelTile> keyValuePair in c.tileData) {
                    if (keyValuePair.Value.Tile == _voidTilePlaceholder) {
                        // skip, b/c not path
                    } else {
                        coreChunk.subChunkDictionary.TryGetValue(keyValuePair.Key, out SubChunk subChunk);
                        // add to path count for subChunk
                        subChunk.pathIntersectCount = subChunk.pathIntersectCount + 1;
                        // initialize gameObject if path subChunk gameObject has not been created yet
                        if (subChunk.gameObject == null) { 
                            subChunk.gameObject = new GameObject($"SubChunk '{subChunk.coreCell}'");
                            subChunk.gameObject.transform.parent = coreChunk.gameObject.transform;
                            subChunk.gameObject.transform.localRotation = Quaternion.identity;
                            subChunk.gameObject.transform.localScale = Vector3Int.one;
                        }
                    }
                }
            } else { Debug.Log($"Failed Completion (transform == {c.gridTransform.Position}) at {c.contradictionLocation}"); }
        }
        private IEnumerator FinalizeCoreChunk(CoreChunk coreChunk) {
            int _currentInstantiateCount = _maxGameObjectInstantiatePerUpdate;

            if (_doCollapse) {
                // populate path dictionary to be used for path collapse
                if (coreChunk.paths == null) coreChunk.paths = new List<SubChunk>();
                if (coreChunk.voidTransitions == null) coreChunk.voidTransitions = new List<SubChunk>();
                // assign subChunkType & add pathSubChunks to list
                foreach (KeyValuePair<Vector3Int, SubChunk> keyValuePair in coreChunk.subChunkDictionary) {
                    if (keyValuePair.Value.pathIntersectCount != 0) {
                        coreChunk.paths.Add(keyValuePair.Value);
                        keyValuePair.Value.type = SubChunkType.PathPath;
                    } else if (keyValuePair.Value.pathIntersectCount == 0) {
                        keyValuePair.Value.type = SubChunkType.VoidVoid;
                    }
                }
            } else {
                // just instantiate generic objects for debugging
                foreach (KeyValuePair<Vector3Int, SubChunk> keyValuePair in coreChunk.subChunkDictionary) {
                    if (keyValuePair.Value.pathIntersectCount != 0) {
                        // skip in future & do in a second pass
                        Instantiate(_pathTile, keyValuePair.Value.gridTransform.Position + coreChunk.gridTransform.Position, keyValuePair.Value.gridTransform.Rotation, keyValuePair.Value.gameObject.transform);
                    } else if (keyValuePair.Value.pathIntersectCount == 0) {
                        Instantiate(_voidTile, keyValuePair.Value.gridTransform.Position + coreChunk.gridTransform.Position, keyValuePair.Value.gridTransform.Rotation, keyValuePair.Value.gameObject.transform);
                    }
                    _currentInstantiateCount--;
                    if (_currentInstantiateCount <= 0) {
                        _currentInstantiateCount = _maxGameObjectInstantiatePerUpdate;
                        yield return null;
                    }
                }
            }
        }
        #endregion

        #region Collapse 
        private IEnumerator ManageCollapse(CoreChunk coreChunk) {
            // start collapse
            yield return StartCoroutine(FirstPathCollapse(coreChunk));
            // start instantiation
            yield return StartCoroutine(TemporaryPathVertInstantiation(coreChunk));
        }
        private IEnumerator FirstPathCollapse(CoreChunk coreChunk) {
            ///
            /// TODO:
            /// add in a corner check for all paths
            /// check all 8 corners and the 3 adjacent neighbors
            /// if neighbors are all void
            /// pull corner verts inwards to smooth corners
            /// 
            //iterate through pathlist
            for (int i = 0; i < coreChunk.paths.Count; i++) {
                // get the next subchunk to iterate
                SubChunk subChunk = coreChunk.paths[i];

                // do this now so the subchunk updates the other chunks 
                // with the correct status when this iteration is complete
                subChunk.status = SubChunkStatus.CollapsedOnce;
                // initialize subChunks neighborArray
                subChunk.neighborArray = new SubChunk[6];

                // 6 below == # of cubeDir
                for (int dir = 0; dir < 6; dir++) {
                    if (_coreChunkGrid.TryMove(subChunk.coreCell, (CellDir)dir, out Cell neighborCell, out CellDir inverseDir, out _)) {
                        coreChunk.subChunkDictionary.TryGetValue((Vector3Int)neighborCell, out SubChunk neighborChunk);
                        // if null continue to next loop
                        // this happens on the edge of the grid outside it's bounds
                        // future, add coreChunk face check to finalize subchunks
                        if (neighborChunk == null) { continue; } 
                        if (neighborChunk.type != SubChunkType.PathPath) {
                            if (neighborChunk.status == SubChunkStatus.Pending) {
                                // first path to update this void
                                // update status & add initialize the neighbors neighborArray
                                neighborChunk.status = SubChunkStatus.NeighborUpdated;
                                neighborChunk.neighborArray = new SubChunk[6];
                            } else {
                                // add to 'directions' dictionary for final check
                                // after self neighborArray is fully populated
                                // b/c this void has another PathPath neighbor
                                // need to check and make sure there are both paths in 
                                // the edge subChunk & the neighborChunk
                                // matching the neighbors Path neighbor direction
                                coreChunk.voidTransitions.Add(neighborChunk);
                            }
                            // initialize & store subChunk knotPositions[]
                            // b/c mesh seperates void from everything
                            // VoidTransition SubChunks will adjust knots later
                            // points will be used to instantiate knots/obj when generation is complete
                            if (subChunk.vertPositions == null) {
                                // initialize dictionary
                                subChunk.vertPositions = new Dictionary<int, Vector3[]>();
                            }
                            subChunk.vertPositions.Add(dir, GetKnotDefaultPositions(dir));
                            // add self to neighbors neighborArray
                            neighborChunk.neighborArray[(int)inverseDir] = subChunk;
                        }
                        if (neighborChunk.type == SubChunkType.PathPath) {
                            // nothing for now
                            // do not need to add to array
                            // b/c the other PathPath chunk will do it when it is collapsed
                            // or it already has
                        }
                        // add neighbor to the path subChunk neighbor array
                        subChunk.neighborArray[dir] = neighborChunk;
                    } else { /* No cell found */ }
                }
            }
            /// <summary>
            /// Check the stored void cells for transitions
            /// Note: needs to come after all paths have updated their own neighborArray
            /// need to have for edge check
            /// 0L, 1R, 2U, 3D, 4F, 5B
            /// </summary>

            /// iterate through voidChunk list
            // do edge chack between all pathNeighbors to each voidChunk
            // if void transition validated >> adjust all relavent neighbor knots
            for (int voidChunkIndex = 0; voidChunkIndex < coreChunk.voidTransitions.Count; voidChunkIndex++) {
                // store locally for now // remove later // just using for clarity
                var voidChunk = coreChunk.voidTransitions[voidChunkIndex];
                var voidNeighborArray = coreChunk.voidTransitions[voidChunkIndex].neighborArray;

                // validate which path neighbors before the edge check
                var neighborPairs = GetPossibleNeighborPairs(voidNeighborArray);
                neighborPairs = ValidateNeighborPairs(neighborPairs);

                // process valid neghbor pairs
                // check for valid transitions
                // adjust valid transition knot positions
                foreach (int[] validPair in neighborPairs) {
                    // dir for first subChunk in valid pair
                    // note: only need one of the chunks,
                    // b/c if validity is mutual
                    if (CaveGenUtils.TestIfVoidType(validPair[0], voidNeighborArray[validPair[1]])) {
                        // valid transition
                        if (voidChunk.type == SubChunkType.VoidVoid) {
                            // set to VoidTransition if VoidVoid
                            voidChunk.type = SubChunkType.VoidTransition;
                        }
                        // adjust both pathChunks knot positions
                        // move knots in inverse direction of path neighbor (towards voidChunk)
                        // select knot pair in dir of other validPair chunk from void chunk (closer to shared edge by path chunks)
                        // pass reference to path chunk (method will update the knots with the offset)
                        var neighborPath0 = voidNeighborArray[validPair[0]];
                        var neighborPath1 = voidNeighborArray[validPair[1]];

                        HandleSettingKnotPos(CaveGenUtils.InvertCubeDirInt(validPair[0]), validPair[1], neighborPath0);
                        HandleSettingKnotPos(CaveGenUtils.InvertCubeDirInt(validPair[1]), validPair[0], neighborPath1);
                    } 
                }
            }
            // called from above
            void HandleSettingKnotPos(int moveDir, int knotSelectionDirection, SubChunk pathChunk) {
                int[] knotsIndex = GetKnotPairIndex(moveDir, knotSelectionDirection);
                pathChunk.vertPositions[moveDir][knotsIndex[0]] = GetKnotOffset(moveDir, pathChunk.vertPositions[moveDir][knotsIndex[0]]);
                pathChunk.vertPositions[moveDir][knotsIndex[1]] = GetKnotOffset(moveDir, pathChunk.vertPositions[moveDir][knotsIndex[1]]);
            }

            var originalCount = coreChunk.voidTransitions.Count;
            for (int index = originalCount - 1; index > 0; index--) {
                // make sure no VoidVoid chunks are left on the coreChunks voidTransitions list
                if (coreChunk.voidTransitions[index].type != SubChunkType.VoidTransition) {
                    // no pathNeighbors had vaid transitions
                    // remove from void transition list
                    coreChunk.voidTransitions.Remove(coreChunk.voidTransitions[index]);
                } else if (coreChunk.voidTransitions[index].type != SubChunkType.VoidVoid) {
                    // nothing for now
                }
            }
            /// In theory all the positions for each knot have been made on the border of every path to air tile
            /// all the knots should have the correct face/transition position assigned
            yield return null;
        }
        /// <summary>
        /// all the functions for the above check are below
        /// </summary>
        #region Collapse Methods
        private Vector3[] GetKnotDefaultPositions(int dir) {
            // initialize array
            var knotPos = new Vector3[4];

            var xCellFaceOffset = _subChunkSize.x * .5f;
            var yCellFaceOffset = _subChunkSize.y * .5f;   
            var zCellFaceOffset = _subChunkSize.z * .5f;

            var xPerpOffset = xCellFaceOffset - _knotPerpendicularOffset;
            var yPerpOffset = yCellFaceOffset - _knotPerpendicularOffset;
            var zPerpOffset = zCellFaceOffset - _knotPerpendicularOffset;
            // get cell face offset from center of cell
            switch (dir) {
                case 0: // left
                    var axisOffset = -(xCellFaceOffset - _transitionOffset);
                    knotPos[0] = new Vector3(axisOffset, yPerpOffset,-zPerpOffset);
                    knotPos[1] = new Vector3(axisOffset, yPerpOffset, zPerpOffset);
                    knotPos[2] = new Vector3(axisOffset,-yPerpOffset,-zPerpOffset);
                    knotPos[3] = new Vector3(axisOffset,-yPerpOffset, zPerpOffset);
                    break;
                case 1: // right
                    axisOffset = xCellFaceOffset - _transitionOffset;
                    knotPos[0] = new Vector3(axisOffset, yPerpOffset, zPerpOffset);
                    knotPos[1] = new Vector3(axisOffset, yPerpOffset,-zPerpOffset);
                    knotPos[2] = new Vector3(axisOffset,-yPerpOffset, zPerpOffset);
                    knotPos[3] = new Vector3(axisOffset,-yPerpOffset,-zPerpOffset);
                    break;
                case 2: // up (think back is "up", aka look up when facing forward)
                    axisOffset = yCellFaceOffset - _transitionOffset;
                    knotPos[0] = new Vector3(-xPerpOffset, axisOffset, -zPerpOffset);
                    knotPos[1] = new Vector3( xPerpOffset, axisOffset, -zPerpOffset);
                    knotPos[2] = new Vector3(-xPerpOffset, axisOffset, zPerpOffset);
                    knotPos[3] = new Vector3( xPerpOffset, axisOffset, zPerpOffset);
                    break;
                case 3: // down (think forward is "up", aka down when facing forward)
                    axisOffset = -(yCellFaceOffset - _transitionOffset);
                    knotPos[0] = new Vector3(-xPerpOffset, axisOffset, zPerpOffset);
                    knotPos[1] = new Vector3( xPerpOffset, axisOffset, zPerpOffset);
                    knotPos[2] = new Vector3(-xPerpOffset, axisOffset,-zPerpOffset);
                    knotPos[3] = new Vector3( xPerpOffset, axisOffset,-zPerpOffset);
                    break;
                case 4: // forward
                    axisOffset = zCellFaceOffset - _transitionOffset;
                    knotPos[0] = new Vector3(-xPerpOffset, yPerpOffset, axisOffset);
                    knotPos[1] = new Vector3( xPerpOffset, yPerpOffset, axisOffset);
                    knotPos[2] = new Vector3(-xPerpOffset, -yPerpOffset, axisOffset);
                    knotPos[3] = new Vector3( xPerpOffset, -yPerpOffset, axisOffset);
                    break;
                case 5: // back
                    axisOffset = -(zCellFaceOffset - _transitionOffset);
                    knotPos[0] = new Vector3( xPerpOffset, yPerpOffset, axisOffset);
                    knotPos[1] = new Vector3(-xPerpOffset, yPerpOffset, axisOffset);
                    knotPos[2] = new Vector3( xPerpOffset,-yPerpOffset, axisOffset);
                    knotPos[3] = new Vector3(-xPerpOffset,-yPerpOffset, axisOffset);
                    break;
            }
            // return array to be added to subchunk
            return knotPos;
        }
        private List<int[]> GetPossibleNeighborPairs(SubChunk[] neighborArray) {
            var neighborPairs = new List<int[]>();
            var confirmedPaths = new List<int>();
            for (int index = 0; index < neighborArray.Length; index++) {
                // continue if null (no path neghbor)
                if (neighborArray[index] == null) continue;
                // add all possible neighbor combinations to list
                for (int path = 0; path < confirmedPaths.Count; path++) {
                    if (!neighborPairs.Contains(new int[] { confirmedPaths[path], index })) {
                        if (!neighborPairs.Contains(new int[] { index, confirmedPaths[path] })) {
                            neighborPairs.Add(new int[] { confirmedPaths[path], index });
                        }
                    }
                }
                // add the curent path index to the list
                confirmedPaths.Add(index); 
            }
            // return possible neighborPairs
            return neighborPairs;
        }
        private List<int[]> ValidateNeighborPairs(List<int[]> neighborPairs) {
            // initialize list for valid pairs
            var validPairs = new List<int[]>();
            for (int index = 0; index < neighborPairs.Count; index++) {
                var intArray = neighborPairs[index];
                switch (intArray[0]) {
                    case 0: // left
                        if (intArray[1] == 2 || intArray[1] == 3 || intArray[1] == 4 || intArray[1] == 5) {
                            validPairs.Add(intArray);
                        }
                        break;
                    case 1: // right
                        if (intArray[1] == 2 || intArray[1] == 3 || intArray[1] == 4 || intArray[1] == 5) {
                            validPairs.Add(intArray);
                        }
                        break;
                    case 2: // up
                        if (intArray[1] == 0 || intArray[1] == 1 || intArray[1] == 4 || intArray[1] == 5) {
                            validPairs.Add(intArray);
                        }
                        break;
                    case 3: // down
                        if (intArray[1] == 0 || intArray[1] == 1 || intArray[1] == 4 || intArray[1] == 5) {
                            validPairs.Add(intArray);
                        }
                        break;
                    case 4: // forward
                        if (intArray[1] == 0 || intArray[1] == 1 || intArray[1] == 2 || intArray[1] == 3) {
                            validPairs.Add(intArray);
                        }
                        break;
                    case 5: // back
                        if (intArray[1] == 0 || intArray[1] == 1 || intArray[1] == 2 || intArray[1] == 3) {
                            validPairs.Add(intArray);
                        }
                        break;
                }
            }
            // list should be only pairs that are valid transitions
            return validPairs;
        }

        private int[] GetKnotPairIndex(int knotDirection, int knotSelectionDir) {
            // use the knotPositions dictionary
            // get array by direction
            // move knots based on same dir
            // knots are drawn in a Z so vertex points are easier to assign later with proper normals
            // upper left == 0, upper right == 1, lower left == 2, lower right == 3
            void HandleLocalDebug(int stage) {
                Debug.Log($"Failed knot selection stage {stage} in the {(CubeDir)knotSelectionDir} direction");
            }
            if (knotSelectionDir == 2 || knotSelectionDir == 3) {
                // up or down are always the same pairs regardless of moveDir
                // can skip switch and return knotPairIndex
                if (knotSelectionDir == 2) {
                    // move up knots in moveDir
                    return new int[] { 0, 1 };
                } else {
                    // move down knots in moveDir
                    return new int[] { 2, 3 };
                }
            }
            switch (knotDirection) {
                case (0): // move left knots 
                    switch (knotSelectionDir) {
                        case (4): // move forward knots left
                            return new int[] { 1, 3 };
                        case (5): // move back knots left
                            return new int[] { 0, 2 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
                case (1):// move knots right
                    switch (knotSelectionDir) {
                        case (4): // move forward knots right
                            return new int[] { 0, 2 };
                        case (5): // move back knots right
                            return new int[] { 1, 3 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
                case (2): // move knots up
                    switch (knotSelectionDir) {
                        case (0):// move left knots up
                            return new int[] { 0, 2 };
                        case (1):// move right knots up
                            return new int[] { 1, 3 };
                        case (4):// move forward knots up ('lower' in Z formation)
                            return new int[] { 2, 3 };
                        case (5):// move back knots up ('upper' in Z formation)
                            return new int[] { 0, 1 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
                case (3):// move knots down
                    switch (knotSelectionDir) {
                        case (0):// move left knots down
                            return new int[] { 0, 2 };
                        case (1):// move right knots down
                            return new int[] { 1, 3 };
                        case (4):// move forward knots down ('upper' in Z formation)
                            return new int[] { 0, 1 };
                        case (5):// move back knots down ('lower' in Z formation)
                            return new int[] { 2, 3 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
                case (4): // move knots forward
                    switch (knotSelectionDir) {
                        case (0):// move left knots forward
                            return new int[] { 0, 2 };
                        case (1):// move right knots forward
                            return new int[] { 1, 3 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
                case (5):// move knots back
                    switch (knotSelectionDir) {
                        /// Note: they are arranged as if you are centered in the cell
                        /// then turn around and draw z
                        /// so left knots are actually in the + x dir & visa versa
                        case (0):// move left knots back
                            return new int[] { 1, 3 };
                        case (1):// move right knots back
                            return new int[] { 0, 2 };
                    }
                    HandleLocalDebug(2);
                    throw new Exception();
            }
            HandleLocalDebug(1);
            throw new Exception();
        }
        Vector3 GetKnotOffset(int offsetDirection, Vector3 knotPosition) {
            // offset knot position if the knot has not already been offset
            var xCellFaceOffset = _subChunkSize.x * .5f;
            var yCellFaceOffset = _subChunkSize.y * .5f;
            var zCellFaceOffset = _subChunkSize.z * .5f;
            switch (offsetDirection) {
                case 0:
                    var axisOffset = -(xCellFaceOffset - _transitionOffset);
                    if (knotPosition.x == axisOffset) {
                        return knotPosition + new Vector3(-_transitionOffset, 0, 0);
                    } else { return knotPosition; }
                case 1:
                    axisOffset = xCellFaceOffset - _transitionOffset;
                    if (knotPosition.x == axisOffset) {
                        return knotPosition + new Vector3(_transitionOffset, 0, 0);
                    } else { return knotPosition; }
                case 2:
                    axisOffset = yCellFaceOffset - _transitionOffset;
                    if (knotPosition.y == axisOffset) {
                        return knotPosition + new Vector3(0, _transitionOffset, 0);
                    } else { return knotPosition; }
                case 3:
                    axisOffset = -(yCellFaceOffset - _transitionOffset);
                    if (knotPosition.y == axisOffset) {
                        return knotPosition + new Vector3(0, -_transitionOffset, 0);
                    } else { return knotPosition; }
                case 4:
                    axisOffset = zCellFaceOffset - _transitionOffset;
                    if (knotPosition.z == axisOffset) {
                        return knotPosition + new Vector3(0, 0, _transitionOffset);
                    } else { return knotPosition; }
                case 5:
                    axisOffset = -(zCellFaceOffset - _transitionOffset);
                    if (knotPosition.z == axisOffset) {
                        return knotPosition + new Vector3(0, 0, -_transitionOffset);
                    } else { return knotPosition; }
            }
            Debug.Log($"Failed to get offset knots in the {(CubeDir)offsetDirection} direction");
            return knotPosition;
        }
        #endregion
        #endregion

        #region Instantiation
        private IEnumerator TemporaryPathVertInstantiation(CoreChunk coreChunk) {
            int _currentInstantiateCount = _maxGameObjectInstantiatePerUpdate;
            // instantiate _knotDebugVisual for each knot != null
            // & instantiate _pathDebugVisual in each path subChunk
            var pathCount = coreChunk.paths.Count;
            for (int pathsInstantiated = 0; pathsInstantiated < pathCount; pathsInstantiated++) {
                var subChunk = coreChunk.paths[pathsInstantiated];
                // instantiate center cell pathObj
                Instantiate(_pathDebugObj, subChunk.gridTransform.Position + coreChunk.gridTransform.Position, subChunk.gridTransform.Rotation, subChunk.gameObject.transform);
                _currentInstantiateCount--;

                if (subChunk.vertPositions == null) {
                    // this happens if a path is fully surrounded by path cells
                    // no knot positions are every added
                    // skip here so no errors are thrown
                    continue;
                }
                // iterate through all knots
                for (int dir = 0; dir < 6; dir++) {
                    if (subChunk.vertPositions.ContainsKey(dir)) {
                        // place all 4 knots at the previously calculated & stored world position
                        for (int knotsInstantiated = 0; knotsInstantiated < 4; knotsInstantiated++) {
                            Instantiate(_knotDebugObjs[dir], subChunk.vertPositions[dir][knotsInstantiated] + subChunk.gridTransform.Position + coreChunk.gridTransform.Position, subChunk.gridTransform.Rotation, subChunk.gameObject.transform);
                            _currentInstantiateCount--;
                        }
                    }
                }
                if (_currentInstantiateCount <= 0) {
                    _currentInstantiateCount = _maxGameObjectInstantiatePerUpdate;
                    yield return null;
                }
            }
        }
        #endregion
    }
}
