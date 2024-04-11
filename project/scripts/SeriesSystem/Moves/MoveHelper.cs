using UnityEngine;

namespace SlimeGame 
{
    public static class MoveHelper 
    {
        private readonly static Vector3 _halfVector3 = new (.5f,.5f,.5f);
        private readonly static float _defaultScale = 1;
        private readonly static float _minScale = .87f;
        private readonly static float _maxScale = 1.4f;

        /// <summary>
        /// returns an offset repesenting the neighbor cell relative to the input vector
        /// </summary>
        public static Vector3Int NeighborCellOffsetFromVector(Vector3 vector,float? scale = null) 
        {
            var finalScale = scale ?? _defaultScale;
            finalScale = finalScale < _minScale ? _minScale : finalScale > _maxScale ? _maxScale : finalScale;
            return Vector3Int.FloorToInt(_halfVector3 + (finalScale * Vector3.Normalize(vector)));
        }
    }
}