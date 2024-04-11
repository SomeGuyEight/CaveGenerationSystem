using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public static class TesseraUtils
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
        private static readonly CellDir[] _allCellDir = _allFaceDirs.Select(x => x.ToCellDir()).ToArray();


        /// <summary>
        /// Returns a <see cref="CubeBound"/> that contains all the <see cref="TesseraTileInstance"/>'s <see cref="TesseraTileInstance.Cell"/> &amp; <see cref="TesseraTileInstance.Cells"/>
        /// <br/> ( ! ) Does NOT change any <see cref="TesseraTileInstance"/> values
        /// </summary>
        public static CubeBound GetCellBound(this TesseraTileInstance tesseraTileInstance)
        {
            var min = tesseraTileInstance.Cell;
            var maxCell = tesseraTileInstance.Cell;
            for (int i = 0;i < tesseraTileInstance.Cells.Length;i++)
            {
                min = Vector3Int.Min(min,tesseraTileInstance.Cells[i]);
                maxCell = Vector3Int.Max(maxCell,tesseraTileInstance.Cells[i]);
            }
            return new CubeBound(min,maxCell + Vector3Int.one);
        }

        /// <summary>
        /// Aligns the <see cref="TesseraTileInstance"/>'s  Cell &amp; Cells values to a new Cell
        /// <br/><br/> ( ! ) DOES affect Postition, Cell &amp; Cells values
        /// <br/> ( ! ) Does NOT affect Rotation or Scale values
        /// <br/><br/> -> This method still maintains the <see cref="TesseraTileInstance"/>'s alignment to it's Grid &amp; Changes the Cell &amp; Cells Values
        /// </summary>
        public static void AlignCellsAndPosition(this TesseraTileInstance tesseraTileInstance,Vector3Int newCell,Vector3 cellSize)
        {
            tesseraTileInstance.AlignCells(newCell);
            tesseraTileInstance.AlignPosition(newCell,cellSize);
        }

        /// <summary>
        /// Aligns the <see cref="TesseraTileInstance"/>'s  Cell &amp; Cells values to a new Cell
        /// <br/><br/> ( ! ) DOES affect Cell &amp; Cells values
        /// <br/> ( ! ) Does NOT affect Postition, Rotation, or Scale 
        /// </summary>
        public static void AlignCells(this TesseraTileInstance tesseraTileInstance,Vector3Int newCell)
        {
            var cellOffset = newCell - tesseraTileInstance.Cell;
            if (cellOffset == Vector3Int.zero)
            {
                return;
            }

            tesseraTileInstance.Cell = newCell;
            for (int i = 0;i < tesseraTileInstance.Cells.Length;i++)
            {
                tesseraTileInstance.Cells[i] = tesseraTileInstance.Cells[i] + cellOffset;
            }
        }

        /// <summary>
        /// Aligns <see cref="TesseraTileInstance"/>'s <see cref="TRS"/> to a new Position
        /// <br/><br/> ( ! ) DOES affect Position values
        /// <br/> ( ! ) Does NOT affect Cell, Cells, Rotation, or Scale values
        /// <br/><br/> -> This method still maintains the <see cref="TesseraTileInstance"/>'s alignment to it's Grid, but does not affect the Cell &amp; Cells values
        /// </summary>
        public static void AlignPosition(this TesseraTileInstance tesseraTileInstance,Vector3Int newCell,Vector3 cellSize)
        {
            var newPosition = Vector3.Scale(newCell + new Vector3(.5f,.5f,.5f),cellSize);
            tesseraTileInstance.LocalPosition += newPosition - tesseraTileInstance.Position;
            tesseraTileInstance.Position = newPosition;
        }


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


        public static TesseraPalette GetNewTilePalette(List<PaletteEntry> entries)
        {
            var newPalette = ScriptableObject.CreateInstance<TesseraPalette>();
            newPalette.entries = entries.DeepClone();
            return newPalette;
        }
        private static List<PaletteEntry> DeepClone(this List<PaletteEntry> entries)
        {
            List<PaletteEntry> clone = new (entries.Count);
            for (var i = 0;i < entries.Count;i++)
            {
                clone.Add(entries[i].DeepClone());
            }
            return clone;
        }
        private static PaletteEntry DeepClone(this PaletteEntry entry)
        {
            return new () 
            { 
                color = entry.color, 
                name = entry.name 
            };
        }
    }
}