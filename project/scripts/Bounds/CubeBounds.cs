using UnityEngine;
using System;
using System.Collections;
using Sylves;

namespace SlimeGame
{
    public class CubeBounds : IEnumerable
    {
        public CubeBounds(CubeBound[] bounds,Vector3? targetPosition)
        {
            Bounds = bounds;
            TargetPosition = targetPosition;
        }

        public CubeBound[] Bounds;
        public Vector3? TargetPosition;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        private CubeBoundEnum GetEnumerator() => new (Bounds);
    }

    internal class CubeBoundEnum : IEnumerator
    {
        public CubeBoundEnum(CubeBound[] bounds)
        {
            Bounds = bounds;
        }

        private int _index = -1;
        public readonly CubeBound[] Bounds;

        public bool MoveNext() => ++_index < Bounds.Length;
        public void Reset()
        {
            _index = -1;
        }

        object IEnumerator.Current => Current;
        public CubeBound Current
        {
            get
            {
                try
                {
                    return Bounds[_index];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
