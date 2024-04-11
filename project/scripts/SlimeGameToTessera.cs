using UnityEngine;
using System;
using System.Collections.Generic;
using Tessera;

namespace SlimeGame
{
    public class SlimeGameToTessera
    {


        #region Tessera Tile Instance

        /// <see cref="TesseraTileInstance"/> => from internal set => public set
        ///     => <see cref="TesseraTileInstance.Cell"/>
        ///     => <see cref="TesseraTileInstance.LocalPosition"/>
        ///     => <see cref="TesseraTileInstance.Position"/>  
        //
        ///     Position & LocalPosition => implemented by
        ///         1. <see cref="TesseraUtils.AlignCellsAndPosition"/>
        ///             => <see cref="TesseraUtils.AlignPosition"/>
        ///         2. <see cref="BigTileGenerator.InitializeCellToTesseraTileInstances"/>
        ///     Cell => implemented by 
        ///         1. <see cref="TesseraUtils.AlignCellsAndPosition"/>
        ///             => <see cref="TesseraUtils.AlignCells"/>
        ///             

        #endregion


        #region Tessera Initial Constraint Builder

        /// <see cref="TesseraInitialConstraintBuilder"/> new method 
        ///     => <see cref="GetInitialConstraint"/>
        //
        ///     => implemented by
        ///         1. <see cref="CellInstance.GetInitialConstraint"/>
        ///         2. <see cref="CellInstance.GetSkyboxConstraint"/>
        ///         3. <see cref="ICellInstanceCollection.GetInitialConstraint"/>
        ///         4. <see cref="ICellTypesCollection.GetInitialConstraint"/>

        #region TesseraInitialConstraintBuilder modification => new method GetInitialConstraint

        /// <see cref="TesseraInitialConstraintBuilder"/>>
        public void GetInitialConstraint()
        //public TesseraInitialConstraint GetInitialConstraint(string name,List<SylvesOrientedFace> faceDetails,List<Vector3Int> sylvesOffsets,Sylves.Cell cell,Sylves.CellRotation? cellRotation)
        {
            //var rotation = cellRotation ?? grid.GetCellType(cell).GetIdentity();
            //// TODO: Needs support for big tiles
            //return new TesseraInitialConstraint
            //{
            //    name = name,
            //    faceDetails = faceDetails,
            //    offsets = sylvesOffsets,
            //    cell = (Vector3Int)cell,
            //    rotation = rotation,
            //};
        }

        #endregion
        #endregion


        #region Tessera Tile Base

        /// <see cref="TesseraTileBase"/> => new method 
        ///     => <see cref="OverrideOffsetsAndOrientedFaces"/>
        //
        ///     => implemented by:
        ///         1. <see cref="MCTileDatabaseGenerator.GenerateNewFullDatabase"/>
        ///             => <see cref="MCTileDatabaseGenerator.CreateTesseraTiles"/>
        ///         2. <see cref="GenerationManager.SaveNewBigTileButton"/>
        ///             => <see cref="GenerationManager.SaveNewBigTile"/>
        ///                 => <see cref="BigTileGenerator.SaveAsNewBigTile"/>
        ///                     => <see cref="BigTileGenerator.SaveBigTileAsPrefab"/>
        ///         3. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        ///             => <see cref="GenerationUtils.CreateDebugObject"/>
        ///

        #region TesseraTileBase modification => new method

        /// add the following method to <see cref="TesseraTileBase"/>
        public void OverrideOffsetsAndOrientedFaces(List<Vector3Int> offsets,List<SylvesOrientedFace> orientedFaces)
        {
            //if (sylvesOffsets != null)
            //{
            //    sylvesOffsets.Clear();
            //    sylvesOffsets = null;
            //}
            //sylvesOffsets = offsets;

            //if (sylvesFaceDetails != null)
            //{
            //    sylvesFaceDetails.Clear();
            //    sylvesFaceDetails = null;
            //}
            //sylvesFaceDetails = orientedFaces;
        }

        #endregion
        #endregion


