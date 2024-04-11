#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class BigTileGeneratorOptions
    {
        public BigTileGeneratorOptions(MCTileDatabaseSO database,CellInstance[] cellInstances,Vector3Int cellSize,Material meshMaterial,bool isSymmetric,RotationGroupType? rotationType = null)
        {
            Database = database;
            CellInstances = cellInstances;
            CellGrid = new CubeGrid(cellSize);
            MeshMaterial = meshMaterial;
            IsSymetric = isSymmetric;
            RotationType = rotationType ?? RotationGroupType.None;
        }

        public MCTileDatabaseSO Database { get; private set; }
        public CellInstance[] CellInstances { get; private set; }
        public CubeGrid CellGrid { get; private set; }
        public Vector3 CellSize => CellGrid.CellSize;
        public Material MeshMaterial { get; private set; }
        public RotationGroupType RotationType { get; private set; }
        public bool IsSymetric { get; private set; }

        public Dictionary<Vector3Int,TesseraTileInstance> CellToTesseraTileInstances { get; set; }
        public List<Vector3Int> SylvesOffsets { get; set; }
        public List<SylvesOrientedFace> OrientedFaces { get; set; }
    }
}
#endif