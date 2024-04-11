using UnityEngine;
using Tessera;
using Sylves;

namespace SlimeGame
{
    public class CellInstanceArray : ICellInstanceCollection,ICellTypesCollection
    {
        public CellInstanceArray(TesseraInitialConstraintBuilder builder,MCTileDatabaseSO database,CellInstance[,,] cells,Vector3Int arrayOrigin,CubeBound generationBound)
        {
            _cells = cells;
            _builder = builder;
            _database = database;         
            _constraintBound = new(generationBound.min - Vector3Int.one,generationBound.max + Vector3Int.one);
            _indexOffset = arrayOrigin - _constraintBound.min;
        }

        private ICellInstanceCollection AsCellCollection => this;
        /// private ICellTypesCollection AsCellTypesCollection => this;
        private readonly CellInstance[,,] _cells;
        private readonly Vector3Int _indexOffset;

        private readonly TesseraInitialConstraintBuilder _builder;
        private readonly MCTileDatabaseSO _database;
        private readonly CubeBound _constraintBound;

        public MCTileDatabaseSO Database { get { return _database; } }
        public TesseraInitialConstraintBuilder Builder { get { return _builder; } }
        public CubeBound ConstraintBound { get { return _constraintBound; } }

        private Vector3Int GetIndex(Vector3Int cell) => cell + _indexOffset;

        private bool IsCellInCells(Vector3Int cell,out Vector3Int index,out Vector3Int localCell)
        {
            index = GetIndex(cell);
            localCell = AsCellCollection.GetLocalCell(cell);
            if (index.x > -1 && index.y > -1 && index.z > -1) {
                var constraintSize = ConstraintBound.size;
                if (index.x < constraintSize.x && index.y < constraintSize.y && index.z < constraintSize.z) {
                    return true;
                }
            }
            return false;
        }
        public bool TryGetCellInstance(Vector3Int cell,out CellInstance cellInstance,out Vector3Int localCell)
        {
            if (IsCellInCells(cell,out var cellsIndex,out localCell)) 
            {
                cellInstance = _cells[cellsIndex.x,cellsIndex.y,cellsIndex.z];
                if (cellInstance != null) 
                {
                    return true;
                }
            }
            cellInstance = null;
            return false;
        }
        public bool TryGetCellTypes(Vector3Int cell,out CellTypes cellTypes,out Vector3Int localCell)
        {
            if (TryGetCellInstance(cell,out var cellInstance,out localCell))
            {
                cellTypes = cellInstance != null ? cellInstance.CellTypes : CellTypes.Void;
                return true;
            }
            cellTypes = CellTypes.None;
            return false;
        }
    }
}
