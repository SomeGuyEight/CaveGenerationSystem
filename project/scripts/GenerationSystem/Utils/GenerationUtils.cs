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
                /// TODO: 
                /// 1. Handle volumes => ?? place box collider or gizmo & new obj name has info ??
                /// 2. Make a better workaround for volume constraints
                /// > In current implementation of <see cref="TesseraVolumeFilter"/> ... <see cref="Dummy_TesseraVolumeFilter"/>
                ///     >> return null when Cell, Offsets, or FaceDetails are requested
                ///     >> throws an exception when CellRotation are requested
                if (ic is TesseraVolumeFilter)
                {
                    continue;
                }

                var cell = (Vector3Int)ic.Cell;
                foreach (var offset in ic.Offsets)
                {
                    offsets.Add(offset + cell);
                }
                foreach (var orientedFace in ic.FaceDetails)
                {
                    orientedFace.Deconstruct(out var faceOffset,out var dir,out var faceDetails);
                    var (rotatedDir, rotatedDetails) = CubeCellType.Instance.RotateBy(dir,faceDetails,ic.CellRotation);
                    orientedFaces.Add(new(faceOffset + cell,rotatedDir,rotatedDetails));
                }
            }
            tileObj.transform.position = Vector3.Scale(generationBound.min + new Vector3(.5f,.5f,.5f),cellSize);
            tesseraTile.OverrideOffsetsAndOrientedFaces(offsets,orientedFaces);

            return tileObj;
        }

    }
    
}
