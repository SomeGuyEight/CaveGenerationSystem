using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame.PathWalk {
    class MainPath :IPath {
        #region Variables
        private PathType _pathType;
        private CellDir _direction;
        private string _name;
        private GameObject _gameObject;
        private GameObject _parentObject;
        private Vector3Int _corePosition;
        private Vector3Int _worldPosition;
        private Vector3Int _localPosition;
        private Dictionary<int, Cell> _directionalPathPositions;
        private Dictionary<Vector3Int, GameObject> _pathPositionObjects;
        private List<SplitPath> _splitPathsList;
        private Dictionary<Vector3Int, GameObject> _splitObjectsDictionary;
        private List<CellDir> _pastMoveDirections;
        private int _transverseTally;
        private int _step;
        private int _splitPathTally;
        private int _lastSplitChangeTally;

        #region  Getters & Setters
        internal PathType PathType { get { return _pathType; } set { _pathType = value; } }
        internal CellDir Direction { get { return _direction; } set { _direction = value; } }
        internal string Name { get { return _name; } set { _name = value; } }
        internal GameObject GameObject { get { return _gameObject; } set { _gameObject = value; } }
        internal GameObject ParentObject { get { return _parentObject; } set { _parentObject = value; } }
        internal Dictionary<int, Cell> DirectionalPathPositions { get { return _directionalPathPositions; } set { _directionalPathPositions = value; } }
        internal Dictionary<Vector3Int, GameObject> PathPositionObjects { get { return _pathPositionObjects; } set { _pathPositionObjects = value; } }
        internal List<SplitPath> SplitPathsList { get { return _splitPathsList; } set { _splitPathsList = value; } }
        internal Dictionary<Vector3Int, GameObject> SplitObjectsDictionary { get { return _splitObjectsDictionary; } set { _splitObjectsDictionary = value; } }
        internal List<CellDir> PastMoveDirections { get { return _pastMoveDirections; } set { _pastMoveDirections = value; } }
        internal Vector3Int CorePosition { get { return _corePosition; } set { _corePosition = value; } }
        internal Vector3Int WorldPosition { get { return _worldPosition; } set { _worldPosition = value; } }
        internal Vector3Int LocalPosition { get { return _localPosition; } set { _localPosition = value; } }
        internal int TransverseTally { get { return _transverseTally; } set { _transverseTally = value; } }
        internal int Step { get { return _step; } set { _step = value; } }
        internal int SplitPathTally { get { return _splitPathTally; } set { _splitPathTally = value; } }
        internal int LastSplitChangeTally { get { return _lastSplitChangeTally; } set { _lastSplitChangeTally = value; } }
        #endregion

        #region IPath Methods
        public PathType GetPathType() { return _pathType; }
        public CellDir GetDirection() { return _direction; }
        public string GetName() { return Name; }
        public GameObject GetGameObject() { return GameObject; }
        public GameObject GetParentObject() { return ParentObject; }
        public Vector3Int GetWorldPosition() { return WorldPosition; }
        public Vector3Int GetLocalPosition() { return LocalPosition; }
        public List<CellDir> GetPastMoveDirections() { return PastMoveDirections; }
        public int GetTransverseTally() { return TransverseTally; }
        public int GetStep() { return Step; }
        public void SetPathType(PathType pathType) { PathType = pathType; }
        public void SetDirection(CellDir direction) { Direction = direction; }
        public void SetName(string name) { Name = name; }
        public void SetGameObject(GameObject gameObject) { GameObject = gameObject; }
        public void SetParentObject(GameObject parentObject) { ParentObject = parentObject; }
        public void SetWorldPosition(Vector3Int worldPosition) { WorldPosition = worldPosition; }
        public void SetLocalPosition(Vector3Int localPosition) { LocalPosition = localPosition; }
        public void SetPastMoveDirections(List<CellDir> pastMoveDirections) { PastMoveDirections = pastMoveDirections; }
        public void SetTransverseTally(int transverseTally) { TransverseTally = transverseTally; }
        public void SetStep(int step) { Step = step; }
        #endregion
        #endregion

        #region Class Methods
        internal void UpdateWorldPosition(Vector3Int newPosition) {
            _worldPosition = newPosition;
            _localPosition = GetRelativeLocalPosition(newPosition - _corePosition);
        }
        internal void UpdateLocalPosition(Vector3Int newPosition) {
            _localPosition = GetRelativeLocalPosition(newPosition);
            _worldPosition = newPosition + _corePosition;
        }
        internal Vector3Int GetRelativeLocalPosition(Vector3Int position) {
            switch ((CubeDir) _direction) {
                case CubeDir.Right or CubeDir.Left: return new Vector3Int(0, position.y, position.z);
                case CubeDir.Up or CubeDir.Down: return new Vector3Int(position.x, 0, position.z);
                case CubeDir.Forward or CubeDir.Back: return new Vector3Int(position.x, position.y, 0);
            }
            Debug.Log($"Failed to get MainPath relative local position {(CubeDir) _direction}");
            throw new Exception();
        }

        /// <summary>
        /// Defaults to current Main Path World Position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal int GetPathDirectionPosition() {
            switch ((CubeDir)_direction) {
                case CubeDir.Right or CubeDir.Left: return _worldPosition.x;
                case CubeDir.Up or CubeDir.Down: return _worldPosition.y;
                case CubeDir.Forward or CubeDir.Back: return _worldPosition.z;
            }
            Debug.Log($"Failed to get MainPath position from direction {(CubeDir)_direction}");
            throw new Exception();
        }
        internal int GetPathDirectionPosition(Vector3Int position) {
            switch ((CubeDir) _direction) {
                case CubeDir.Right or CubeDir.Left: return position.x;
                case CubeDir.Up or CubeDir.Down: return position.y;
                case CubeDir.Forward or CubeDir.Back: return position.z;
            }
            Debug.Log($"Failed to get MainPath position from direction {(CubeDir)_direction}");
            throw new Exception();
        }
        internal CellDir[] GetTransverseDirections() {
            switch ((CubeDir)_direction) {
                case CubeDir.Right or CubeDir.Left: 
                    return new CellDir[] {
                        (CellDir)CubeDir.Up,
                        (CellDir)CubeDir.Down,
                        (CellDir)CubeDir.Forward,
                        (CellDir)CubeDir.Back
                    };
                case CubeDir.Up or CubeDir.Down:
                    return new CellDir[] {
                        (CellDir)CubeDir.Right,
                        (CellDir)CubeDir.Left,
                        (CellDir)CubeDir.Forward,
                        (CellDir)CubeDir.Back
                    };
                case CubeDir.Forward or CubeDir.Back:
                    return new CellDir[] {
                        (CellDir)CubeDir.Right,
                        (CellDir)CubeDir.Left,
                        (CellDir)CubeDir.Up,
                        (CellDir)CubeDir.Down
                    };
            }
            Debug.Log($"Failed to get Transverse Directions for Main Path from direction {(CubeDir) _direction}");
            throw new Exception();
        }
        #endregion
    }
}
