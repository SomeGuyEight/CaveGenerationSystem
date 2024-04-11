#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class BigTileGenerator
    {
        public static void SaveAsNewBigTile(BigTileGeneratorOptions options,string folderPath,string nameInsert = null)
        {
            if (options == null || options.CellInstances == null || options.CellInstances.Length < 1)
            {
                throw new Exception("Cell Instance array was empty and invalid to create big tile");
            }
            var cellOffset = GetCellOffset(options.CellInstances);
            InitializeCellToTesseraTileInstances(ref options,cellOffset);
            InitializeBigTileOffsetsAndFaces(ref options);
            SaveBigTileAsPrefab(options,folderPath,$"Tessera Big Tile ( {nameInsert ?? ""} ) {SGUtils.DateTimeStamp()}");
        }

        private static Vector3Int GetCellOffset(CellInstance[] cellInstances)
        {
            var rawMin = cellInstances[0].CellBound.min;
            for (int i = 0;i < cellInstances.Length;i++)
            {
                rawMin = Vector3Int.Min(rawMin,cellInstances[i].CellBound.min);
            }
            return Vector3Int.zero - rawMin;
        }
        private static void InitializeCellToTesseraTileInstances(ref BigTileGeneratorOptions options,Vector3Int cellOffset)
        {
            var cellInstances = options.CellInstances;
            var cellSize = options.CellSize;
            var halfCell = Vector3.Scale(cellSize,new Vector3(.5f,.5f,.5f));
            Dictionary <Vector3Int,TesseraTileInstance> cellsToTesseraTileInstances = new ();
            for (int i = 0;i < cellInstances.Length;i++)
            {
                var currentCell = cellInstances[i].CellBound.min;
                var newCell = currentCell + cellOffset;
                var deepCopyOfInstance = cellInstances[i].TesseraInstance.Clone();
                deepCopyOfInstance.AlignCellsAndPosition(newCell,cellSize);
                /// to align the center of local cell (0,0,0) with (0,0,0) 
                /// -> this matches how the TesseraTileBase stores offsets and makes working with them easier
                deepCopyOfInstance.Position -= halfCell;
                cellsToTesseraTileInstances.TryAdd(deepCopyOfInstance.Cell,deepCopyOfInstance);
            }
            options.CellToTesseraTileInstances = cellsToTesseraTileInstances;
        }
        private static void InitializeBigTileOffsetsAndFaces(ref BigTileGeneratorOptions options)
        {
            var cellGrid = options.CellGrid;
            var cellsToTesseraTileInstances = options.CellToTesseraTileInstances;
            List<Vector3Int> sylvesOffsets = new ();
            List<SylvesOrientedFace> orientedFaces = new ();
            foreach (var (cell, tesseraInstance) in cellsToTesseraTileInstances)
            {
                var rotation = tesseraInstance.CellRotation;
                Dictionary<CellDir,FaceDetails> cellDirsToFaceDetails = new ();
                foreach (var rawFaceDetails in tesseraInstance.Tile.sylvesFaceDetails)
                {
                    var (cellDir, faceDetails) = tesseraInstance.Tile.SylvesCellType.RotateBy(rawFaceDetails.dir,rawFaceDetails.faceDetails,rotation);
                    cellDirsToFaceDetails.Add(cellDir,faceDetails);
                }
                foreach (CellDir dir in cellGrid.GetCellDirs((Cell)cell))
                {
                    if (!cellGrid.TryMove((Cell)cell,dir,out var neigborCell,out _,out _))
                    {
                        continue;
                    }
                    if (cellsToTesseraTileInstances.ContainsKey((Vector3Int)neigborCell))
                    {
                        continue;
                    }
                    if (cellDirsToFaceDetails.TryGetValue(dir,out var faceDetails))
                    {
                        var deepClone = faceDetails.DeepClone();
                        if (!sylvesOffsets.Contains(cell))
                        {
                            sylvesOffsets.Add(cell);
                        }
                        orientedFaces.Add(new SylvesOrientedFace(cell,dir,deepClone));
                    }
                }
            }
            options.SylvesOffsets = sylvesOffsets;
            options.OrientedFaces = orientedFaces;
        }
        private static void SaveBigTileAsPrefab(BigTileGeneratorOptions options,string folderPath,string tileName)
        {
            var tempObject = new GameObject(tileName);
            var newTesseraTile = tempObject.AddComponent<TesseraTile>();

            /// ( ! ) Need to remove default face details b/c <see cref="TesseraTile"/> constructor adds six at (0,0,0)
            newTesseraTile.RemoveOffset(Vector3Int.zero);
            newTesseraTile.center = Vector3Int.zero;
            newTesseraTile.rotatable = options.RotationType != RotationGroupType.None;
            newTesseraTile.rotationGroupType = options.RotationType;
            newTesseraTile.symmetric = options.IsSymetric;
            newTesseraTile.palette = options.Database.Palette;

            /// GameObject and Sylves offsets are all oriented to (0,0,0)
            newTesseraTile.OverrideOffsetsAndOrientedFaces(options.SylvesOffsets,options.OrientedFaces);
            foreach (var tesseraTileInstance in options.CellToTesseraTileInstances.Values)
            {
                var objects = TesseraGenerator.Instantiate(tesseraTileInstance,tempObject.transform,tesseraTileInstance.Tile.gameObject,true);
                for (int i = 0;i < objects.Length;i++)
                {
                    var go = objects[i];
                    var newMeshRenderers = go.GetComponentsInChildren<MeshRenderer>();
                    for (int ii = 0;ii < newMeshRenderers.Length;ii++)
                    {
                        if (newMeshRenderers[ii] != null)
                        {
                            newMeshRenderers[ii].material = options.MeshMaterial;
                        }
                    }
                }
            }
            PrefabUtility.SaveAsPrefabAsset(tempObject,folderPath + $"/{tileName}.prefab");
            GameObject.Destroy(tempObject);
        }
    }
}
#endif
