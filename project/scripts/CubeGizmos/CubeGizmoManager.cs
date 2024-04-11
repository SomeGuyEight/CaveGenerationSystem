using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace SlimeGame
{
    /// <summary>
    /// WIP unimplemented calss:
    /// <br/> -> Potential use to limit components in scene &amp; the number of <see cref="OnDrawGizmos"/> called each update
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public class CubeGizmoManager : SerializedMonoBehaviour
    {
        private HashSet<CubeGizmo> _registeredGizmos = new ();
        private HashSet<CubeGizmo> _enabledGizmos = new ();

        void OnDrawGizmos()
        {
            foreach (var gizmo in _enabledGizmos)
            {
                if (gizmo != null)
                {
                    Gizmos.color = gizmo.Color;
                    Gizmos.DrawWireCube(gizmo.Bounds.center,gizmo.Bounds.size);
                }
            }
        }

        public bool TryRegisterGizmosInChildren(GameObject go,bool isGizmosEnabled)
        {
            if (go == null)
            {
                return false;
            }
            var gizmos = go.GetComponents<CubeGizmo>();
            if (gizmos != null && gizmos.Length > 0)
            {
                AddGizmos();
            }
            foreach (var transform in go.GetComponentsInChildren<Transform>())
            {
                gizmos = transform.gameObject.GetComponents<CubeGizmo>();
                if (gizmos != null && gizmos.Length > 0)
                {
                    AddGizmos();
                }
            }
            return true;

            void AddGizmos()
            {
                for (int i = 0;i < gizmos.Length;i++)
                {
                    TryRegisterGizmo(gizmos[i],isGizmosEnabled);
                }
            }
        }
        public bool TryRegisterGizmo(CubeGizmo gizmo,bool isGizmoEnabled)
        {
            if (gizmo != null)
            {
                _registeredGizmos.Add(gizmo);
                if (isGizmoEnabled)
                {
                    _enabledGizmos.Add(gizmo);
                }
                return true;
            }
            return false;
        }

        public bool TrySetGizmosInChildrenEnabled(GameObject go,bool isGizmosEnabled)
        {
            if (go == null)
            {
                return false;
            }
            var gizmos = go.GetComponents<CubeGizmo>();
            if (gizmos != null && gizmos.Length > 0)
            {
                TrySetGizmosInArrayEnabled();
            }
            foreach (var transform in go.GetComponentsInChildren<Transform>())
            {
                gizmos = transform.gameObject.GetComponents<CubeGizmo>();
                if (gizmos != null && gizmos.Length > 0)
                {
                    TrySetGizmosInArrayEnabled();
                }
            }

            return true;

            void TrySetGizmosInArrayEnabled()
            {
                for (int i = 0;i < gizmos.Length;i++)
                {
                    TrySetGizmoEnabled(gizmos[i],isGizmosEnabled);
                }
            }
        }
        public bool TrySetGizmoEnabled(CubeGizmo gizmo,bool isGizmosEnabled)
        {
            if (gizmo == null)
            {
                return false;
            }

            if (!_registeredGizmos.Contains(gizmo))
            {
                _registeredGizmos.Add(gizmo);
            }
            if (isGizmosEnabled)
            {
                return _enabledGizmos.Add(gizmo);
            }
            else
            {
                return _enabledGizmos.Remove(gizmo);
            }
        }

        public bool TryUnregisterGizmosInChildren(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            var gizmos = go.GetComponents<CubeGizmo>();
            if (gizmos != null && gizmos.Length > 0)
            {
                RemoveGizmos();
            }
            foreach (var transform in go.GetComponentsInChildren<Transform>())
            {
                gizmos = transform.gameObject.GetComponents<CubeGizmo>();
                if (gizmos != null && gizmos.Length > 0)
                {
                    RemoveGizmos();
                }
            }
            return true;

            void RemoveGizmos()
            {
                for (int i = 0;i < gizmos.Length;i++)
                {
                    TryUnregisterGizmo(gizmos[i]);
                }
            }
        }
        public bool TryUnregisterGizmo(CubeGizmo gizmo)
        {
            if (gizmo != null)
            {
                return _registeredGizmos.Remove(gizmo);
            }
            return false;
        }

        private void ClearNullGizmosRegistered()
        {
            foreach (var gizmo in _registeredGizmos)
            {
                if (gizmo == null)
                {
                    _registeredGizmos.Remove(gizmo);
                }
            }
        }
        private void ClearNullGizmosEnabled()
        {
            foreach (var gizmo in _enabledGizmos)
            {
                if (gizmo == null)
                {
                    _enabledGizmos.Remove(gizmo);
                }
            }
        }
    }
}