using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class CellTypesDictionary : ICellTypesCollection
    {
        public CellTypesDictionary(TesseraInitialConstraintBuilder builder,MCTileDatabaseSO database,Dictionary<Cell,CellTypes> cells,CubeBound generationBound)
        {
            _cellTypes = cells;
            _builder = builder;
            _database = database;
            _constraintBound = new(generationBound.min - Vector3Int.one,generationBound.max + Vector3Int.one);
        }

        private ICellTypesCollection AsCellTypesCollection => this;
        private readonly Dictionary<Cell,CellTypes> _cellTypes;

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

        public bool TryGetCellTypes(Vector3Int cell,out CellTypes cellTypes,out Vector3Int localCell)
        {
            localCell = AsCellTypesCollection.GetLocalCell(cell);
            return _cellTypes.TryGetValue((Cell)cell,out cellTypes);
        }
    }
}
