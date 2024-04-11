using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class CellTypesArray : ICellTypesCollection
    {
        public CellTypesArray(TesseraInitialConstraintBuilder builder,MCTileDatabaseSO database,CellTypes[,,] cellTypes,Vector3Int arrayOrigin,CubeBound generationBound)
        {
            _cellTypes = cellTypes;
            _builder = builder;
            _database = database;
            _constraintBound = new(generationBound.min - Vector3Int.one,generationBound.max + Vector3Int.one);
            _indexOffset = arrayOrigin - _constraintBound.min;
        }

        private ICellTypesCollection AsCellTypesCollection => this;
        private readonly CellTypes[,,] _cellTypes;
        private readonly Vector3Int _indexOffset;

        private readonly TesseraInitialConstraintBuilder _builder;
        private readonly MCTileDatabaseSO _database;
        private readonly CubeBound _constraintBound;

        public MCTileDatabaseSO Database { get { return _database; } }
        public TesseraInitialConstraintBuilder Builder { get { return _builder; } }
        public CubeBound ConstraintBound { get { return _constraintBound; } }

        public List<Vector3Int> ZeroOffsetList => new() { Vector3Int.zero };
        public Vector3Int LocalGenerationMin => Vector3Int.zero;
        public Vector3Int LocalGenerationMax => ConstraintBound.size - new Vector3Int(2,2,2);
        public CubeBound LocalGenerationBound => new(LocalGenerationMin,LocalGenerationMax);
        public Vector3Int GenerationMin => ConstraintBound.min + Vector3Int.one;
        public Vector3Int GenerationMax => ConstraintBound.max - Vector3Int.one;
        public Vector3Int GenerationOffset => Vector3Int.zero - GenerationMin;
        public CubeBound GenerationBound => new(GenerationMin,GenerationMax);
        public Vector3Int LocalConstraintMin => -Vector3Int.one;
        public Vector3Int LocalConstraintMax => ConstraintBound.size - Vector3Int.one;
        public CubeBound LocalConstraintBound => new(LocalConstraintMin,LocalConstraintMax);

        private Vector3Int GetIndex(Vector3Int cell) => cell + _indexOffset;

        private bool IsCellInCellTypes(Vector3Int cell,out Vector3Int index,out Vector3Int localCell)
        {
            index = GetIndex(cell);
            localCell = AsCellTypesCollection.GetLocalCell(cell);
            if (index.x > -1 && index.y > -1 && index.z > -1)
            {
                var constraintSize = ConstraintBound.size;
                if (index.x < constraintSize.x && index.y < constraintSize.y && index.z < constraintSize.z)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TryGetCellTypes(Vector3Int cell,out CellTypes cellTypes,out Vector3Int localCell)
        {
            if (IsCellInCellTypes(cell,out var cellsIndex,out localCell))
            {
                cellTypes = _cellTypes[cellsIndex.x,cellsIndex.y,cellsIndex.z];
                return true;
            }
            cellTypes = CellTypes.None;
            return false;
        }
    }
}
