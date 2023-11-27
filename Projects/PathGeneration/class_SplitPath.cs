using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame.PathWalk {
    internal class SplitPath : IPath {
        #region Variables
        private PathType _pathType;
        private CellDir _direction;
        private string _name;
        private GameObject _gameObject;
        private GameObject _parentObject;
        private MainPath _parentPath;
        private Vector3Int _worldPosition;
        private Vector3Int _localPosition;
        private List<CellDir> _pastMoveDirections;
        private int _transverseTally;
        private int _step;

        #region Getters & Setters
        internal PathType PathType { get { return _pathType; } set { _pathType = value; } }
        internal CellDir Direction { get { return _direction; } set { _direction = value; } }
        internal string Name { get { return _name; } set { _name = value; } }
        internal GameObject GameObject { get { return _gameObject; } set { _gameObject = value; } }
        internal GameObject ParentObject { get { return _parentObject; } set { _parentObject = value; } }
        internal MainPath ParentPath { get { return _parentPath; } set { _parentPath = value; } }
        internal Vector3Int WorldPosition { get { return _worldPosition; } set { _worldPosition = value; } }
        /// <summary>
        /// Relative to the Main Path Directional Position, not the Main Path CorePosition
        /// </summary>
        internal Vector3Int LocalPosition { get { return _localPosition; } set { _localPosition = value; } }
        internal List<CellDir> PastMoveDirections { get { return _pastMoveDirections; } set { _pastMoveDirections = value; } }
        internal int TransverseTally { get { return _transverseTally; } set { _transverseTally = value; } }
        internal int Step { get { return _step; } set { _step = value; } }
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
        public void SetPathType(PathType path) { PathType = path; }
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
        internal void UpdateWorldPosition(Vector3Int newPosition) {
            _worldPosition = newPosition;
            _localPosition = GetRelativeLocalPosition(newPosition);
        }
        internal void UpdateLocalPosition(Vector3Int newPosition) {
            _localPosition = GetRelativeLocalPosition(newPosition);
            _worldPosition = newPosition + _parentPath.CorePosition;
        }
        internal Vector3Int GetRelativeLocalPosition(Vector3Int position) {

            // find the SplitPath directional position
            int splitDirectionalPosition = GetPathDirectionPosition(position);

            // find ParentPath position at the splits directional position 
            Vector3Int mainPathDirectionalCellPosition = (Vector3Int)_parentPath.DirectionalPathPositions[splitDirectionalPosition];

            // find & return transverse distance for the split path from the ParentPath DirectionalPosition
            Vector3Int splitRelativeLocalPosition = position - mainPathDirectionalCellPosition;
            return splitRelativeLocalPosition;
        }
        internal int GetPathDirectionPosition() {
            switch ((CubeDir)_direction) {
                case CubeDir.Right or CubeDir.Left: return _worldPosition.x;
                case CubeDir.Up or CubeDir.Down: return _worldPosition.y;
                case CubeDir.Forward or CubeDir.Back: return _worldPosition.z;
            }
            Debug.Log($"Failed to get SplitPath position from direction {(CubeDir)_direction}");
            throw new Exception();
        }
        internal int GetPathDirectionPosition(Vector3Int position) {
            switch ((CubeDir)_direction) {
                case CubeDir.Right or CubeDir.Left: return position.x;
                case CubeDir.Up or CubeDir.Down: return position.y;
                case CubeDir.Forward or CubeDir.Back: return position.z;
            }
            Debug.Log($"Failed to get SplitPath position from direction {(CubeDir) _direction}");
            throw new Exception();
        }
    }
}
