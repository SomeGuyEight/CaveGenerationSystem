using UnityEngine;
using Sylves;

namespace SlimeGame
{
    public class CubeGizmoComponent : MonoBehaviour, ICubeGizmo
    {
        public CubeGizmoComponent(Color color,CubeBound cubeBound,Vector3 cellSize) 
        {
            UpdateColor(color);
            UpdateBounds(cubeBound,cellSize);
        }
        public CubeGizmoComponent(Color color,Bounds bounds) 
        {
            UpdateColor(color);
            UpdateBounds(bounds);
        }

        private Color _color;
        private Bounds _bounds;
        private bool _isEnabled;

        public Color Color { get { return _color; } }
        public Bounds Bounds { get { return _bounds; } }
        public bool IsEnabled { get { return _isEnabled; } }

        public void SetEnabled(bool isEnabled) 
        {
            if(_isEnabled != isEnabled)
            {
                _isEnabled = isEnabled;
            }
        }
        public void UpdateColor(Color color) 
        {
            if(_color != color) 
            {
                _color = color;
                _color.a = color.a;
            }
        }
        public void UpdateBounds(Bounds bounds) 
        {
            if(_bounds != bounds) 
            {
                _bounds = bounds;
            }
        }
        public void UpdateBounds(CubeBound cubeBound,Vector3 cellSize) 
        {
            var min = Vector3.Scale(cubeBound.min,cellSize);
            var size = Vector3.Scale(cubeBound.size,cellSize);
            var center = min + Vector3.Scale(size,new Vector3(.5f,.5f,.5f));
            var newBounds = new Bounds(center,size);
            _bounds = newBounds;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color;
            Gizmos.DrawWireCube(Bounds.center,Bounds.size);
        }
        void OnDrawGizmos() 
        {
            if(IsEnabled)
            {
                Gizmos.color = Color;
                Gizmos.DrawWireCube(Bounds.center,Bounds.size);
            }
        }

        public static bool TryAddCubeGizmo(GameObject gameObject,CubeBound cubeBound,Vector3 cellSize,Color color,out CubeGizmoComponent newGizmo)
        {
            if(gameObject == null)
            {
                newGizmo = null;
                return false;
            }
            var min = Vector3.Scale(cubeBound.min,cellSize);
            var size = Vector3.Scale(cubeBound.size,cellSize);
            var center = min + Vector3.Scale(size,new Vector3(.5f,.5f,.5f));
            var bounds = new Bounds(center,size);
            newGizmo = AddCubeGizmo(gameObject,bounds,color);
            return true;
        }
        public static bool TryAddCubeGizmo(GameObject gameObject,Bounds bounds,Color color,out CubeGizmoComponent newGizmo) 
        {
            if(gameObject == null) 
            {
                newGizmo = null;
                return false;
            }
            newGizmo = AddCubeGizmo(gameObject,bounds,color);
            return true;
        }
        private static CubeGizmoComponent AddCubeGizmo(GameObject gameObject,Bounds bounds,Color color) 
        {
            var newGizmo = gameObject.AddComponent<CubeGizmoComponent>();
            newGizmo.UpdateColor(color);
            newGizmo.UpdateBounds(bounds);
            return newGizmo;
        }
    }
}
