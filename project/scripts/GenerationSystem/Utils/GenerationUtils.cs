using UnityEngine;
using System;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public static class GenerationUtils
    {
        public static GameObject CreateDebugObject(this List<ITesseraInitialConstraint> constraints,string title,TesseraPalette palette,CubeBound generationBound,Vector3 cellSize)
        {
            GameObject tileObj = new ($"{title} ( min = {generationBound.min}; max = {generationBound.max}; size = {generationBound.size} )");
            var tesseraTile = tileObj.AddComponent<TesseraTile>();
            tesseraTile.cellSize = cellSize;
            tesseraTile.palette = palette;

            List<Vector3Int> offsets = new ();
            List<SylvesOrientedFace> orientedFaces = new ();
            foreach (var ic in constraints)
            {
                /// TODO: Handle volume filters
                Vector3Int icCell;
                Sylves.CellRotation icCellRotation;
                List<Vector3Int> icOffsets;
                List<SylvesOrientedFace> icOrientedFaces;
                
                if (ic is TesseraInitialConstraint initialConstraint)
                {
                    if (!initialConstraint.TryGetInternalValues(out icCell,out icCellRotation, out icOffsets,out icOrientedFaces))
                    {
                        continue;
                    }
                }
                else if (ic is TesseraPinConstraint pinConstraint)
                {
                    if (!pinConstraint.TryGetInternalValues(out icCell,out icCellRotation,out icOffsets,out icOrientedFaces))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                foreach (var offset in icOffsets)
                {
                    offsets.Add(offset + icCell);
                }
                foreach (var orientedFace in icOrientedFaces)
                {
                    orientedFace.Deconstruct(out var faceOffset,out var dir,out var faceDetails);
                    var (rotatedDir, rotatedDetails) = CubeCellType.Instance.RotateBy(dir,faceDetails,icCellRotation);
                    orientedFaces.Add(new(faceOffset + icCell,rotatedDir,rotatedDetails));
                }
            }
            tileObj.transform.position = Vector3.Scale(generationBound.min + new Vector3(.5f,.5f,.5f),cellSize);
            tesseraTile.OverrideOffsetsAndOrientedFaces(offsets,orientedFaces);

            return tileObj;
        }

    }
    
}
