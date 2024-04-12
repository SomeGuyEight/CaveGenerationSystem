using UnityEngine;
using System.Collections.Generic;
using Sylves;

namespace Tessera
{
    public static class TesseraToSlimeGameUtils
    {

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
        /// Directly modifies <see cref="TesseraTileInstance.Position"/> with the offset parameter
        /// </summary>
        /// <param name="tesseraTileInstance"></param>
        /// <param name="offset"></param>
        public static void OffsetPosition(this TesseraTileInstance tesseraTileInstance,Vector3 offset)
        {
            tesseraTileInstance.Position += offset;
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

        public static TesseraInitialConstraint GetInitialConstraint(this TesseraInitialConstraintBuilder builder,string name,List<SylvesOrientedFace> faceDetails,List<Vector3Int> sylvesOffsets,Cell cell,Sylves.CellRotation? cellRotation)
        {

            var rotation = cellRotation ?? builder.Grid.GetCellType(cell).GetIdentity();
            // TODO: Needs support for big tiles
            return new TesseraInitialConstraint
            {
                name = name,
                faceDetails = faceDetails,
                offsets = sylvesOffsets,
                cell = (Vector3Int)cell,
                rotation = rotation,
            };
        }

        public static bool TryGetInternalValues(this TesseraInitialConstraint ic,out Vector3Int cell,out Sylves.CellRotation cellRotation,out List<Vector3Int> offsets,out List<SylvesOrientedFace> faceDetails)
        {
            if (ic != null)
            {
                cell = ic.cell;
                cellRotation = ic.rotation;
                offsets = ic.offsets;
                faceDetails = ic.faceDetails;
                return true;
            }
            return FailedTryGetInternalValues(out cell,out cellRotation,out offsets,out faceDetails);
        }
        public static bool TryGetInternalValues(this TesseraPinConstraint ic,out Vector3Int cell,out Sylves.CellRotation cellRotation,out List<Vector3Int> offsets,out List<SylvesOrientedFace> faceDetails)
        {
            if (ic != null)
            {
                cell = ic.cell;
                cellRotation = ic.rotation;
                offsets = ic.tile.sylvesOffsets;
                faceDetails = ic.tile.sylvesFaceDetails;
                return true;
            }
            return FailedTryGetInternalValues(out cell,out cellRotation,out offsets,out faceDetails);
        }
        public static bool TryGetInternalValues(this ITesseraInitialConstraint ic,out Vector3Int cell,out Sylves.CellRotation cellRotation,out List<Vector3Int> offsets,out List<SylvesOrientedFace> faceDetails)
        {
            if (ic is TesseraInitialConstraint initialConstraint)
            {
                cell = initialConstraint.cell;
                cellRotation = initialConstraint.rotation;
                offsets = initialConstraint.offsets;
                faceDetails = initialConstraint.faceDetails;
                return true;
            }
            else if (ic is TesseraPinConstraint pinConstraint)
            {
                cell = pinConstraint.cell;
                cellRotation = pinConstraint.rotation;
                offsets = pinConstraint.tile.sylvesOffsets;
                faceDetails = pinConstraint.tile.sylvesFaceDetails;
                return true;
            }
            return FailedTryGetInternalValues(out cell,out cellRotation,out offsets,out faceDetails);
        }
        private static bool FailedTryGetInternalValues(out Vector3Int cell,out Sylves.CellRotation cellRotation,out List<Vector3Int> offsets,out List<SylvesOrientedFace> faceDetails)
        {
            cell = Vector3Int.zero;
            /// CubeRotation.identity -> doesn't really matter though        
            cellRotation = (Sylves.CellRotation)18;
            offsets = null;
            faceDetails = null;
            return false;
        }


        public static TesseraPalette GetNewTilePalette(this List<PaletteEntry> entries)
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
            return new()
            {
                color = entry.color,
                name = entry.name
            };
        }

        public static void OverrideOffsetsAndOrientedFaces(this TesseraTileBase tileBase,List<Vector3Int> offsets,List<SylvesOrientedFace> orientedFaces)
        {
            if (tileBase.sylvesOffsets != null)
            {
                tileBase.sylvesOffsets.Clear();
                tileBase.sylvesOffsets = null;
            }
            tileBase.sylvesOffsets = offsets;

            if (tileBase.sylvesFaceDetails != null)
            {
                tileBase.sylvesFaceDetails.Clear();
                tileBase.sylvesFaceDetails = null;
            }
            tileBase.sylvesFaceDetails = orientedFaces;
        }
    }
}
