using UnityEngine;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame.PathWalk {
    internal enum PathType {
        Main,
        Split,
    }
    interface IPath {
        public PathType GetPathType();
        public CellDir GetDirection();
        public string GetName();
        public GameObject GetGameObject();
        public GameObject GetParentObject();
        public Vector3Int GetWorldPosition();
        public Vector3Int GetLocalPosition();
        public List<CellDir> GetPastMoveDirections();
        public int GetTransverseTally();
        public int GetStep();
        public void SetPathType(PathType pathType);
        public void SetDirection(CellDir direction);
        public void SetName(string name);
        public void SetGameObject(GameObject gameObject);
        public void SetParentObject(GameObject parentObject);
        public void SetWorldPosition(Vector3Int worldPosition);
        public void SetLocalPosition(Vector3Int localPosition);
        public void SetPastMoveDirections(List<CellDir> PastMoveDirections);
        public void SetTransverseTally(int transverseTally);
        public void SetStep(int step);
    }
}
