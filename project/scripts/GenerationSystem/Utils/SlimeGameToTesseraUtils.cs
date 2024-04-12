using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public static class SlimeGameToTesseraUtils
    {
        private class RelativeCorners
        {
            public RelativeCorners(Corners topLeft,Corners topRight,Corners bottomLeft,Corners bottomRight)
            {
                TopLeft     = topLeft; 
                TopRight    = topRight;
                BottomLeft  = bottomLeft;
                BottomRight = bottomRight;
            }
            public Corners TopLeft      { get; private set; }
            public Corners TopRight     { get; private set; }
            public Corners BottomLeft   { get; private set; }
            public Corners BottomRight  { get; private set; }
        }
        private static readonly Dictionary<Directions,RelativeCorners> _relativeCorners = new ()
        {
            { Directions.Right , new (Corners.RUB,Corners.RUF,Corners.RDB,Corners.RDF) },
            { Directions.Left  , new (Corners.LUF,Corners.LUB,Corners.LDF,Corners.LDB) },
            { Directions.Up    , new (Corners.LUF,Corners.RUF,Corners.LUB,Corners.RUB) },
            { Directions.Down  , new (Corners.RDF,Corners.LDF,Corners.RDB,Corners.LDB) },
            { Directions.Fwd   , new (Corners.RUF,Corners.LUF,Corners.RDF,Corners.LDF) },
            { Directions.Back  , new (Corners.LUB,Corners.RUB,Corners.LDB,Corners.RDB) },
        };
        private static readonly Directions[] _allFaceDirs = DirectionsHelper.AllFaceDirections;

        public static SylvesOrientedFace DeepClone(this SylvesOrientedFace orientedFace,Vector3Int? offset = null)
        {
            return new(offset ?? orientedFace.offset,orientedFace.dir,orientedFace.faceDetails.DeepClone());
        }
        public static List<SylvesOrientedFace> ToOrientedFaces(this FaceDetails faceDetails,CellDir cellDirs,Vector3Int? offset = null)
        {
            return new() { new (offset ?? Vector3Int.zero,cellDirs,faceDetails) };
        }
        public static List<SylvesOrientedFace> ToOrientedFaces(this MCTile mCTile,int airPaint,int voidPaint,Vector3Int? offset = null)
        {
            return ToOrientedFaces((Corners)mCTile,airPaint,voidPaint,offset ?? Vector3Int.zero);
        }
        public static List<SylvesOrientedFace> ToOrientedFaces(this Corners cornersMask,int airPaint,int voidPaint,Vector3Int? offset = null)
        {
            List<SylvesOrientedFace> orientedFaces = new (6);
            foreach (var faceDirs in _allFaceDirs)
            {
                var faceDetails = cornersMask.GetSquareFaceDetails(faceDirs,airPaint,voidPaint);
                orientedFaces.Add(new(offset ?? Vector3Int.zero,faceDirs.ToCellDir(),faceDetails));
            }
            return orientedFaces;
        }
        public static bool DoAllCornerPaintsMatch(this List<SylvesOrientedFace> sylvesOrientedFaces,out int paintIndex)
        {
            paintIndex = -1;
            foreach (var (_, _, faceDetails) in sylvesOrientedFaces)
            {
                if (faceDetails != null)
                {
                    if (!DoCornerPaintsMatch(faceDetails,out var nextPaintIndex))
                    {
                        paintIndex = -1;
                        return false;
                    }
                    if (paintIndex == -1)
                    {
                        paintIndex = nextPaintIndex;
                    }
                    if (paintIndex != nextPaintIndex)
                    {
                        paintIndex = -1;
                        return false;
                    }
                }
            }
            return true;

            static bool DoCornerPaintsMatch(FaceDetails faceDetails,out int cornerPaintIndex)
            {
                cornerPaintIndex = faceDetails.topLeft;
                if (cornerPaintIndex != faceDetails.topRight)
                {
                    cornerPaintIndex = -1;
                }
                else if (cornerPaintIndex != faceDetails.bottomLeft)
                {
                    cornerPaintIndex = -1;
                }
                else if (cornerPaintIndex != faceDetails.bottomRight)
                {
                    cornerPaintIndex = -1;
                }
                return cornerPaintIndex != -1;
            }
        }

        public static FaceDetails GetSquareFaceDetails(this Corners cornersMask,Directions faceDirs,int airPaint,int voidPaint)
        {
            var relativeCorners = _relativeCorners[faceDirs];
            FaceDetails faceDetails = new ()
            {
                faceType    = FaceType.Square,
                topLeft     = cornersMask.HasFlags(relativeCorners.TopLeft     ) ? airPaint : voidPaint,
                topRight    = cornersMask.HasFlags(relativeCorners.TopRight    ) ? airPaint : voidPaint,
                bottomLeft  = cornersMask.HasFlags(relativeCorners.BottomLeft  ) ? airPaint : voidPaint,
                bottomRight = cornersMask.HasFlags(relativeCorners.BottomRight ) ? airPaint : voidPaint,
            };
            return faceDetails;
        }
        public static FaceDetails DeepClone(this FaceDetails faceDetails,FaceType? faceType = null)
        {
            return new()
            {
                faceType    = faceType ?? faceDetails.faceType,
                topLeft     = faceDetails.topLeft,
                top         = faceDetails.top,
                topRight    = faceDetails.topRight,
                left        = faceDetails.left,
                center      = faceDetails.center,
                right       = faceDetails.right,
                bottomLeft  = faceDetails.bottomLeft,
                bottom      = faceDetails.bottom,
                bottomRight = faceDetails.bottomRight,
            };
        }
        public static FaceDetails GetMatchingCornerPaint(int paintIndex)
        {
            return new()
            {
                faceType = FaceType.Square,
                topLeft     = paintIndex,
                topRight    = paintIndex,
                bottomLeft  = paintIndex,
                bottomRight = paintIndex,
            };
        }

    }
}
