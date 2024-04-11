using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sylves;
using Tessera;

namespace SlimeGame 
{
    public class InstanceGenerator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private readonly GenerationManager _generationManager;

        private bool _generating = false;

        public void GenerateNewAirCell(TileInstance tileInstance,Vector3 position) 
        {
            var cell = tileInstance.CellGrid.FindCell(position);
            if(cell != null && !_generating) 
            {
                StartCoroutine(GenerateNewAirCell(tileInstance,(Cell)cell));
            }
        }
        public IEnumerator GenerateNewAirCell(TileInstance tileInstance,Cell cell)
        {
            if(!tileInstance.Cells.TryGetValue(cell,out var cellInstance)) 
            {
                cellInstance = tileInstance.InitializeCellInstance(cell,CellTypes.Air);
                tileInstance.Cells.Add(cell, cellInstance);
            } 
            else if(cellInstance.CellTypes.HasFlags(CellTypes.Air) == false) 
            {
                cellInstance.CellTypes = CellTypes.Air;
                cellInstance.Update = true;
            }
            var generationBound = GenerateCoreCell(_generationManager,tileInstance,cell);
            tileInstance.AddNewBoundAndUpdateTile(generationBound);
            _generating = true;
            yield return StartCoroutine(GenerateInstanceBounds(_generationManager,tileInstance,generationBound));
            _generating = false;
        }        
        private static CubeBound GenerateCoreCell(GenerationManager manager,TileInstance tileInstance,Cell newAirCell)
        {
            var minNeighborCell = (Vector3Int)newAirCell - Vector3Int.one;
            CubeBound cellBound = new(minNeighborCell,minNeighborCell + new Vector3Int(3,3,3));

            foreach (var cell in cellBound)
            {
                var cellTypes = cell == newAirCell ? CellTypes.Air : CellTypes.Surface;
                if (tileInstance.Cells.TryGetValue(cell,out CellInstance cellInstance))
                {
                    if (ShouldUpdateCellTypes(cellTypes,cellInstance.CellTypes))
                    {
                        cellInstance.CellTypes = cellTypes;
                    }
                    else if (cellInstance.CellTypes.HasFlags(CellTypes.Air))
                    {
                        continue;
                    }
                    /// here b/c surface cells around a new Air Cell need to update regardless of CellSubTypes change
                    cellInstance.Update = true;
                }
                else
                {
                    tileInstance.InitializeCellInstance(cell,cellTypes);
                }
            }
            FollowAirCellCollapse(ref cellBound,manager,tileInstance,new [] { tileInstance.Cells[newAirCell] });
            return cellBound;

            static bool ShouldUpdateCellTypes(CellTypes newTypes,CellTypes oldTypes)
            {
                if (oldTypes.HasFlags(CellTypes.Air) && (newTypes.HasFlags(CellTypes.Surface) || newTypes.HasFlags(CellTypes.Void)))
                {
                    return false;
                }
                if (newTypes == oldTypes)
                {
                    return false;
                }
                return true;
            }
        }
        private static bool ShouldUpdateCellTypes(CellTypes newTypes,CellTypes oldTypes,out CellTypes updatedTypes) 
        {
            if((newTypes.HasFlags(CellTypes.Surface) && oldTypes.HasFlags(CellTypes.Air)) || (newTypes == oldTypes)) 
            {
                updatedTypes = oldTypes;
                return false;
            }
            if(newTypes.HasFlags(CellTypes.Air) && oldTypes.HasFlags(CellTypes.Surface)) 
            {
                updatedTypes = newTypes;
                return true;
            }
            updatedTypes = newTypes | oldTypes;
            return true;
        }
        private static void FollowAirCellCollapse(ref CubeBound collapsedBound,GenerationManager manager,TileInstance tileInstance,CellInstance[] newAirCells) 
        {
            while(newAirCells.Length > 0)
            {
                var cellsCollapsedToAir = new HashSet<CellInstance>();
                for(int i = 0; i < newAirCells.Length; i++) 
                {
                    var newCoreCellInstance =  newAirCells[i];
                    newCoreCellInstance.CellTypes = CellTypes.Air;
                    newCoreCellInstance.Update = true;

                    var newCoreCell = (Vector3Int)newCoreCellInstance.Cell;
                    var min = newCoreCell - Vector3Int.one;
                    var max = min + new Vector3Int(3,3,3);
                    var newBound = new CubeBound(min,max);
                    collapsedBound = collapsedBound.Union(newBound);

                    UpdateNewAirCellNeighbors(manager,tileInstance,newCoreCell);
                    var tempCells = CollapseNeighborCellsToAir(tileInstance,newBound);
                    if(tempCells.Count > 0)
                    {
                        foreach(var collapsedInstance in tempCells) 
                        {
                            cellsCollapsedToAir.Add(collapsedInstance);
                        }
                    }
                }
                newAirCells = cellsCollapsedToAir.ToArray();
            }
        }
        private static void UpdateNewAirCellNeighbors(GenerationManager manager,TileInstance tileInstance,Vector3Int newAirCell) 
        {
            var defaultNeighborCellTypes = manager.DefaultNeighborCellTypes;
            var minNeighborCell = newAirCell - Vector3Int.one;
            CubeBound neighborsBound = new(minNeighborCell,minNeighborCell + new Vector3Int(3,3,3));
            foreach (var neighborCell in neighborsBound) 
            {
                var index = minNeighborCell - neighborCell;
                CellTypes newTypes = defaultNeighborCellTypes[index.x,index.y,index.z];
                if (tileInstance.Cells.TryGetValue(neighborCell,out var neighborCellInstance)) 
                {
                    if (ShouldUpdateCellTypes(newTypes,neighborCellInstance.CellTypes,out var updatedTypes))
                    {
                        neighborCellInstance.CellTypes = updatedTypes;
                    }
                    neighborCellInstance.Update = true;
                }
                else 
                {
                    throw new Exception("Cell was collapsed to Air Cell, but has contradicting neighborCellInstance cells (no tileInstance)");
                }
            }
        }
        private static HashSet<CellInstance> CollapseNeighborCellsToAir(TileInstance tileInstance,CubeBound cellBound)
        {        
            var collapsedToAirCells = new HashSet<CellInstance>();      
            foreach (var cell in cellBound) 
            {
                if (!tileInstance.Cells.TryGetValue(cell,out var cellInstance) || cellInstance.CellTypes.HasFlags(CellTypes.Air)) 
                {
                    continue;
                }
                if (!HasVoidNeighbor(cellInstance))
                {
                    collapsedToAirCells.Add(cellInstance);
                }
            }
            return collapsedToAirCells;

            bool HasVoidNeighbor(CellInstance cellInstance)
            {
                var minNeighborCell = (Vector3Int)cellInstance.Cell - Vector3Int.one;
                CubeBound neighborsBound = new(minNeighborCell,minNeighborCell + new Vector3Int(3,3,3));
                foreach (var cell in neighborsBound)
                {
                    if (!tileInstance.Cells.TryGetValue(cell,out var neighborCellInstance) || neighborCellInstance.CellTypes.HasFlags(CellTypes.Void))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public static IEnumerator RegenerateInstanceBounds(GenerationManager generationManager,TileInstance tileInstance,CubeBound generatorBounds,TesseraGenerator surfaceGenerator = null,bool returnStats = true) 
        {
            SetSurfaceCellsToUpdate(tileInstance,generatorBounds);
            yield return generationManager.StartCoroutine(GenerateInstanceBounds(generationManager,tileInstance,generatorBounds,surfaceGenerator,returnStats));
        }
        private static void SetSurfaceCellsToUpdate(TileInstance tileInstance,CubeBound cellBoundToUpdate) 
        {
            var cells = tileInstance.Cells;
            foreach(var cell in cellBoundToUpdate)
            { 
                if(cells.TryGetValue(cell,out var cellInstance)) 
                {
                    if(cellInstance.IsMultiCell == true)
                    {
                        /// b/c multi will be updated in <see cref="GenerateInstanceBounds"/> -> <see cref="HandleMultiCellsInGenerationBound"/>
                        continue;
                    }
                    cellInstance.Update = true;
                }
            }
        }

        public static IEnumerator GenerateInstanceBounds(GenerationManager generationManager,TileInstance tileInstance,CubeBound generationBound,TesseraGenerator surfaceGenerator = null,bool returnStats = true)
        {
            generationBound = HandleMultiCellsInGenerationBound(tileInstance,generationBound);

            var generator = surfaceGenerator != null ? surfaceGenerator : generationManager.SurfaceGenerator;
            var center = generationBound.min + Vector3.Scale(generationBound.size,new Vector3(.5f,.5f,.5f));
            generator.bounds = new(center,generationBound.size);

            CellInstanceDictionary constraintCells = new (generationManager.ConstraintBuilder,generationManager.Database,tileInstance.Cells,generationBound);
            var constraints = constraintCells.BuildGenerationConstraints(out var airCellsToUpdate);

            TesseraCompletion completion = null;
            TesseraStats stats = null;
            if (generationManager.DebugMode.HasFlags(GenerationDebugMode.SaveGenerationStats)) 
            {
                yield return generator.StartGenerate(new TesseraGenerateOptions
                {
                    onComplete = c => completion = c,
                    returnStats = s => stats = s,
                    initialConstraints = constraints,
                });
                generationManager.AddGenerationStats(stats,completion,generationBound);
            }
            else 
            {
                yield return generator.StartGenerate(new TesseraGenerateOptions 
                {
                    onComplete = c => completion = c,
                    initialConstraints = constraints,
                });
            }

            if (completion.success)
            {
                UpdateAirCellsAfterGeneration(tileInstance,airCellsToUpdate);
                InstantiateCompletion(generationManager,tileInstance,completion,generationBound,out var cellsConvertedToAir);
                HandleNewAirFromInstantiation(tileInstance,cellsConvertedToAir);
                tileInstance.TryUpdateAllSocketPositions();
                if (generationManager.DebugMode.HasFlags(GenerationDebugMode.ConstraintDebugOnSuccess))
                {
                    var debugObject = constraints.CreateDebugObject("Successful Generation Constraints",generationManager.Database.Palette,generationBound,tileInstance.CellSize);
                    generationManager.EnqueueConstraintDebugObject(debugObject);
                }
            }
            else
            {
                Debug.Log("Failed Generation");
                if (generationManager.DebugMode.HasFlags(GenerationDebugMode.ConstraintDebugOnFail))
                {
                    var debugObject = constraints.CreateDebugObject("Failed Generation Constraints",generationManager.Database.Palette,generationBound,tileInstance.CellSize);
                    generationManager.EnqueueConstraintDebugObject(debugObject);
                }
            }
        }
        private static CubeBound HandleMultiCellsInGenerationBound(TileInstance tileInstance,CubeBound generationBound)
        {
            CubeBound updatedBounds = new (generationBound.min,generationBound.max);
            foreach (var cell in generationBound)
            {
                if (tileInstance.Cells.TryGetValue(cell,out var cellInstance) && cellInstance != null && cellInstance.IsMultiCell)
                {
                    if (cellInstance.TrySetSubCellsToUpdate(out var multiCellBound))
                    {
                        updatedBounds = updatedBounds.Union(multiCellBound);
                    }
                }
            }
            return updatedBounds;
        }
        private static void UpdateAirCellsAfterGeneration(TileInstance tileInstance,Cell[] airCellsToUpdate)
        {
            for (var i = 0;i < airCellsToUpdate.Length;i++)
            {
                if (tileInstance.Cells.TryGetValue(airCellsToUpdate[i],out var cellInstance))
                {
                    cellInstance.UpdateNewAirOrVoidCell();
                }
            }
        }
        private static void InstantiateCompletion(GenerationManager manager,TileInstance tileInstance,TesseraCompletion completion,CubeBound generationBound,out CellInstance[] cellsConvertedToAir) 
        {
            var min = generationBound.min;
            var database = manager.Database;
            var convertedToAir = new List<CellInstance>();
            for(int i = 0; i < completion.tileInstances.Count; i++) 
            {
                var tesseraTileInstance = completion.tileInstances[i];
                var cellInWorldSpace = tesseraTileInstance.Cell + min;
                if(!tileInstance.Cells.TryGetValue((Cell)cellInWorldSpace,out var cellInstance) || cellInstance.CellTypes.HasFlags(CellTypes.Void)) 
                {
                    continue;
                }

                if (database.IsAirTile(tesseraTileInstance.Tile) && !cellInstance.CellTypes.HasFlags(CellTypes.Air))
                {
                    /// TODO: conversion methods
                    /// -> method to convert cellInstance to Void, air, or surface
                    Debug.Log($"{cellInstance.Cell} was not an air cell, but had an air tile => converted to air from {cellInstance.CellTypes}");
                    cellInstance.CellTypes = CellTypes.Air;
                    cellInstance.UpdateGizmoColor();
                    cellInstance.UpdateGizmoDimensions();
                    cellInstance.UpdateNewAirOrVoidCell();
                    convertedToAir.Add(cellInstance);
                }

                if (tesseraTileInstance.Cells.Length == 1)
                {
                    if (!MCTileDatabaseSO.AreTileAndCellTypesSynced(tesseraTileInstance.Tile,cellInstance.CellTypes,out var actualCellTypes))
                    {
                        /// TODO: add method to correct
                        Debug.Log($"{cellInstance.Cell} CellSubTypes were not synced => {cellInstance.CellTypes} converted to {actualCellTypes}");
                        cellInstance.CellTypes = actualCellTypes;
                    }
                    if (MCTileHelper.IsDefaultMCTile(tesseraTileInstance.Tile.name,out var mCtile) && mCtile == MCTile._41_0001_0111)
                    {
                        cellInstance.CellTypes = Get41CornerCellTypes(tesseraTileInstance.Rotation);
                    }
                    else
                    {
                        cellInstance.CellTypes = GetDefaultCellTypes(tileInstance.Cells,(Vector3Int)cellInstance.Cell);
                    }
                }
                else
                {
                    cellInstance.CellTypes = GetDefaultCellTypes(tileInstance.Cells,(Vector3Int)cellInstance.Cell);
                }

                cellInstance.UpdateToPrimaryCell(tesseraTileInstance);
            }

            cellsConvertedToAir = convertedToAir.ToArray();
        }
        /// <summary>
        /// TODO: Find a better way to handle this
        /// <br/> ( ! ) Needed to make sure all Cell surfaces are painted to reflect proximity to core cell
        /// <br/> &amp; needed to ensure CellSubTypes are accurately reflected when building constraints
        /// </summary>
        private static void HandleNewAirFromInstantiation(TileInstance tileInstance,CellInstance[] cellsConvertedToAir)
        {
            for (int i = 0;i < cellsConvertedToAir.Length;i++)
            {
                var minNeighborCell = (Vector3Int)cellsConvertedToAir[i].Cell - Vector3Int.one;
                CubeBound neighborsBound = new(minNeighborCell,minNeighborCell + new Vector3Int(3,3,3));
                foreach (var neighborCell in neighborsBound)
                {
                    var neighborInstance = tileInstance.Cells[neighborCell];
                    CellTypes newTypes;
                    if (neighborInstance.MCTile == MCTile._41_0001_0111)
                    {
                        newTypes = Get41CornerCellTypes(neighborInstance.TesseraInstance.Rotation);
                    }
                    else
                    {
                        newTypes = GetDefaultCellTypes(tileInstance.Cells,(Vector3Int)neighborCell);
                    }
                    if (neighborInstance.IsMultiCell == true)
                    {
                        neighborInstance.UpdateDisplayName();
                        continue;
                    }
                    neighborInstance.EditSurfaceTypeAndMesh(newTypes);
                }
            }
        }
        /// <summary>
        /// This can become pretty perfomant:
        /// <br/> -> previously faster methods implemented, but the floor/ceiling/wall surfaces had mistakes
        /// <br/> -> this implementation gets correct type everytime at the cost of more Dictionary.TryGetValue()
        /// </summary>
        private static CellTypes GetDefaultCellTypes(Dictionary<Cell,CellInstance> cells,Vector3Int cell)
        {
            Cell cellAbove = new (cell.x,cell.y + 1,cell.z);
            if (!cells.TryGetValue(cellAbove,out var instanceAbove) || instanceAbove.CellTypes.HasFlags(CellTypes.Void))
            {
                return CellTypes.CeilingSurface;
            }
            else if (instanceAbove.CellTypes.HasFlags(CellTypes.Air))
            {
                return CellTypes.FloorSurface;
            }

            Cell cellBelow = new(cell.x,cell.y - 1,cell.z);
            if (!cells.TryGetValue(cellBelow,out var instanceBelow) || instanceBelow.CellTypes.HasFlags(CellTypes.Void))
            {
                return CellTypes.FloorSurface;
            }
            return instanceBelow.CellTypes.HasFlags(CellTypes.Air) ? CellTypes.CeilingSurface : CellTypes.WallSurface;
        }
        /// <summary>
        /// TODO: Refine b/c this relies on the default MCTile:
        /// <br/> -> can't be applied without the MCTile reference 
        /// <br/> -> will not work if the default MCTile for _41 is changed
        /// </summary>
        private static CellTypes Get41CornerCellTypes(Quaternion rotation)
        {
            var eulers = rotation.eulerAngles;
            if ((eulers.x > 89 && eulers.x < 91) || (eulers.y > 179 && eulers.y < 181 && (eulers.z > 95 || eulers.z < 85)))
            {
                return CellTypes.FloorSurface;
            }
            return CellTypes.CeilingSurface;
        }

    }
}