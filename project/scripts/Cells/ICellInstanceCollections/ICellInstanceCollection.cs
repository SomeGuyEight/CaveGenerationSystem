using UnityEngine;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public interface ICellInstanceCollection
    {
        public TesseraInitialConstraintBuilder Builder { get; }
        public CubeBound ConstraintBound { get; }
        public MCTileDatabaseSO Database { get; }

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

        public bool TryGetCellInstance(Vector3Int cell,out CellInstance cellInstance,out Vector3Int localCell);

        public Vector3Int GetLocalCell(Vector3Int cell)
        {
            return cell + GenerationOffset;
        }
        public List<SylvesOrientedFace> GetZeroOffsetOrientedFaceList(CellDir cellDirs,FaceDetails faceDetails)
        {
            return new() { new(Vector3Int.zero,cellDirs,faceDetails) };
        }
        public ITesseraInitialConstraint GetInitialConstraint(Vector3Int cell,string name,List<SylvesOrientedFace> orientedFaces,Sylves.CellRotation? cellRotation = null)
        {
            var localCell = GetLocalCell(cell);
            return Builder.GetInitialConstraint(name + $"{localCell}",orientedFaces,ZeroOffsetList,(Cell)localCell,cellRotation);
        }
    }
}