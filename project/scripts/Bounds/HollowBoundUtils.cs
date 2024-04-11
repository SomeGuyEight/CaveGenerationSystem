using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame
{
    public static class HollowBoundUtils
    {
        public static IEnumerable<CubeBounds> SearchFromDirections(CubeBound maxBound,Directions directions,int? stepThickness)
        {
            var initialBound = GetInitialBound(maxBound,directions);
            var targetPosition = GetTargetPosition(initialBound);
            /// set thickness to 0 b/c each iteration below will increment the bounds
            /// >> with 0 thickness, no cells are skipped on first iteration
            var hollowBound = new HollowBound(initialBound,maxBound,0,false);
            hollowBound.UpdateThicknesses(stepThickness ?? 1);

            CubeBounds cubeBounds = new(new[] { hollowBound.GetBoundFromDirections(Directions.None) },targetPosition);

            if (hollowBound.DoesInnerBoundContainCells())
            {
                /// holow center has cells >> only happens if all lengths are odd values
                /// -> return cells so none are skipped
                yield return cubeBounds;
            }

            int tempFailsafeTally = 0;
            while (hollowBound.MaxedDirections != Directions.All && tempFailsafeTally < 500)
            {
                hollowBound.IncrementAllDirections();
                cubeBounds.Bounds = hollowBound.GetBoundsFromDirectionsFlags(Directions.All.UnsetFlags(hollowBound.MaxedDirections),false);      
                yield return cubeBounds;

                tempFailsafeTally++;
            }
        }
        private static CubeBound GetInitialBound(CubeBound maxBound,Directions directions)
        {
            var posDirs = Directions.Right;
            var negDirs = Directions.Left;
            GetMinMax(maxBound.min.x,maxBound.max.x,maxBound.size.x,out int minX,out int maxX);
            posDirs = Directions.Up;
            negDirs = Directions.Down;
            GetMinMax(maxBound.min.y,maxBound.max.y,maxBound.size.y,out int minY,out int maxY);
            posDirs = Directions.Fwd;
            negDirs = Directions.Back;
            GetMinMax(maxBound.min.z,maxBound.max.z,maxBound.size.z,out int minZ,out int maxZ);

            return new(new(minX,minY,minZ),new(maxX,maxY,maxZ));

            void GetMinMax(int min,int max,int length,out int minResult,out int maxResult)
            {
                if (directions.HasFlags(posDirs))
                {
                    minResult = maxResult = max;
                }
                else if (directions.HasFlags(negDirs))
                {
                    minResult = maxResult = min;
                }
                else
                {
                    minResult = min + Math.DivRem(length,2,out int rem);
                    maxResult = minResult + rem;
                }
            }
        }
        private static Vector3 GetTargetPosition(CubeBound initialBound)
        {
            var min = initialBound.min;
            var max = initialBound.max;

            float targetX = min.x + (.5f * (max.x - min.x));
            float targetY = min.y + (.5f * (max.y - min.y));
            float targetZ = min.z + (.5f * (max.z - min.z));

            return new Vector3(targetX,targetY,targetZ);
        }

        /// <summary>
        /// WIP: Creates an evenly sized <see cref="HollowBound"/> with 27 <see cref="CubeBound"/> filling the maxBound (<see cref="CubeBound"/>) parameter
        /// </summary>
        /// <param name="maxBound"> The <see cref="CubeBound"/> containing the cells to segement into 27 directional Bounds</param>
        public static HollowBound CreateSegmentedHollowBound(CubeBound maxBound)
        {
            var xThird = Math.DivRem(maxBound.size.x,3,out var xRem);
            var xThickness = xRem == 2 ? xThird + 1 : xThird;
            var yThird = Math.DivRem(maxBound.size.y,3,out var yRem);
            var yThickness = yRem == 2 ? yThird + 1 : yThird;
            var zThird = Math.DivRem(maxBound.size.z,3,out var zRem);
            var zThickness = zRem == 2 ? zThird + 1 : zThird;

            Vector3Int innerMin = new Vector3Int(maxBound.min.x + xThickness,maxBound.min.y + yThickness,maxBound.min.z + zThickness);
            Vector3Int innerMax = new Vector3Int(maxBound.max.x - xThickness,maxBound.max.y - yThickness,maxBound.max.z - zThickness);
            KeyValuePair<Directions,int>[] dirsThicknessKVPs = new KeyValuePair<Directions,int>[]{
                new(Directions.Right ,xThickness),
                new(Directions.Left  ,xThickness),
                new(Directions.Up    ,yThickness),
                new(Directions.Down  ,yThickness),
                new(Directions.Fwd   ,zThickness),
                new(Directions.Back  ,zThickness),
            };

            return new HollowBound(innerMin,innerMax,maxBound,dirsThicknessKVPs,false);
        }
    }
}
