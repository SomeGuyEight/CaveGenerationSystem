using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sylves;

namespace SlimeGame
{
    public static class TileHelper
    {
        public static readonly Dictionary<TileTypes,(Directions positive,Directions negative)> TileTypesToStepDirections = new ()
        {
            { TileTypes.Seed              , new ( Directions.Fwd , Directions.Back ) },
            { TileTypes.HorizontalCenter  , new ( Directions.Fwd , Directions.Back ) },
            { TileTypes.VerticalCenter    , new ( Directions.Up  , Directions.Down ) },
            { TileTypes.TransitionFloor   , new ( Directions.Fwd , Directions.Up   ) },
            { TileTypes.TransitionCeiling , new ( Directions.Fwd , Directions.Down ) },
            { TileTypes.HorizontalCap     , new ( Directions.Fwd , Directions.Back ) },
            { TileTypes.VerticalCap       , new ( Directions.Up  , Directions.Down ) },
        };
        public static CellTypes[][,] DefaultCellTypesArrays => new CellTypes[][,]
        {
            new CellTypes[,]
            {
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
            },                                                  
            new CellTypes[,]                                    
            {                                                   
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.AirOrigin   ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
            },                                                  
            new CellTypes[,]                                    
            {                                                   
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
                { CellTypes.FloorSurface ,CellTypes.WallSurface ,CellTypes.CeilingSurface },
            },
        };
        private static (SocketTypes, Directions)[] DefaultSeedSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Origin , Directions.None ),
            new ( SocketTypes.Step   , Directions.Fwd  ),
            new ( SocketTypes.Step   , Directions.Back ),
            new ( SocketTypes.Seed   , Directions.Fwd  ),
            new ( SocketTypes.Seed   , Directions.Back ),
        };
        private static (SocketTypes, Directions)[] DefaultHorizontalCenterSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Fwd  ),
            new ( SocketTypes.Step , Directions.Back ),
        };
        private static (SocketTypes, Directions)[] DefaultHorizontalCapSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Fwd  ),
            new ( SocketTypes.Step , Directions.Back ),
            new ( SocketTypes.Seed , Directions.Fwd  ),
        };
        private static (SocketTypes, Directions)[] DefaultVerticalCenterSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Up   ),
            new ( SocketTypes.Step , Directions.Down ),
        };
        private static (SocketTypes, Directions)[] DefaultVerticalCapSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Up   ),
            new ( SocketTypes.Step , Directions.Down ),
            new ( SocketTypes.Seed , Directions.Up   ),
            new ( SocketTypes.Seed , Directions.Down ),
        };
        private static (SocketTypes, Directions)[] DefaultTransitionFloorSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Fwd ),
            new ( SocketTypes.Step , Directions.Up  ),
        };
        private static (SocketTypes, Directions)[] DefaultTransitionCeilingSockets => new (SocketTypes, Directions)[]
        {
            new ( SocketTypes.Step , Directions.Fwd  ),
            new ( SocketTypes.Step , Directions.Down ),
        };

        public static (SocketTypes socketTypes, Directions directions)[] GetDefaultSocketValues(TileTypes tileTypes)
        {
            return tileTypes switch
            {
                TileTypes.Seed              => DefaultSeedSockets,
                TileTypes.HorizontalCenter  => DefaultHorizontalCenterSockets,
                TileTypes.HorizontalCap     => DefaultHorizontalCapSockets,
                TileTypes.VerticalCenter    => DefaultVerticalCenterSockets,
                TileTypes.VerticalCap       => DefaultVerticalCapSockets,
                TileTypes.TransitionFloor   => DefaultTransitionFloorSockets,
                TileTypes.TransitionCeiling => DefaultTransitionCeilingSockets,
                _                           => new (SocketTypes, Directions)[0]
            };
        }

        public static BaseSocket[] GetDefaultSockets(this TileSO tile)
        {
            var defaultValues = GetDefaultSocketValues(tile.Types);
            var sockets = new BaseSocket[defaultValues.Length];

            for (var i = 0;i < defaultValues.Length;i++)
            {
                var (socketTypes, directions) = defaultValues[i];
                if (TryGetCellFromDirections(tile,CellTypes.Air,directions,out var validCell))
                {
                    sockets[i] = socketTypes.NewSocket(directions,validCell);
                }
            }
            return sockets;
        }

        public static bool TryGetCellFromDirections(TileSO tile,CellTypes cellTypes,Directions directions,out Vector3Int validCell)
        {
            HashSet<Cell> validCells;
            var cells = tile.CellTypesArrays;
            foreach (var cubeBounds in HollowBoundUtils.SearchFromDirections(new(Vector3Int.zero,tile.ArraySize),directions,1))
            {
                validCells = new();
                for (int i = 0;i < cubeBounds.Bounds.Length;i++)
                {
                    var bound = cubeBounds.Bounds[i];
                    foreach (var cell in bound)
                    {
                        if (cells[cell.x,cell.y,cell.z].HasFlags(cellTypes))
                        {
                            validCells.Add(cell);
                        }
                    }
                }

                if (validCells.Count > 0)
                {
                    validCell = GetValidCell(cubeBounds.TargetPosition);
                    return true;
                }
            }

            Debug.Log($"Could not find valid cell for socket -> returning cell (0,0,0) & false");
            validCell = Vector3Int.zero;
            return false;

            Vector3Int GetValidCell(Vector3? targetPosition)
            {
                if (targetPosition == null)
                {
                    return (Vector3Int)validCells.ToArray()[Random.Range(0,validCells.Count)];
                }
                var target = (Vector3)targetPosition;
                List<Cell> nearCells = new (0);
                float? minDistance = null;
                foreach (var cell in validCells)
                {
                    var newDistance = (target - (Vector3Int)cell).magnitude;
                    if (minDistance == null || minDistance > newDistance)
                    {
                        minDistance = newDistance;
                        nearCells = new List<Cell> { cell };
                    }
                    else if (minDistance == newDistance)
                    {
                        nearCells.Add(cell);
                    }
                }
                return (Vector3Int)nearCells[Random.Range(0,nearCells.Count)];
            }
        }

    }
}
