using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Sylves;
using Tessera;

namespace SlimeGame
{
    public class CellInstance : BaseGenerationInstance
    {
        public CellInstance(InstanceManager manager,MCTileDatabaseSO database,TileInstance parentTile,CellTypes cellTypes,Cell cell,bool update)
        {
            Cell = cell;
            ParentTile = parentTile;
            Update = update;
            Manager = manager;
            CellTypes = cellTypes;
            BaseInitializeProperties(manager,database,(Vector3Int)cell,Vector3Int.one,parentTile);
        }

        private static readonly CubeCellType _cubeType = CubeCellType.Instance;
        private CellTypes _cellTypes;
        private GizmoTypes _enabledGizmos;
        private bool _isPrimaryCell = true;


        [HideInInspector]
        public readonly TileInstance ParentTile;

        public bool Update { get; set; }
        public Cell Cell { get; private set; }
        public bool IsMultiCell { get; private set; }
        public CellTypes CellTypes
        {
            get => _cellTypes;
            set
            {
                _cellTypes = value;
                UpdateEnabledGizmos();
                UpdateGizmoColor();
            }
        }
        public override GizmoTypes GizmoTypes { get { return _enabledGizmos; } }
        public override ColliderTypes ColliderTypes { get { return ColliderTypes.Cell; } }
        public GameObject MeshObject { get; private set; }

        public TesseraTileInstance TesseraInstance { get { return _tesseraInstance; } }
        private TesseraTileInstance _tesseraInstance;
        private int OffsetIndex => _cachedOffsetIndex ?? CacheOffsetIndex();
        private int? _cachedOffsetIndex;
        private int CacheOffsetIndex()
        {
            var cellVector3Int = (Vector3Int)Cell;
            var cells = _tesseraInstance.Cells;
            for (int i = 0;i < cells.Length;i++)
            {
                if (cells[i] == cellVector3Int)
                {
                    _cachedOffsetIndex = i;
                    return i;
                }
            }
            if (_cachedOffsetIndex == null)
            { }
            throw new Exception("offset cell was not in Tessera Tile Instance");
        }
        private List<SylvesOrientedFace> OrientedFaces => _cachedOrientedFaces ?? CacheOrientedFaces();
        private List<SylvesOrientedFace> _cachedOrientedFaces;
        private List<SylvesOrientedFace> CacheOrientedFaces()
        {
            var cellVector3Int = (Vector3Int)Cell;
            var allOrientedFaces = _tesseraInstance.Tile.sylvesFaceDetails;
            var localOffset = _tesseraInstance.Tile.sylvesOffsets[OffsetIndex];
            _cachedOrientedFaces = new(6);
            foreach (var orientedFace in allOrientedFaces.Where(x => x.offset == localOffset))
            {
                var faceDetailClone = orientedFace.faceDetails.DeepClone(FaceType.Square);
                _cachedOrientedFaces.Add(new SylvesOrientedFace(Vector3Int.zero,orientedFace.dir,faceDetailClone));
            }
            return _cachedOrientedFaces;
        }
        
        private CubeBound GetMultiCellBound() => _tesseraInstance?.GetCellBound();
        private Sylves.CellRotation GetCellRotation() => _tesseraInstance != null ? _tesseraInstance.CellRotations[OffsetIndex] : DefaultRotation;

        private bool AreCollidersEnabled() => Manager.IsColliderEnabled(ColliderTypes,IsSelected ? IsSelected : ParentTile.IsSelected);

        /// <summary> 
        /// Returns true if this is not a multiCell 
        /// <br/> -> ( <see cref="IsMultiCell"/> == <see langword="false"/> )
        /// <br/> OR if this is the parent cell of it's multiCell 
        /// <br/> -> ( <see cref="IsMultiCell"/> == <see langword="true"/> &amp;&amp; <see cref="PrimaryCell"/> == <see langword="this"/> )
        /// </summary>
        public MCTile? MCTile => _tesseraInstance != null ? MCTileHelper.GetMCTile(_tesseraInstance.Tile.name) : null;

