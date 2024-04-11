using UnityEngine;
using Sylves;

namespace SlimeGame 
{
    /// <summary>
    /// WIP class to be used with <see cref="CubeGizmoManager"/>
    /// <br/> -> non <see cref="MonoBehaviour"/> version of <see cref="CubeGizmoComponent"/>
    /// </summary>
    public class CubeGizmo : ICubeGizmo 
    {
        public CubeGizmo() { }
        public CubeGizmo(CubeGizmoManager manager) 
        {
            _cubeGizmoManager = manager;
        }
        public CubeGizmo(CubeGizmoManager manager,Color color,CubeBound cubeBound,Vector3 cellSize) 
        {
            _cubeGizmoManager = manager;
            UpdateColor(color);
            UpdateBounds(cubeBound,cellSize);
        }
        public CubeGizmo(CubeGizmoManager manager,Color color,Bounds bounds)
        {
            _cubeGizmoManager = manager;
            UpdateColor(color);
            UpdateBounds(bounds);
        }

        private Color _color;
        private Bounds _bounds;
        private bool _isEnabled;
        private readonly CubeGizmoManager _cubeGizmoManager;

        public Color Color { get { return _color; } }
        public Bounds Bounds { get { return _bounds; } }
        public bool IsEnabled { get { return _isEnabled; } }

        public void SetEnabled(bool isEnabled) 
        {
            if(_isEnabled != isEnabled) 
            {
                _isEnabled = isEnabled;
                if(_cubeGizmoManager != null)
                {
                    _cubeGizmoManager.TrySetGizmoEnabled(this,IsEnabled);
                }
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
    }
}
