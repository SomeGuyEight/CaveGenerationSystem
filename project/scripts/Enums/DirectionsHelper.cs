using UnityEngine;
using System;
using System.Collections.Generic;

namespace SlimeGame 
{
    public static class DirectionsHelper 
    {
        internal static readonly Dictionary<Directions,Vector3Int> DirectionsToNeighborOffset = new () 
        {
            { Directions.None           ,Vector3Int.zero            },
            { Directions.Right          ,Vector3Int.right           },
            { Directions.Left           ,Vector3Int.left            },
            { Directions.Up             ,Vector3Int.up              },
            { Directions.Down           ,Vector3Int.down            },
            { Directions.Fwd            ,Vector3Int.forward         },
            { Directions.Back           ,Vector3Int.back            },
            { Directions.RightUp        ,new Vector3Int( 1, 1, 0)   },
            { Directions.RightDown      ,new Vector3Int( 1,-1, 0)   },
            { Directions.RightFwd       ,new Vector3Int( 1, 0, 1)   },
            { Directions.RightBack      ,new Vector3Int( 1, 0,-1)   },
            { Directions.LeftUp         ,new Vector3Int(-1, 1, 0)   },
            { Directions.LeftDown       ,new Vector3Int(-1,-1, 0)   },
            { Directions.LeftFwd        ,new Vector3Int(-1, 0, 1)   },
            { Directions.LeftBack       ,new Vector3Int(-1, 0,-1)   },
            { Directions.UpFwd          ,new Vector3Int( 0, 1, 1)   },
            { Directions.UpBack         ,new Vector3Int( 0, 1,-1)   },
            { Directions.DownFwd        ,new Vector3Int( 0,-1, 1)   },
            { Directions.DownBack       ,new Vector3Int( 0,-1,-1)   },
            { Directions.RightUpFwd     ,new Vector3Int( 1, 1, 1)   },
            { Directions.RightUpBack    ,new Vector3Int( 1, 1,-1)   },
            { Directions.RightDownFwd   ,new Vector3Int( 1,-1, 1)   },
            { Directions.RightDownBack  ,new Vector3Int( 1,-1,-1)   },
            { Directions.LeftUpFwd      ,new Vector3Int(-1, 1, 1)   },
            { Directions.LeftUpBack     ,new Vector3Int(-1, 1,-1)   },
            { Directions.LeftDownFwd    ,new Vector3Int(-1,-1, 1)   },
            { Directions.LeftDownBack   ,new Vector3Int(-1,-1,-1)   },
        };
        /// <summary>
        /// ( ! ) Does not include <see cref="Directions.None"/> ( ! )
        /// <br/> Valid Directions => no inverse pairs flagged ( eg. "Right" &amp; "Left" flagged)
        /// </summary>
        internal static readonly Directions[] AllValidDirections = new []
        {
            Directions.Right         ,
            Directions.Left          ,
            Directions.Up            ,
            Directions.Down          ,
            Directions.Fwd           ,
            Directions.Back          ,
            Directions.RightUp       ,
            Directions.RightDown     ,
            Directions.RightFwd      ,
            Directions.RightBack     ,
            Directions.LeftUp        ,
            Directions.LeftDown      ,
            Directions.LeftFwd       ,
            Directions.LeftBack      ,
            Directions.UpFwd         ,
            Directions.UpBack        ,
            Directions.DownFwd       ,
            Directions.DownBack      ,
            Directions.RightUpFwd    ,
            Directions.RightUpBack   ,
            Directions.RightDownFwd  ,
            Directions.RightDownBack ,
            Directions.LeftUpFwd     ,
            Directions.LeftUpBack    ,
            Directions.LeftDownFwd   ,
            Directions.LeftDownBack  ,
        };
        public static Directions[] AllFaceDirections => new Directions[]
        {
            Directions.Right,
            Directions.Left,
            Directions.Up,
            Directions.Down,
            Directions.Fwd,
            Directions.Back,
        };
        public static Directions[] AllEdgeDirections => new Directions[] 
        {
            Directions.RightUp,
            Directions.RightDown,
            Directions.RightFwd,
            Directions.RightBack,
            Directions.LeftUp,
            Directions.LeftDown,
            Directions.LeftFwd,
            Directions.LeftBack,
            Directions.UpFwd,
            Directions.UpBack,
            Directions.DownFwd,
            Directions.DownBack,
        };
        public static Directions[] AllCornerDirections => new Directions[] 
        {
            Directions.RightUpFwd,
            Directions.RightUpBack,
            Directions.RightDownFwd,
            Directions.RightDownBack,
            Directions.LeftUpFwd,
            Directions.LeftUpBack,
            Directions.LeftDownFwd,
            Directions.LeftDownBack,
        };

        public static Directions[] GetDirectionsFromDirectionTypes(DirectionTypes directionTypes,bool includeNone)
        {
            directionTypes = directionTypes.UnsetFlags(DirectionTypes.Horizontal | DirectionTypes.Vertical);
            return directionTypes switch 
            {
                DirectionTypes.Face       => GetCombineDirections(AllFaceDirections,null,null,includeNone),
                DirectionTypes.Edge       => GetCombineDirections(AllEdgeDirections,null,null,includeNone),
                DirectionTypes.Corner     => GetCombineDirections(AllCornerDirections,null,null,includeNone),
                DirectionTypes.FaceEdge   => GetCombineDirections(AllFaceDirections,AllEdgeDirections,null,includeNone),
                DirectionTypes.FaceCorner => GetCombineDirections(AllFaceDirections,AllCornerDirections,null,includeNone),
                DirectionTypes.EdgeCorner => GetCombineDirections(AllEdgeDirections,AllCornerDirections,null,includeNone),
                DirectionTypes.All        => GetCombineDirections(AllFaceDirections,AllEdgeDirections,AllCornerDirections,includeNone),
                _ => throw new Exception("Failed to get Direction Array from DirectionTypes"),
            };
        }
        private static Directions[] GetCombineDirections(Directions[] dirs1,Directions[] dirs2,Directions[] dirs3,bool includeNone = false)
        {
            int count = includeNone ? 1 : 0;
            count = (dirs1 == null) ? count : count + dirs1.Length;
            count = (dirs2 == null) ? count : count + dirs2.Length;
            count = (dirs3 == null) ? count : count + dirs3.Length;
            if(count == 0) 
            {
                return null;
            }

            var combinedArray = new Directions[count];
            var index = 0;
            if(includeNone) 
            {
                combinedArray[index] = Directions.None;
                index++;
            }
            AddArray(dirs1);
            AddArray(dirs2);
            AddArray(dirs3);

            return combinedArray;

            void AddArray(Directions[] inputArray)
            {
                if(inputArray == null)
                {
                    return;
                }
                for(int i = 0; i < inputArray.Length; i++)
                {
                    combinedArray[index] = inputArray[i];
                    index++;
                }
            }
        }

    }
}