        #region Tessera Stats

        /// <see cref="TesseraStats"/> => from internal => public
        //
        ///     => implemented by 
        ///         1. <see cref="FullGenerationStats"/>
        ///         2. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        ///             => <see cref="GenerationManager.AddGenerationStats"/>
        ///         3. <see cref="GenerationManager.SaveFullGenerationStatsToExcel"/>
        ///             => <see cref="StatsCSVWriter.SaveCompletionAndStatsCSV"/>
        ///

        #region Tessera Generate Options

        /// <see cref="TesseraGenerateOptions"/> => add action
        ///     => public Action<TesseraStats> returnStats;
        // 
        ///     => implemented by 
        ///         1. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        /// 

        #endregion
        #endregion


        #region Tessera Tile Instance

        /// <see cref="ITesseraInitialConstraint"/> => implement public getters for => Cell, CellRotation, & FaceDetails
        ///     => <see cref="_ITesseraInitialConstraint"/>
        ///     => <see cref="_TesseraInitialConstraint"/>
        ///     => <see cref="_TesseraVolumeFilter"/>
        ///     => <see cref="_TesseraPinConstraint"/>
        //
        ///     => implemented by
        ///         1. <see cref="InstanceGenerator.GenerateInstanceBounds"/>
        ///             => needs <see cref="GenerationUtils.CreateDebugObject"/>
        /// 

        #region ITesseraInitialConstraint modification => new public getters
        /// ( !! ) Important: Adding these open up the references, but the methods implemented do not change any of the values
        ///     >> having the references may lead to 'mutations' if implemented beyond what is here
        ///     >> be careful not to assign to the references if accessing elsewhere
        /// => Add the following getters and setters to 
        ///     <see cref="ITesseraInitialConstraint"/>
        ///     <see cref="TesseraInitialConstraint"/>  
        ///     <see cref="TesseraVolumeFilter"/>
        ///     <see cref="TesseraPinConstraint"/>
        ///     
#pragma warning disable
        public interface _ITesseraInitialConstraint
        {
            /// <see cref="ITesseraInitialConstraint"/>
            //public Vector3Int? Cell { get; }
            //public List<SylvesOrientedFace> FaceDetails { get; }
            //public List<Vector3Int> Offsets { get; }
            //public Sylves.CellRotation CellRotation { get; }
        }

        /// <summary>
        /// Initial constraint objects fix parts of the generation process in places.
        /// Use the utility methods on <see cref="TesseraGenerator"/> to create these objects.
        /// </summary>
        [Serializable]
        public class _TesseraInitialConstraint : _ITesseraInitialConstraint
        {
            /// <see cref="TesseraInitialConstraint"/>
            //public Vector3Int? Cell { get { return cell; } }
            //public List<SylvesOrientedFace> FaceDetails { get { return faceDetails; } }
            //public List<Vector3Int> Offsets { get { return offsets; } }
            //public Sylves.CellRotation CellRotation { get { return rotation; } }
        }
        
        public class _TesseraVolumeFilter : _ITesseraInitialConstraint
        {
            /// <see cref="TesseraVolumeFilter"/>
            //public Vector3Int? Cell { get { return (Vector3Int)cells[0]; } }
            //public List<SylvesOrientedFace> FaceDetails { get { throw new Exception(); } }
            //public List<Vector3Int> Offsets { get { throw new Exception(); } }
            //public Sylves.CellRotation CellRotation { get { throw new Exception(); } }
        }

        public class _TesseraPinConstraint : _ITesseraInitialConstraint
        {
            /// <see cref="TesseraPinConstraint"/>
            //public Vector3Int? Cell { get { return cell; } }
            //public List<SylvesOrientedFace> FaceDetails { get { return tile.sylvesFaceDetails; } }
            //public List<Vector3Int> Offsets { get { return tile.sylvesOffsets; } }
            //public Sylves.CellRotation CellRotation { get { return rotation; } }
        }
#pragma warning restore
        #endregion
        #endregion


    }
}
