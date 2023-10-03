using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sylves;

namespace Tessera.CaveGeneration {
    public class PathAnchor {
        public Vector3 WorldPosition { get; set; } // maybe?
        public CoreChunk ParentCoreChunk { get; set; }
        public Sylves.Cell ChunkCell { get; set; }
        public TesseraPinned PinnedTile { get; set; }

        /* from public TesseraTile : TesseraTileBase
         * 
           public TesseraTile()
          {
              sylvesFaceDetails = new List<SylvesOrientedFace>
              {
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Left, new FaceDetails() ),
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Right, new FaceDetails() ),
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Up, new FaceDetails() ),
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Down, new FaceDetails() ),
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Forward, new FaceDetails() ),
                  new SylvesOrientedFace(Vector3Int.zero, (Sylves.CellDir)Sylves.CubeDir.Back, new FaceDetails() ),
              };
          }

          public override Sylves.IGrid SylvesCellGrid => SylvesExtensions.CubeGridInstance;
          public override Sylves.ICellType SylvesCellType => SylvesExtensions.CubeCellType;

          public BoundsInt GetBounds()
          {
              var min = sylvesOffsets[0];
              var max = min;
              foreach (var o in sylvesOffsets)
              {
                  min = Vector3Int.Min(min, o);
                  max = Vector3Int.Max(max, o);
              }

              return new BoundsInt(min, max - min);
          }

           /// from TesseraGeneratorHelper
           private void ApplyInitialConstraintsAndSkybox()
          {
              var t1 = DateTime.Now;
              var mask = maskedTopology.Mask;

              var initialConstraintHelper = new TesseraInitialConstraintHelper(propagator, grid, tileModelInfo, palette);

              foreach (var ic in options.initialConstraints)
              {
                  initialConstraintHelper.Apply(ic);
                  CheckStatus($"Contradiction after setting initial constraint {ic.Name}.");
              }
              CheckStatus("Contradiction after setting initial constraints.");
              stats.initialConstraintsTime += (DateTime.Now - t1).TotalSeconds;


              // Apply skybox (if any)
              if (options.skyBox != null)
              {
                  t1 = DateTime.Now;
                  var cellType = grid.GetCellType();
                  var skyBoxFaceDetailsByFaceDir = cellType.GetCellDirs().ToDictionary(x => x, x => {
                      var ix = cellType.Invert(x);
                      return options.skyBox.faceDetails.FirstOrDefault(f => f.dir == x).faceDetails ??
                          // For triangles, try the inverse direction
                          options.skyBox.faceDetails.FirstOrDefault(f => f.dir == ix).faceDetails ??
                          throw new Exception($"Couldn't find skybox for direction {x}");
                  });
                  foreach (var cell in grid.GetCells())
                  {
                      foreach (var dir in grid.GetCellDirs(cell))
                      {
                          if (!grid.TryMove(cell, dir, out var destCell, out var _, out var _))
                          {
                              // Edge of grid (unmasked)
                              initialConstraintHelper.FaceConstrain2((Vector3Int)cell, dir, skyBoxFaceDetailsByFaceDir[dir]);
                          }
                      }
                  }
                  stats.skyboxTime += (DateTime.Now - t1).TotalSeconds;
              }
              CheckStatus("Contradiction after setting initial constraints and skybox.");
          }
        */
    }
}