        /// <summary> 
        /// Returns this CellInstance's <see cref="TesseraTileBase.sylvesFaceDetails"/> from it's <see cref="_tesseraInstance"/> after <see cref="CellRotation"/> has been applied
        /// </summary>
        private FaceDetails GetAppliedFaceDetails(CellDir faceDir)
        {
            var rotation = GetCellRotation();
            var orientedFaces = OrientedFaces;
            for (int i = 0;i < orientedFaces.Count;i++)
            {
                var (dir, faceDetails) = _cubeType.RotateBy(orientedFaces[i].dir,orientedFaces[i].faceDetails,rotation);
                if (dir == faceDir)
                {
                    return faceDetails;
                }
            }
            /// applies a default face to the skybox to mask out the missing interior face inside big tiles
            return Database.DefaultFaceDetails;
        }
        public ITesseraInitialConstraint GetInitialConstraint(TesseraInitialConstraintBuilder builder,Cell offsetCell)
        {
            return builder.GetInitialConstraint(
                    $"({GetType()} {Cell} :: ( Offset Cell {offsetCell} )",
                    OrientedFaces,
                    new() { Vector3Int.zero },
                    offsetCell,
                    GetCellRotation());
        }
        public ITesseraInitialConstraint GetSkyboxConstraint(TesseraInitialConstraintBuilder builder,Cell offsetCell,CellDir faceDir)
        {
            //Debug.Log($"Skybox cell {Cell} ( local {offsetCell} ) CellDir {(CubeDir)faceDir} => applied rotation {GetCellRotation()}");
            return builder.GetInitialConstraint(
                    $"({GetType()} {Cell} :: ( Skybox Cell {offsetCell} )",
                    GetAppliedFaceDetails(faceDir).ToOrientedFaces(faceDir),
                    new() { Vector3Int.zero },
                    offsetCell,
                    Sylves.CubeRotation.Identity);
        }

        protected override void CreateDisplayName()
        {
            if (_tesseraInstance != null)
            {
                var insert = !IsMultiCell ? "" : _isPrimaryCell ? "Primary Multi " : "Secondary Multi ";
                DisplayName = $"{insert} {GetType().Name} ( {Cell} ) - {_tesseraInstance.Tile.name}";
            }
            else
            {
                DisplayName = $"{GetType().Name} {_cellTypes} {Cell}";
            }            
        }
        protected override void CreateCellBound(Vector3Int spawnCell,Vector3Int size) => DefaultCreateCellBound(spawnCell,size);
        protected override void CreateMainObject() 
        {
            if (MainObject != null)
            {
                DestroyAndUnregisterAllObjects();
            }
            var cellPosition = Vector3.Scale((Vector3Int)Cell,CellSize);
            MainObject = new GameObject();
            MainObject.transform.position = cellPosition;
            MainObject.transform.parent = ParentTile.CellHolder.transform;
            MainObject.name = DisplayName + DateStamp + TimeStamp;
        }
        protected override void CreateGizmo() => DefaultCreateGizmo();
        protected override void CreateColliders() => Colliders = new Collider[0];
        protected override void RegisterWithInstanceManager() => Manager.TryRegisterInstance(this);
        private void CreateMeshHolder()
        {
            if (MeshObject != null)
            {
                DestroyAndUnregisterMeshObject();
            }
            if (Colliders != null)
            {
                Colliders = new Collider[0];
            }
            var cellPosition = Vector3.Scale((Vector3Int)Cell,CellSize);
            MeshObject = new GameObject($"{CellTypes} Mesh Object");
            MeshObject.transform.position = cellPosition;
            MeshObject.transform.parent = MainObject.transform;
        }

