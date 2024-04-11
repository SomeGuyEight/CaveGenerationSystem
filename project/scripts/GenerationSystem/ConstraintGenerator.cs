using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public static class ConstraintGenerator
    {

        public static List<ITesseraInitialConstraint> BuildGenerationConstraints(this ICellInstanceCollection constraintCells,out Cell[] airCellsToUpdate)
        {
            List<ITesseraInitialConstraint> initialConstraints = new ();
            AddSkyBoxConstraints(ref initialConstraints,constraintCells);
            AddGenerationBoundConstraints(ref initialConstraints,constraintCells,out airCellsToUpdate);
            return initialConstraints;
        }
        private static List<ITesseraInitialConstraint> AddSkyBoxConstraints(ref List<ITesseraInitialConstraint> initialConstraints,ICellInstanceCollection constraintCells)
        {
            var hollowBound = new HollowBound(constraintCells.GenerationBound,constraintCells.ConstraintBound,1,false);
            var allFaceDirs = DirectionsHelper.AllFaceDirections;

            for (int i = 0;i < allFaceDirs.Length;i++)
            {
                var borderDirs = allFaceDirs[i];
                var borderBound = hollowBound.GetBoundFromDirections(borderDirs);
                AddBorderSkyBoxConstraints(ref initialConstraints,constraintCells,borderDirs,borderBound);
            }
            return initialConstraints;
        }
        private static void AddBorderSkyBoxConstraints(ref List<ITesseraInitialConstraint> initialConstraints,ICellInstanceCollection constraintCells,Directions borderDirections,CubeBound borderBound)
        {
            var inverseDirs = borderDirections.Invert().ToCellDir();
            var airOrientedFaces = constraintCells.Database.AirFaceDetails.ToOrientedFaces(inverseDirs);
            var voidOrientedFaces = constraintCells.Database.VoidFaceDetails.ToOrientedFaces(inverseDirs);

            var grid = constraintCells.Builder.Grid;
            Dictionary<Cell,Corners> cellToCornersMasks = new ();
            foreach (var externalCell in borderBound)
            {
                grid.TryMove(externalCell,inverseDirs,out var internalCell,out _,out _);
                if (!constraintCells.TryGetCellInstance((Vector3Int)internalCell,out var internalInstance,out var _) || internalInstance.CellTypes.HasFlags(CellTypes.Void))
                {
                    initialConstraints
                        .Add(constraintCells
                        .GetInitialConstraint((Vector3Int)externalCell,"Void AdjacentModel Skybox Cell",voidOrientedFaces));
                    continue;
                }
                if (internalInstance.CellTypes.HasFlags(CellTypes.Air))
                {
                    initialConstraints
                        .Add(constraintCells
                        .GetInitialConstraint((Vector3Int)externalCell,"Air AdjacentModel Skybox Cell",airOrientedFaces));
                    continue;
                }
                /// Significantly slower than just using corners -> MCTile below
                ///if (!internalInstance.Update)
                ///{
                ///    //var internalDetails = internalInstance.GetAppliedFaceDetails(borderDirs);
                ///    //var reflected = internalDetails.GetReflectedClone(FaceType.Square);
                ///    //var orientedFaces = reflected.ToOrientedFaces(inverseDirs);
                ///    //initialConstraints
                ///    //    .Add(constraintCells
                ///    //    .GetInitialConstraint((Vector3Int)externalCell,"Surface AdjacentModel Skybox Cell",orientedFaces));
                ///    //continue;
                ///}

                if (!constraintCells.TryGetCellInstance((Vector3Int)externalCell,out var externalInstance,out var _) || externalInstance.CellTypes.HasFlags(CellTypes.Void))
                {
                    initialConstraints
                        .Add(constraintCells
                        .GetInitialConstraint((Vector3Int)externalCell,"Void AdjacentModel Skybox Cell",voidOrientedFaces));
                    continue;                   
                }
                else if (externalInstance.CellTypes.HasFlags(CellTypes.Air))
                {
                    initialConstraints
                        .Add(constraintCells
                        .GetInitialConstraint((Vector3Int)externalCell,"Air AdjacentModel Skybox Cell",airOrientedFaces));
                    continue;
                }
                /// Significantly slower than just using corners -> MCTile below
                ///else if (externalInstance.TesseraInstance != null)
                ///{
                ///    //var externalDetails = externalInstance.GetAppliedFaceDetails(inverseDirs);
                ///    //var orientedFaces = externalDetails.ToOrientedFaces(inverseDirs);
                ///    //initialConstraints
                ///    //    .Add(constraintCells
                ///    //    .GetInitialConstraint((Vector3Int)externalCell,"Surface AdjacentModel Skybox Cell",orientedFaces));
                ///    //continue;
                ///}

                var cornersMask = GetBorderCellCornersMask(constraintCells,borderDirections,externalCell);
                cellToCornersMasks.Add(externalCell,cornersMask);
            }

            AddSkyBoxConstraintsFromCornersMasks(ref initialConstraints,constraintCells,borderDirections,cellToCornersMasks.ToArray());
        }
        private static Corners GetBorderCellCornersMask(ICellInstanceCollection constraintCells,Directions currentBorderDirs,Cell currentCell)
        {
            var inverseBorderDirs = currentBorderDirs.Invert();
            var innerCorners = inverseBorderDirs.ToCorners();

            var cellCorners = Corners.None;
            foreach (var (directions, neighborOffset) in DirectionsHelper.DirectionsToNeighborOffset)
            {
                if (directions.HasFlags(currentBorderDirs))
                {
                    /// cell has no impact on border constraint
                    continue;
                }
                if (constraintCells.TryGetCellInstance((Vector3Int)currentCell + neighborOffset,out var cellInstance,out _))
                {
                    if (cellInstance.CellTypes.HasFlags(CellTypes.Air))
                    {
                        cellCorners |= directions.ToCorners();
                    }
                }
                if (cellCorners.HasFlags(innerCorners))
                {
                    /// all corners impacting constraint complete
                    return cellCorners;
                }
            }
            return cellCorners;
        }
        private static void AddSkyBoxConstraintsFromCornersMasks(ref List<ITesseraInitialConstraint> initialConstraints,ICellInstanceCollection constraintCells,Directions borderDirections,KeyValuePair<Cell,Corners>[] cellToCornersMasks)
        {
            var airPaint = constraintCells.Database.AirPaint;
            var voidPaint = constraintCells.Database.VoidPaint;
            var inverseDirections = borderDirections.Invert();
            var inverseDirs = inverseDirections.ToCellDir();

            for (int i = 0;i < cellToCornersMasks.Length;i++)
            {
                var (cell,cornersMask) = cellToCornersMasks[i];
                var faceDetails = cornersMask.GetSquareFaceDetails(inverseDirections,airPaint,voidPaint);
                var orientedFaceList = constraintCells.GetZeroOffsetOrientedFaceList(inverseDirs,faceDetails);
                initialConstraints
                    .Add(constraintCells
                    .GetInitialConstraint((Vector3Int)cell,"Skybox Cell",orientedFaceList));
            }
        }
        private static void AddGenerationBoundConstraints(ref List<ITesseraInitialConstraint> initialConstraints,ICellInstanceCollection constraintCells,out Cell[] airCellsToUpdate)
        {
            var builder = constraintCells.Builder;
            var voidTile = constraintCells.Database.VoidTile;
            var airTile = constraintCells.Database.AirTile;
            CubeBound generationBound = constraintCells.GenerationBound;

            List<Cell> airCellsToUpdateList = new ();
            foreach (var cell in generationBound)
            {
                if (!constraintCells.TryGetCellInstance((Vector3Int)cell,out var cellInstance,out var localCell) || cellInstance.CellTypes.HasFlags(CellTypes.Void))
                {
                    initialConstraints
                        .Add(builder
                        .GetInitialConstraint(voidTile,(Cell)localCell,null));
                }
                else if (cellInstance.CellTypes.HasFlags(CellTypes.Air))
                {
                    if (cellInstance.Update == true)
                    {
                        airCellsToUpdateList.Add(cell);
                    }
                    initialConstraints
                        .Add(builder
                        .GetInitialConstraint(airTile,(Cell)localCell,null));
                }
                else if (cellInstance.CellTypes.HasFlags(CellTypes.Surface) && !cellInstance.Update)
                {
                    initialConstraints
                        .Add(cellInstance
                        .GetInitialConstraint(builder,(Cell)localCell));
                }
            }
            airCellsToUpdate = airCellsToUpdateList.ToArray();
        }

        public static List<ITesseraInitialConstraint> BuildAirAndVoidConstraints(this CellTypes[][,] cellTypesArray,TesseraInitialConstraintBuilder constraintBuilder,MCTileDatabaseSO database)
        {
            var cellArraySize = new Vector3Int(cellTypesArray[0].GetLength(0),cellTypesArray[0].GetLength(1),cellTypesArray.Length);
            CubeBound cellBound = new(Vector3Int.zero,cellArraySize);

            List<ITesseraInitialConstraint> initialConstraints = new ();
            foreach (var cell in cellBound)
            {
                var cellTypes = cellTypesArray[cell.z][cell.x,cell.y];
                if (cellTypes.HasFlags(CellTypes.Void))
                {
                    initialConstraints
                        .Add(constraintBuilder
                        .GetInitialConstraint(database.VoidTile,cell,null));
                }
                else if (cellTypes.HasFlags(CellTypes.Air))
                {
                    initialConstraints
                        .Add(constraintBuilder
                        .GetInitialConstraint(database.AirTile,cell,null));
                }
            }
            return initialConstraints;
        }
    }
}