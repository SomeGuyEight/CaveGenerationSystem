using UnityEngine;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame.PathWalk {
    public class PathWalkManager : MonoBehaviour {

        #region Variables
        [Header("Grid")]
        /// <summary>
        /// Original Size was 1,1,1
        /// </summary>
        [SerializeField] Vector3Int _gridCellSize;
        CubeGrid _grid;

        [Header("Main Path Walk Parameters")]
        /// <summary>
        /// Original Range was 16,16,16
        /// </summary>
        [SerializeField] Vector3Int _mainTransverseWalkRange;

        [Header("Main Weights")]
        [SerializeField] int _mainMaxWeightPerDirection = 50;
        [SerializeField] int _mainPathDirectionWeight = 20;
        [SerializeField] int _mainTransverseTallyWeightFactor = 10;
        [SerializeField] int _mainPastMoveWeightFactor = 4;
        [SerializeField] int _mainOtherDirWeightFactor = 4;
        [SerializeField] int _mainMaxTransverseMoveTally = 2;
        [SerializeField] int _mainMaxPastMovesWeighted = 4;
        [SerializeField] int _mainMaxPastMoveDirectionsStored = 4;
        /// <summary>
        /// Original ints were 6,5,4,3,2 
        /// </summary>
        [SerializeField] int[] _mainPastMoveWeightsByIndex = new int[5];

        [Header("Split Path Walk Parameters")]
        /// <summary>
        /// Original Range was 16,16,16
        /// </summary>
        [SerializeField] Vector3Int _splitTransverseWalkRange;
        [SerializeField] int _maxSplits = 2;
        [SerializeField] int _newSplitWeightFactor = 10;
        [SerializeField] int _splitTestWeightTotal = 100;
        [SerializeField] int _maxSplitTries = 4;

        [Header("Split Weights")]
        [SerializeField] int _splitMaxWeightPerDirection = 50;
        [SerializeField] int _splitPathDirectionWeight = 20;
        [SerializeField] int _splitTransverseTallyWeightFactor = 10;
        [SerializeField] int _splitPastMoveWeightFactor = 4;
        [SerializeField] int _splitOtherDirWeightFactor = 4;
        [SerializeField] int _splitMaxTransverseMoveTally = 2;
        [SerializeField] int _splitMaxPastMovesWeighted = 4;
        [SerializeField] int _splitMaxPastMoveDirectionsStored = 4;
        /// <summary>
        /// Original ints were 6,5,4,3,2 
        /// </summary>
        [SerializeField] int[] _splitPastMoveWeightsByIndex = new int[5];

        [Header("Generation")]
        [SerializeField] int _maxStepsTotal = 2000;
        [SerializeField] int _maxStepsPerUpdate = 2;
        [SerializeField] bool _doMultiplePaths = false;
        private List<MainPath> _paths = new List<MainPath>();
        GameObject _generationHolder;
        private bool _walking = false;

        [Header("Visuals")]
        [SerializeField] GameObject _mainPathVisual;
        [SerializeField] GameObject _splitPathVisual;
        MeshFilter _mainPathMeshFilter;
        MeshFilter _splitPathMeshFilter;
        Material _mainPathMaterial;
        Material _splitPathMaterial;
        #endregion

        #region Awake, Start, Update, FixedUpdate
        private void Awake() {
            PathVisualsSetUp();
            GenerationHolderSetUp();
            GridSetUp();

            /// Temp: Initialize a path for generation to start with
            InitializeMainPath((CellDir)CubeDir.Right);

        }
        private void Update() {
            HandleWalkingThisFrame();
        }
        #endregion

        #region Set Up
        private void PathVisualsSetUp() {
            if (_mainPathVisual.GetComponent<MeshFilter>() != null) {
                _mainPathMeshFilter = _mainPathVisual.GetComponent<MeshFilter>();
            } else {
                Debug.Log($"Main Path Object did not have a Mesh Filter => _pathMeshFilter not set");
            }
            if (_mainPathVisual.GetComponent<MeshRenderer>().material != null) {
                _mainPathMaterial = _mainPathVisual.GetComponent<MeshRenderer>().material;
            } else {
                Debug.Log($"Main Path Object did not have a Mesh Renderer => _pathMeshRenderer not set");
            }
            if (_splitPathVisual.GetComponent<MeshFilter>() != null) {
                _splitPathMeshFilter = _splitPathVisual.GetComponent<MeshFilter>();
            } else {
                Debug.Log($"Split Path Object did not have a Mesh Filter => _pathMeshFilter not set");
            }
            if (_splitPathVisual.GetComponent<MeshRenderer>().material != null) {
                _splitPathMaterial = _splitPathVisual.GetComponent<MeshRenderer>().material;
            } else {
                Debug.Log($"Split Path Object did not have a Mesh Renderer => _pathMeshRenderer not set");
            }
        }
        private void GenerationHolderSetUp() {
            _generationHolder = new GameObject {
                name = "Generation Holder"
            };
            _generationHolder.transform.position = Vector3.zero;
        }
        private void GridSetUp() {
            _grid = new CubeGrid(_gridCellSize);
        }
        #endregion

        #region Initialization
        private MainPath InitializeMainPath(CellDir dir) {
            
            /// TODO: Adapt to use IPath methods
            /// 

            /// TODO: find Y & Z start point & set for newCorePosition
            /// 

            /// Temp: core position set
            var newCorePosition = Vector3Int.zero;

            var newPath = new MainPath {

                /// TODO: Get Core Path based on Direction or passed in int[2]
                /// 

                Name = $"{(CubeDir) dir} & {GenerationUtils.InvertDirection((CubeDir) dir)} Path | Core ({newCorePosition.y}, {newCorePosition.z})",
                PathType = PathType.Main,
                Direction = dir,
                CorePosition = newCorePosition,
                WorldPosition = newCorePosition,
                LocalPosition = newCorePosition - newCorePosition,
                PathPositionObjects = new Dictionary<Vector3Int, GameObject>(),
                DirectionalPathPositions = new Dictionary<int, Cell>(),
                SplitPathsList = new List<SplitPath>(),
                SplitObjectsDictionary = new Dictionary<Vector3Int, GameObject>(),
                PastMoveDirections = new List<CellDir>(),
                Step = 0,
                SplitPathTally = 0,
                LastSplitChangeTally = 0,
            };
            newPath.ParentObject = _generationHolder.gameObject;
            newPath.GameObject = CreatePathObj(newPath);
            _paths.Add(newPath);

            /// TODO: Pick a random transverse starting location for the '0 Step' position
            /// 

            // need to create the '0 Step' Split Path Object for walk to start from
            newPath.PathPositionObjects[newCorePosition] = HandleObjectCreation((Cell) newCorePosition, newPath);
            newPath.DirectionalPathPositions[newPath.GetPathDirectionPosition()] = (Cell)newPath.WorldPosition;
            return newPath;
        }
        private (CellDir, SplitPath) InitializeSplitPath(MainPath mainPath, Cell cell) {
            mainPath.SplitPathTally++;
            var newSplitPath = new SplitPath {
                Name = $"Split Path {mainPath.SplitPathTally} from '{mainPath.Name}'",
                ParentObject = _generationHolder.gameObject,
                ParentPath = mainPath,
                PathType = PathType.Split,
                Direction = mainPath.Direction,
                PastMoveDirections = new List<CellDir>(),
                TransverseTally = 0,
                Step = 0,
            };
            newSplitPath.UpdateWorldPosition((Vector3Int)cell);
            newSplitPath.GameObject = CreatePathObj(newSplitPath);
            mainPath.SplitPathsList.Add(newSplitPath);
            // need to create the '0 Step' Split Path Object for walk to start from
            mainPath.SplitObjectsDictionary[newSplitPath.WorldPosition] = HandleObjectCreation(cell, newSplitPath);
            return (mainPath.Direction, newSplitPath);
        }
        #endregion

        #region Walk
        private void HandleWalkingThisFrame() {
            if (!_walking) {
                _walking = true;
                HandlePathWalk(GetPathForWalk());
                _walking = false;
                //Debug.Log($"Reached end of HandleWalking this frame");
            }
        }
        private MainPath GetPathForWalk() {
            var lastPath = _paths[_paths.Count-1];
            if (lastPath.Step >= _maxStepsTotal && _doMultiplePaths) {

                /// TODO: Search for new path to initialize
                /// 

                /// TODO: Set path dir for new path
                /// 

                lastPath = InitializeMainPath((CellDir)CubeDir.Right);
            } else if (lastPath.Step >= _maxStepsTotal) {
                Debug.Log($"Move direction is full => returning null Main Path");
                return null;
            }
            return lastPath;
        }
        private void HandlePathWalk(IPath path) {
            if (_paths.FindLast(path => path != null).Step < _maxStepsTotal) {
                if (path != null) {
                    if (path.GetPathType() == PathType.Main) {
                        MainPath mainPath = (MainPath) path;
                        MainPathLoop(mainPath);
                        Queue<SplitPath> splitRemovalQueue = new Queue<SplitPath>();
                        foreach (SplitPath splitPath in mainPath.SplitPathsList) {
                            if (SplitPathLoop(splitPath)) {
                                splitRemovalQueue.Enqueue(splitPath);
                            }
                        }
                        for (int processed = 0; processed < splitRemovalQueue.Count; processed++) {
                            mainPath.SplitPathsList.Remove(splitRemovalQueue.Dequeue());
                        }
                    } else if (path.GetPathType() == PathType.Split) {
                        SplitPath splitPath = (SplitPath) path;
                        SplitPathLoop(splitPath);
                    }
                } else {
                    Debug.Log($"Main Path returned to HandleWalking is null => Not progressing to walking");
                }
            }
        }
        /// <summary>
        /// Walks the Main Path 
        /// > Attempts to start new split path if current splits are less than max splits
        /// > Ends when the max main direction steps have happened
        /// </summary>
        /// <param name="mainPath"></param>
        private void MainPathLoop(MainPath mainPath) {
            int stepsThisWalk = _maxStepsPerUpdate;
            for (int mainDirSteps = 0; mainDirSteps < stepsThisWalk; mainDirSteps++) {
                (CubeDir?, bool) nullableStepAndUpdatedTally = PickMoveDirection(mainPath);
                if (nullableStepAndUpdatedTally.Item2 == true) {
                    mainPath.TransverseTally = 0;
                }
                Debug.Log($"Main Path TransverseTally == {mainPath.TransverseTally}");
                if (nullableStepAndUpdatedTally.Item1 == null) {
                    Debug.Log($"Move direction is null while walking Main Path => continueing to next loop");
                    break;
                }
                CellDir step = (CellDir) nullableStepAndUpdatedTally.Item1;
                if (step == mainPath.Direction) {
                    mainPath.TransverseTally = 0;
                } else {
                    mainPath.TransverseTally++;
                }
                mainPath.PastMoveDirections.Insert(0, step);
                if (mainPath.PastMoveDirections.Count > _mainMaxPastMoveDirectionsStored) {
                    mainPath.PastMoveDirections.RemoveAt(_mainMaxPastMoveDirectionsStored);
                }
                _grid.TryMove((Cell) mainPath.WorldPosition, step, out Cell newCell, out CellDir currentMoveInverseDir, out Connection connection);
                mainPath.Step++;
                mainPath.UpdateWorldPosition((Vector3Int) newCell);
                var newStepObject = HandleObjectCreation(newCell, mainPath);
                mainPath.PathPositionObjects[(Vector3Int) newCell] = newStepObject;
                int newDirectionalPosition = mainPath.GetPathDirectionPosition((Vector3Int) newCell);
                if (step != mainPath.Direction) {
                    // not in the main dir => increment maindDirSteps down
                    // - Makes sure the moves in the main dir are only taken into account
                    mainDirSteps--;
                } else if (step == mainPath.Direction && !mainPath.DirectionalPathPositions.ContainsKey(newDirectionalPosition)) {
                    mainPath.DirectionalPathPositions[newDirectionalPosition] = newCell;
                }
                MainPathSplitCheck(mainPath);
            }
        }
        /// <summary>
        /// Walks the split path until it merges with a main path or another split path
        /// </summary>
        /// <param name="splitPath"></param>
        /// <returns>if true > Enqueue split path to terminate after all splits are done walking</returns>
        private bool SplitPathLoop(SplitPath splitPath) {
            bool addToRemovalQueue = false;
            // Getting last main path position - 1 to prevent the main path from ever walking onto the split paths
            int branchStepsThisWalk = Mathf.Abs(splitPath.ParentPath.GetPathDirectionPosition() - splitPath.GetPathDirectionPosition(splitPath.WorldPosition)) - 1;
            for (int branchSplitSteps = 0; branchSplitSteps < branchStepsThisWalk; branchSplitSteps++) {

                /// TODO: add in a method that will decrease the branch range over time
                /// > will effectively limit how long a branch remains split from the main path
                /// 

                (CubeDir?, bool) nullableStepAndUpdatTallyBool = PickMoveDirection(splitPath);
                if (nullableStepAndUpdatTallyBool.Item2 == true) {
                    splitPath.TransverseTally = 0;
                }
                if (nullableStepAndUpdatTallyBool.Item1 == null) {
                    Debug.Log($"Move direction is null while walking Split Path => continueing to next loop");
                    break;
                }
                CellDir step = (CellDir) nullableStepAndUpdatTallyBool.Item1;
                if (step == splitPath.Direction) {
                    splitPath.TransverseTally = 0;
                } else {
                    splitPath.TransverseTally++;
                }
                splitPath.PastMoveDirections.Insert(0, step);
                if (splitPath.PastMoveDirections.Count > _splitMaxPastMoveDirectionsStored) {
                    splitPath.PastMoveDirections.RemoveAt(_splitMaxPastMoveDirectionsStored);
                }
                _grid.TryMove((Cell) splitPath.WorldPosition, step, out Cell newCell, out CellDir currentMoveInverseDir, out Connection connection);
                if (splitPath.ParentPath.PathPositionObjects.TryGetValue((Vector3Int) newCell, out GameObject mainPathGameObject)) {
                    // remove b/c moving into Main Path Cell
                    addToRemovalQueue = true;
                    break;
                } else if (splitPath.ParentPath.SplitObjectsDictionary.TryGetValue((Vector3Int) newCell, out GameObject contridictingSplitPath)) {
                    // remove b/c moving into other Split Path Cell
                    addToRemovalQueue = true;
                    break;
                } else {
                    splitPath.Step++;
                    splitPath.UpdateWorldPosition((Vector3Int) newCell);
                    HandleObjectCreation(newCell, splitPath);
                    if (step != splitPath.Direction) {
                        // Increment back to make sure only moves in main dir are counted this walk
                        branchSplitSteps--;
                    }
                    Debug.Log($"Split Path Transverse Tally == {splitPath.TransverseTally}");
                    //Debug.Log($"{splitPath.Name} Transverse Tally == {splitPath.TransverseTally}");
                }
            }
            return addToRemovalQueue;
        }
        private void MainPathSplitCheck(MainPath mainPath) {
            mainPath.LastSplitChangeTally++;
            if (mainPath.SplitPathsList.Count < _maxSplits) {
                int newSplitWeight = mainPath.SplitPathsList.Count * _newSplitWeightFactor;
                int testInt = UnityEngine.Random.Range(0, _splitTestWeightTotal);
                if (newSplitWeight < testInt) {
                    // Getting last main path position - 1 to prevent the main path from ever walking onto the split paths
                    int previousMainPathIndex = mainPath.GetPathDirectionPosition(mainPath.WorldPosition) - 1;
                    if (mainPath.DirectionalPathPositions.TryGetValue(previousMainPathIndex, out Cell previousMainPathCell)) {
                        CellDir[] transverseDir = mainPath.GetTransverseDirections();
                        int randomIndex = UnityEngine.Random.Range(0, transverseDir.Length);
                        CellDir splitDir = transverseDir[randomIndex];
                        var nullableCell = FindEmptyCellForSplit(mainPath, splitDir, previousMainPathCell);
                        if (nullableCell != null) {
                            var cell = (Cell) nullableCell;
                            mainPath.LastSplitChangeTally = 0;
                            InitializeSplitPath(mainPath, cell);
                            var newSplit = mainPath.SplitPathsList[mainPath.SplitPathsList.Count - 1];
                            newSplit.PastMoveDirections.Insert(0, splitDir);
                        }
                    }
                    // no valid cell was found, just move on and check for split on next Main Path step
                }
            }
        }
        private Cell? FindEmptyCellForSplit(MainPath mainPath, CellDir dir, Cell cell) {
            Cell? nullableCell = null;
            Cell currentTryCell = cell;
            for (int splitTries = 0; splitTries < _maxSplitTries; splitTries++) {
                _grid.TryMove(currentTryCell, dir, out Cell newCell, out CellDir currentMoveInverseDir, out Connection connection);
                if (!mainPath.PathPositionObjects.ContainsKey((Vector3Int) newCell)) {
                    if (!mainPath.SplitObjectsDictionary.ContainsKey((Vector3Int) newCell)) {
                        nullableCell = newCell;
                        break;
                    }
                }
            }
            return nullableCell;
        }
        private (CubeDir?, bool) PickMoveDirection(MainPath mainPath) {
            bool resetTally = false;
            if (mainPath.TransverseTally >= _mainMaxTransverseMoveTally) {
                resetTally = true;
                return ((CubeDir)mainPath.Direction, resetTally);
            }
            var directionWeights = new int[6];
            SetUpDirectionWeightsArray(ref directionWeights, mainPath, _mainTransverseWalkRange, _mainOtherDirWeightFactor, _mainMaxWeightPerDirection);
            AdjustWeightValueBasedOnPastMoves(ref directionWeights, mainPath, _mainMaxPastMovesWeighted, _mainPastMoveWeightsByIndex, _mainPastMoveWeightFactor);
            directionWeights[(int) mainPath.Direction] = Mathf.Min(_mainPathDirectionWeight + (mainPath.TransverseTally * _mainTransverseTallyWeightFactor), _mainMaxWeightPerDirection);
            directionWeights[(int)GenerationUtils.InvertDirection(mainPath.Direction)] = 0;
            // removing inverse direction of last move stops path from walking back on self
            if (mainPath.PastMoveDirections.Count > 0) {
                directionWeights[(int) GenerationUtils.InvertDirection(mainPath.PastMoveDirections[0])] = 0;
            }
            int? nullableInt = PickWeightedCountIndex(directionWeights);
            if (nullableInt == null) {
                return (null, resetTally);
            }
            return ((CubeDir)nullableInt, resetTally);
        }
        private (CubeDir?, bool) PickMoveDirection(SplitPath splitPath) {
            bool resetTally = false;
            if (splitPath.TransverseTally >= _splitMaxTransverseMoveTally) {
                resetTally = true;
                return ((CubeDir) splitPath.Direction, resetTally);
            }
            var directionWeights = new int[6];
            SetUpDirectionWeightsArray(ref directionWeights, splitPath, _splitTransverseWalkRange, _splitOtherDirWeightFactor, _splitMaxWeightPerDirection);
            AdjustWeightValueBasedOnPastMoves(ref directionWeights, splitPath, _splitMaxPastMovesWeighted, _splitPastMoveWeightsByIndex, _splitPastMoveWeightFactor);
            directionWeights[(int) splitPath.Direction] = Mathf.Min(_splitPathDirectionWeight + (splitPath.TransverseTally * _splitTransverseTallyWeightFactor), _splitMaxWeightPerDirection);
            directionWeights[(int) GenerationUtils.InvertDirection(splitPath.Direction)] = 0;
            // removing inverse direction of last move stops path from walking back on self
            if (splitPath.PastMoveDirections.Count > 0) {
                directionWeights[(int) GenerationUtils.InvertDirection(splitPath.PastMoveDirections[0])] = 0;
            }

            // Adjust weights based on distance from main path cell to favor moving towards main path
            // > use step count to increase liklihood of moving towards main path
            // >> as step count increases => more likely to move towards main path
            // if any direction is outside of Split Path Range => move towards path
            // * should not need to worry axis of path, b/c relative axis position will always be 0
            Vector3Int positionRelativeToMainPath = splitPath.GetLocalPosition();
            bool returnThisDir = false;
            var directionInt = HandlePathFunnel(positionRelativeToMainPath, _splitTransverseWalkRange, ref returnThisDir);
            if (returnThisDir) {
                return ((CubeDir) directionInt, resetTally);
            }
            int? nullableInt = PickWeightedCountIndex(directionWeights);
            if (nullableInt == null) {
                return (null, resetTally);
            }
            return ((CubeDir) nullableInt, resetTally);
        }
        private void SetUpDirectionWeightsArray(ref int[] directionWeights, IPath path, Vector3Int transverseWalkRange, int otherDirWeightFactor, int maxWeightPerDirection) {
            var localPosition = path.GetLocalPosition();
            directionWeights[(int)CubeDir.Right] = Mathf.Min((-localPosition.x + transverseWalkRange.x) * otherDirWeightFactor, maxWeightPerDirection);
            directionWeights[(int)CubeDir.Left] = Mathf.Min((localPosition.x + transverseWalkRange.x) * otherDirWeightFactor, maxWeightPerDirection);
            directionWeights[(int)CubeDir.Up] = Mathf.Min((-localPosition.y + transverseWalkRange.y) * otherDirWeightFactor, maxWeightPerDirection);
            directionWeights[(int)CubeDir.Down] = Mathf.Min((localPosition.y + transverseWalkRange.y) * otherDirWeightFactor, maxWeightPerDirection);
            directionWeights[(int)CubeDir.Forward] = Mathf.Min((-localPosition.z + transverseWalkRange.z) * otherDirWeightFactor, maxWeightPerDirection);
            directionWeights[(int)CubeDir.Back] = Mathf.Min((localPosition.z + transverseWalkRange.z) * otherDirWeightFactor, maxWeightPerDirection);
        }
        private void AdjustWeightValueBasedOnPastMoves(ref int[] directionWeights, IPath path, int maxPastMovesWeighted, int[] pastMoveWeightsByIndex, int pastMoveWeightFactor) {
            var pastMoveDir = path.GetPastMoveDirections();
            for (int index = 0; index < maxPastMovesWeighted; index++) {
                if (pastMoveDir.Count > index) {
                    switch ((CubeDir)pastMoveDir[index]) {
                        case CubeDir.Right:
                            directionWeights[(int)CubeDir.Right] = Mathf.Max(directionWeights[(int)CubeDir.Right] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                        case CubeDir.Left:
                            directionWeights[(int)CubeDir.Left] = Mathf.Max(directionWeights[(int)CubeDir.Left] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                        case CubeDir.Up:
                            directionWeights[(int)CubeDir.Up] = Mathf.Max(directionWeights[(int)CubeDir.Up] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                        case CubeDir.Down:
                            directionWeights[(int)CubeDir.Down] = Mathf.Max(directionWeights[(int)CubeDir.Down] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                        case CubeDir.Forward:
                            directionWeights[(int)CubeDir.Forward] = Mathf.Max(directionWeights[(int)CubeDir.Forward] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                        case CubeDir.Back:
                            directionWeights[(int)CubeDir.Back] = Mathf.Max(directionWeights[(int)CubeDir.Back] - (pastMoveWeightsByIndex[index] * pastMoveWeightFactor), 0); break;
                    }
                }
            }
        }
        private int? HandlePathFunnel(Vector3Int relativePosition, Vector3Int maxRange, ref bool returnThisDir) {
            if (relativePosition.x > maxRange.x) {
                Debug.Log($"+X distance too far");
                returnThisDir = true;
                return (int)CubeDir.Left;
            } else if (relativePosition.x < -maxRange.x) {
                Debug.Log($"-X distance too far");
                returnThisDir = true;
                return (int)CubeDir.Right;
            } else if (relativePosition.y > maxRange.y) {
                Debug.Log($"+Y distance too far");
                returnThisDir = true;
                return (int)CubeDir.Down;
            } else if (-relativePosition.y < -maxRange.y) {
                Debug.Log($"-Y distance too far");
                returnThisDir = true;
                return (int)CubeDir.Up;
            } else if (relativePosition.z > maxRange.z) {
                Debug.Log($"+Z distance too far");
                returnThisDir = true;
                return (int)CubeDir.Back;
            } else if (-relativePosition.z < -maxRange.z) {
                Debug.Log($"-Z distance too far");
                returnThisDir = true;
                return (int)CubeDir.Forward;
            }
            return null;
        }
        /// <summary>
        /// Pass in an array of int corresponding to another array of weighted choices
        /// </summary>
        /// <param name="arrayOfWeights"></param>
        /// <returns>Returns a random index from the array weighted relative to the passed in int[] values per index</returns>
        private int? PickWeightedCountIndex(int[] arrayOfWeights) {
            int totalWeightedCount = 0;
            for (int index = 0; index < arrayOfWeights.Length; index++) {
                totalWeightedCount = totalWeightedCount + arrayOfWeights[index];
            }
            int randomInt = UnityEngine.Random.Range(0, totalWeightedCount);
            int currentMaxExclusive = 0;
            for (int index = 0; index < arrayOfWeights.Length; index++) {
                currentMaxExclusive = currentMaxExclusive + arrayOfWeights[index];
                if (randomInt < currentMaxExclusive) {
                    return index;
                }
            }
            Debug.Log($"Random int was never included in weight check => returning null");
            return null;
        }
        #endregion
        
        #region Oject Creation
        private GameObject HandleObjectCreation(Cell cell, IPath path) {

            /// TODO: Set up enqueue system
            ///

            /// TODO: Send to proper Instantiation Method
            /// 

            /// Temp: Instantiate basic obj
            return CreateStepObject(cell, path);

        }
        private GameObject CreateStepObject(Cell cell, IPath path) {
            var type = path.GetPathType();
            var stepObj = new GameObject {
                name = $"{type} Step {path.GetStep()} | Cell {cell}",
            };
            stepObj.transform.position = (Vector3Int)cell;
            stepObj.transform.rotation = Quaternion.identity;
            stepObj.transform.parent = path.GetGameObject().transform;
            var filter = stepObj.AddComponent<MeshFilter>();
            var renderer = stepObj.AddComponent<MeshRenderer>();
            if (type == PathType.Main) {
                filter.mesh = _mainPathMeshFilter.mesh;
                renderer.material = _mainPathMaterial;
            } else if (type == PathType.Split) {
                filter.mesh = _splitPathMeshFilter.mesh;
                renderer.material = _splitPathMaterial;
            }
            return stepObj; 
        }
        private GameObject CreatePathObj(IPath path) {
            var pathObj = new GameObject {
                name = path.GetName()
            };
            pathObj.transform.position = path.GetParentObject().transform.position;
            pathObj.transform.rotation = Quaternion.identity;
            pathObj.transform.parent = path.GetParentObject().transform;
            return pathObj;
        }
        #endregion
    }
}