        public override void UpdateDisplayName()
        {
            CreateDisplayName();
            if (MainObject != null)
            {
                MainObject.name = DisplayName + " " + DateStamp + " " + TimeStamp;
            }
        }
        public override void UpdateGizmoEnabled() 
        {
            DefaultUpdateGizmoEnabled();
            KeepSubCellsSelectedSynced();
        }
        public override void UpdateGizmoDimensions() => DefaultUpdateGizmoDimensions();
        public override void UpdateGizmoColor()
        {
            DefaultUpdateGizmoColor();
            KeepSubCellsSelectedSynced();
        }
        public override void UpdateCollidersEnabled() 
        {
            if (Colliders == null || Colliders.Length < 1)
            {
                return;
            }

            var isEnabled = AreCollidersEnabled();
            for (int i = 0;i < Colliders.Length;i++)
            {
                if (Colliders[i] != null && Colliders[i].enabled != isEnabled)
                {
                    Colliders[i].enabled = isEnabled;
                }
            }
            KeepSubCellsSelectedSynced();
        }
        private void KeepSubCellsSelectedSynced()
        {
            if (!IsMultiCell || !_isPrimaryCell || ParentTile == null)
            {
                return;
            }
            var parentCells = ParentTile.Cells;
            var subCells = _tesseraInstance.Cells;
            for (int i = 1;i < subCells.Length;i++)
            {
                if (parentCells.TryGetValue((Cell)subCells[i],out var cellInstance) && cellInstance != null)
                {
                    if (cellInstance.IsSelected != IsSelected)
                    {
                        Manager.TrySelectMultipleInstance(cellInstance);
                    }
                }
            }
        }
        private void UpdateEnabledGizmos()
        {
            _enabledGizmos = GizmoTypes.Cell;
            if (CellTypes.HasFlags(CellTypes.Void))
            {
                _enabledGizmos |= GizmoTypes.Void;
            }
            if (CellTypes.HasFlags(CellTypes.Air))
            {
                _enabledGizmos |= GizmoTypes.Air;
            }
            if (CellTypes.HasFlags(CellTypes.Surface))
            {
                _enabledGizmos |= GizmoTypes.Surface;
            }
        }
        public void UpdateWithOffsetFromParent(Vector3Int newCell)
        {
            if (_isPrimaryCell && _tesseraInstance != null)
            {
                _tesseraInstance.AlignCellsAndPosition(newCell,CellSize);
            }

            Cell = (Cell)newCell;
            CellBound.min = newCell;
            CellBound.max = newCell + Vector3Int.one;
            UpdateDisplayName();
            UpdateGizmoDimensions();
        }

        public void UpdateNewAirOrVoidCell()
        {
            if (!_isPrimaryCell)
            {
                _isPrimaryCell = true;
            }
            if (IsMultiCell)
            {
                IsMultiCell = false;
            }
            UpdateTesseraInstance(null);
            Colliders = new Collider[0];
            DestroyAndUnregisterMeshObject();
            UpdateDisplayName();
            Update = false;
        }
        public void UpdateToPrimaryCell(TesseraTileInstance newTesseraInstance)
        {
            if (!_isPrimaryCell)
            {
                _isPrimaryCell = true;
            }
            newTesseraInstance.AlignCells((Vector3Int)Cell);
            UpdateTesseraInstance(newTesseraInstance);

            IsMultiCell = newTesseraInstance.Cells.Length > 1;
            if (IsMultiCell)
            {
                HandleNewBigTile();                
            }

            var meshPrefab = IsMultiCell ? newTesseraInstance.Tile.gameObject : Database.GetMeshObject(newTesseraInstance.Tile,CellTypes,out _);
            CreateMeshHolder();
            TesseraGenerator.Instantiate(newTesseraInstance,MeshObject.transform,meshPrefab,false);
            UpdateColliders();
            UpdateDisplayName();
            Update = false;

            void HandleNewBigTile()
            {
                var cells = newTesseraInstance.Cells;
                for (int i = 1;i < cells.Length;i++)
                {
                    if (!ParentTile.Cells.TryGetValue((Cell)cells[i],out var subCellInstance) || subCellInstance == null)
                    {
                        throw new Exception($"No CellInstance at {(Cell)cells[i]} in parent tile when handling new big tile");
                    }
                    subCellInstance.UpdateToSubCell(newTesseraInstance);
                }
            }
        }
        private void UpdateToSubCell(TesseraTileInstance newTesseraInstance)
        {
            if (_isPrimaryCell)
            {
                _isPrimaryCell = false;
            }
            if (!IsMultiCell)
            {
                IsMultiCell = true;
            }
            UpdateTesseraInstance(newTesseraInstance);
            DestroyAndUnregisterMeshObject();
            UpdateDisplayName();
            Update = false;
        }
        private void UpdateTesseraInstance(TesseraTileInstance newTesseraInstance)
        {
            _cachedOffsetIndex = null;
            _cachedOrientedFaces = null;
            _tesseraInstance = newTesseraInstance;
        }
        private void UpdateColliders()
        {
            List<Collider> colliderList = new ();
            var isEnabled = AreCollidersEnabled();
            foreach (var collider in MeshObject.GetComponentsInChildren<Collider>())
            {
                if (collider != null)
                {
                    colliderList.Add(collider);
                    if (collider.enabled != isEnabled)
                    {
                        collider.enabled = isEnabled;
                    }
                    Manager.TryRegisterGameObjectToInstance(collider.gameObject,this);
                }
            }
            Colliders = colliderList.ToArray();
        }

