using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class CellInstanceDictionary : ICellInstanceCollection,ICellTypesCollection
    {
        public CellInstanceDictionary(TesseraInitialConstraintBuilder builder,MCTileDatabaseSO database,Dictionary<Cell,CellInstance> cells,CubeBound generationBound)
        {
            _cells = cells;
            _builder = builder;
            _database = database;
            _constraintBound = new (generationBound.min - Vector3Int.one,generationBound.max + Vector3Int.one);
        }

        private ICellInstanceCollection AsCellInstanceCollection => this;
        /// private ICellTypesCollection AsCellTypesCollection => this;
        private readonly Dictionary<Cell,CellInstance> _cells;

        private readonly TesseraInitialConstraintBuilder _builder;
        private readonly MCTileDatabaseSO _database;
        private readonly CubeBound _constraintBound;

        public MCTileDatabaseSO Database { get { return _database; } }
        public TesseraInitialConstraintBuilder Builder { get { return _builder; } }
        public CubeBound ConstraintBound { get { return _constraintBound; } }

        public bool TryGetCellInstance(Vector3Int cell,out CellInstance cellInstance,out Vector3Int localCell)
        {
            localCell = AsCellInstanceCollection.GetLocalCell(cell);
            return _cells.TryGetValue((Cell)cell,out cellInstance);
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
