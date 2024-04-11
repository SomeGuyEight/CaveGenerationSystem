using UnityEngine;
using Sylves;

namespace SlimeGame
{
    public interface ICubeGizmo
    {
        public Color Color { get; }
        public Bounds Bounds { get; }
        public bool IsEnabled { get; }

        public void SetEnabled(bool isEnabled);
        public void UpdateColor(Color color);
        public void UpdateBounds(Bounds bounds);
        public void UpdateBounds(CubeBound cubeBound,Vector3 cellSize);
    }
}