        public override bool TryRelocate(Vector3Int cellOffset)
        {
            if (ParentTile == null)
            {
                Debug.Log("Relocating Cell Instances not implemented");
                return false;
            }
            return ParentTile.TryRelocate(cellOffset);
        }
        public override bool TryTranslate(Vector3Int cellOffset)
        {
            if (ParentTile == null)
            {
                Debug.Log("Translating Cell Instances not implemented");
                return false;
            }
            return ParentTile.TryTranslate(cellOffset);
        }
        public override bool TryResize(Vector2 scrollInput,RaycastHit hit,int length = 1)
        {
            Debug.Log("Resizing Cell Instances not implemented");
            return false;
        }

        public bool TrySetSubCellsToUpdate(out CubeBound multiCellBound)
        {
            if (!IsMultiCell)
            {
                multiCellBound = CellBound;
                return false;
            }
            var allMultiCells = _tesseraInstance.Cells;
            for (int i = 0; i < allMultiCells.Length;i++)
            {
                var cellInstance = ParentTile.Cells[(Cell)allMultiCells[i]];
                if (cellInstance.Update != true)
                {
                    cellInstance.Update = true;
                }
            }
            multiCellBound = GetMultiCellBound();
            return true;
        }
        public void EditSurfaceTypeAndMesh(CellTypes types)
        {
            if (CellTypes != types)
            {
                CellTypes = types;
            }
            if (_tesseraInstance == null || _tesseraInstance.Cells.Length > 1)
            {
                return;
            }

            UpdateDisplayName();
            CreateMeshHolder();
            UpdateGizmoEnabled();

            var meshObject = Database.GetMeshObject(_tesseraInstance.Tile,CellTypes, out _);
            TesseraGenerator.Instantiate(_tesseraInstance,MeshObject.transform,meshObject,false);
            UpdateColliders();
        }


        public override bool TryUnregisterAndDestroy()
        {
            DestroyAndUnregisterAllObjects();
            Manager.TryUnregisterInstance(this);
            return true;
        }
        public void DestroyAndUnregisterAllObjects()
        {
            if (MainObject != null)
            {
                UnregisterCellInstanceObjects();
                GameObject.Destroy(MainObject);
                MainObject = null;
            }
        }
        public void DestroyAndUnregisterMeshObject()
        {
            if (MeshObject != null)
            {
                UnregisterCellInstanceObjects();
                GameObject.Destroy(MeshObject);
                MeshObject = null;
            }
        }
        private void UnregisterCellInstanceObjects()
        {
            if (Colliders == null || Colliders.Length == 0)
            {
                return;
            }
            for (int i = 0; i < Colliders.Length;i++)
            {
                if (Colliders[i] != null)
                {
                    Manager.TryUnregisterGameObjectToInstance(Colliders[i].gameObject);
                }
            }
        }

    }
}