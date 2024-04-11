using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame
{
    public class HollowBound
    {
        public HollowBound(CubeBound innerBound,CubeBound maxBound,int thickness,bool cacheBounds)
        {
            _outerBound = new CubeBound(innerBound.min,innerBound.max);
            _maxBound = maxBound != null ? new(maxBound.min,maxBound.max) : null;
            UpdateThicknesses(thickness);
            IncrementAllDirections();
            TrySetCachedBoundsEnabled(cacheBounds);
            TryUpdateMaxedDirections();
        }
        public HollowBound(Vector3Int innerMin,Vector3Int innerMax,CubeBound maxBound,KeyValuePair<Directions,int>[] thicknessKVPs,bool cacheBounds)
        {
            _outerBound = new CubeBound(innerMin,innerMax);
            _maxBound = maxBound != null ? new(maxBound.min,maxBound.max) : null;
            UpdateThicknesses(thicknessKVPs);
            IncrementAllDirections();
            TrySetCachedBoundsEnabled(cacheBounds);
            TryUpdateMaxedDirections();
        }

        /// <summary>
        /// Stores the tickness of each directional <see cref="CubeBound"/>
        /// <br/><br/> ( ! ) Only Face <see cref="Directions"/> are included
        /// <br/> If a Face <see cref="Directions"/> is incremented: 
        /// <br/> -> All 'directional' <see cref="CubeBound"/> with the <see cref="Directions"/> <see cref="FlagsAttribute"/> will be directly impacted
        /// <br/> ( eg. An increment of <see cref="Directions.Right"/> will innately increment <see cref="Directions.RightUpFwd"/> etc. by the same value )
        /// </summary>
        private Dictionary<Directions,int> _directionsToBoundThickness = new() 
        {
            { Directions.Right  , 1 },
            { Directions.Left   , 1 },
            { Directions.Up     , 1 },
            { Directions.Down   , 1 },
            { Directions.Fwd    , 1 },
            { Directions.Back   , 1 },
        };
        private CubeBound _outerBound;
        private CubeBound _innerBound;
        private readonly CubeBound _maxBound;
        private Directions _maxedDirections = Directions.None;
        private CubeBound[] _cachedBounds;

        public Directions MaxedDirections { get { return _maxedDirections; } }

        public bool IsCachedBoundsEnabled => _cachedBounds != null;
        public bool IsBounded => _maxBound != null;

        public bool DoesInnerBoundContainCells()
        {
            return _innerBound.min.x != _innerBound.max.x && _innerBound.min.y != _innerBound.max.y && _innerBound.min.z != _innerBound.max.z;
        }

        public void UpdateThicknesses(KeyValuePair<Directions,int>[] keyValues)
        {
            for (int i = 0;i < keyValues.Length;i++)
            {
                var kvp = keyValues[i];
                if (kvp.Key.IsFaceDir())
                {
                    _directionsToBoundThickness[kvp.Key] = kvp.Value;
                }
            }
        }
        public void UpdateThicknesses(int thickness)
        {
            _directionsToBoundThickness = new()
            {
                { Directions.Right , thickness },
                { Directions.Left  , thickness },
                { Directions.Up    , thickness },
                { Directions.Down  , thickness },
                { Directions.Fwd   , thickness },
                { Directions.Back  , thickness },
            };
        }
        private void TryUpdateMaxedDirections()
        {
            if (_maxBound != null)
            {
                _maxedDirections = _innerBound.max.x >= _maxBound.max.x ? _maxedDirections.SetFlags(Directions.Right) : _maxedDirections.UnsetFlags(Directions.Right);
                _maxedDirections = _innerBound.min.x <= _maxBound.min.x ? _maxedDirections.SetFlags(Directions.Left) : _maxedDirections.UnsetFlags(Directions.Left);
                _maxedDirections = _innerBound.max.y >= _maxBound.max.y ? _maxedDirections.SetFlags(Directions.Up) : _maxedDirections.UnsetFlags(Directions.Up);
                _maxedDirections = _innerBound.min.y <= _maxBound.min.y ? _maxedDirections.SetFlags(Directions.Down) : _maxedDirections.UnsetFlags(Directions.Down);
                _maxedDirections = _innerBound.max.z >= _maxBound.max.z ? _maxedDirections.SetFlags(Directions.Fwd) : _maxedDirections.UnsetFlags(Directions.Fwd);
                _maxedDirections = _innerBound.min.z <= _maxBound.min.z ? _maxedDirections.SetFlags(Directions.Back) : _maxedDirections.UnsetFlags(Directions.Back);
            }
        }
        private void TryUpdateCachedBounds()
        {
            if (IsCachedBoundsEnabled)
            {
                var directions = DirectionsHelper.GetDirectionsFromDirectionTypes(DirectionTypes.All,true);
                for (int i = 0;i < directions.Length;i++)
                {
                    _cachedBounds[(int)(directions[i].ToDirection())] = GetUnchacedBoundFromDirections(directions[i]);
                }
            }
        }
        public bool TrySetCachedBoundsEnabled(bool isEnabled)
        {
            if (isEnabled && !IsCachedBoundsEnabled)
            {
                /// 1 (innerBound) + 6 (faces) + 12 (edges) + 8 (corners)
                _cachedBounds = new CubeBound[27];
                TryUpdateCachedBounds();
                return true;
            }
            if (!isEnabled && IsCachedBoundsEnabled)
            {
                _cachedBounds = null;
                return true;
            }
            return false;
        }

        public void IncrementAllDirections()
        {
            var minOffset = new Vector3Int(-_directionsToBoundThickness[Directions.Left],-_directionsToBoundThickness[Directions.Down],-_directionsToBoundThickness[Directions.Back]);
            var maxOffset = new Vector3Int(_directionsToBoundThickness[Directions.Right],_directionsToBoundThickness[Directions.Up],_directionsToBoundThickness[Directions.Fwd]);

            _innerBound = _outerBound;
            _outerBound = new CubeBound(_outerBound.min + minOffset,_outerBound.max + maxOffset);

            ClampToMaxBound();
            TryUpdateCachedBounds();
            TryUpdateMaxedDirections();
        }
        public void IncrementByDirectionsFlags(Directions directions)
        {
            if (directions.HasFlags(Directions.Right))
            {
                _innerBound.max.x = _outerBound.max.x;
                _outerBound.max.x += _directionsToBoundThickness[Directions.Right];
            }
            if (directions.HasFlags(Directions.Left))
            {
                _innerBound.min.x = _outerBound.min.x;
                _outerBound.min.x -= _directionsToBoundThickness[Directions.Left];
            }
            if (directions.HasFlags(Directions.Up))
            {
                _innerBound.max.y = _outerBound.max.y;
                _outerBound.max.y += _directionsToBoundThickness[Directions.Up];
            }
            if (directions.HasFlags(Directions.Down))
            {
                _innerBound.min.y = _outerBound.min.y;
                _outerBound.min.y -= _directionsToBoundThickness[Directions.Down];
            }
            if (directions.HasFlags(Directions.Fwd))
            {
                _innerBound.max.z = _outerBound.max.z;
                _outerBound.max.z += _directionsToBoundThickness[Directions.Fwd];
            }
            if (directions.HasFlags(Directions.Back))
            {
                _innerBound.min.z = _outerBound.min.z;
                _outerBound.min.z -= _directionsToBoundThickness[Directions.Back];
            }

            ClampToMaxBound();
            TryUpdateCachedBounds();
            TryUpdateMaxedDirections();
        }
        private void ClampToMaxBound()
        {
            if (IsBounded) 
            {
                _innerBound.min = Vector3Int.Max(_innerBound.min,_maxBound.min);
                _outerBound.min = Vector3Int.Max(_outerBound.min,_maxBound.min);
                _innerBound.max = Vector3Int.Min(_innerBound.max,_maxBound.max);
                _outerBound.max = Vector3Int.Min(_outerBound.max,_maxBound.max);
            }
        }

        /// <summary>
        /// Directions do not need to be valid enum
        /// <br/> ( eg. "None","Right","RightUpFwd",etc ) 
        /// <br/> <br/> This method returns bounds according to each flag included
        /// <br/> ( eg1. "Right" => Right face bound ) 
        /// <br/> ( eg2. "RightUpFwd" returns an array with => Right, Up, &amp; Fwd Face CubeBounds + RightUp, RightFwd, &amp; UpFwd Edge CubeBounds + RightUpFwd CubeBound ) 
        /// <br/><br/> ( ! ) If a single Directional bound from a valid enum is needed 
        /// <br/> -> Use <see cref="GetBoundFromDirections"/> instead
        /// </summary>
        public CubeBound[] GetBoundsFromDirectionsFlags(Directions directions,bool includeNone)
        {
            if (directions == Directions.None)
            {
                return includeNone ? new CubeBound[] { GetBoundFromDirections(Directions.None) } : new CubeBound[0];
            }

            var flaggedDirs = directions.GetAllFlags(includeNone);
            var bounds = new CubeBound[flaggedDirs.Length];

            for (int i = 0;i < flaggedDirs.Length;i++)
            {
                bounds[i] = GetBoundFromDirections(flaggedDirs[i]);
            }

            return bounds;
        }
        /// <summary>
        /// Directions should be a valid <see cref="Directions"/> type
        /// <br/> ( eg. "None","Right","RightUpFwd",etc ) 
        /// <br/><br/> ( ! ) If multiple Directional bound are needed from a single Directions parameter using <see cref="FlagsAttribute"/>
        /// <br/> -> Use <see cref="GetBoundsFromDirectionsFlags"/> instead
        /// </summary>
        public CubeBound GetBoundFromDirections(Directions directions)
        {
            if (!directions.IsValidType())
            {
                throw new Exception($"{directions} were not valid when getting hollow bound by Directions");
            }
            if (IsCachedBoundsEnabled)
            {
                return _cachedBounds[(int)directions.ToDirection()];
            }
            return GetUnchacedBoundFromDirections(directions);
        }
        private CubeBound GetUnchacedBoundFromDirections(Directions directions)
        {
            Vector3Int min = _innerBound.min;
            Vector3Int max = _innerBound.max;

            if (directions.HasFlags(Directions.Right))
            {
                min.x = _innerBound.max.x;
                max.x = _outerBound.max.x;
            }
            if (directions.HasFlags(Directions.Left))
            {
                min.x = _outerBound.min.x;
                max.x = _innerBound.min.x;
            }
            if (directions.HasFlags(Directions.Up))
            {
                min.y = _innerBound.max.y;
                max.y = _outerBound.max.y;
            }
            if (directions.HasFlags(Directions.Down))
            {
                min.y = _outerBound.min.y;
                max.y = _innerBound.min.y;
            }
            if (directions.HasFlags(Directions.Fwd))
            {
                min.z = _innerBound.max.z;
                max.z = _outerBound.max.z;
            }
            if (directions.HasFlags(Directions.Back))
            {
                min.z = _outerBound.min.z;
                max.z = _innerBound.min.z;
            }

            return new CubeBound(min,max);
        }

    }
}