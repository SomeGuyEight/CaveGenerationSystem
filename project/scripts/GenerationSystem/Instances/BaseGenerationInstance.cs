using UnityEngine;
using Sirenix.OdinInspector;
using Sylves;

namespace SlimeGame
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class BaseGenerationInstance
    {
        /// <summary>
        /// Call in constructors of classes inheriting from <see cref="BaseGenerationInstance"/>.
        /// <br/><br/> ( ! ) Keep here &amp; not in a constructor:
        /// <br/> -> inherited classes need fields &amp; properties set b/c this needs them.
        /// </summary>
        protected void BaseInitializeProperties(InstanceManager manager,MCTileDatabaseSO database,Vector3Int spawnCell,Vector3Int boundSize,BaseGenerationInstance parent = null)
        {
            Parent = parent;
            Manager = manager;
            Database = database;
            CellGrid = manager.CellGrid;

            DefaultCreateDateAndTimeStamps();
            CreateCellBound(spawnCell,boundSize);

            CreateDisplayName();
            CreateMainObject();

            CreateGizmo();
            CreateColliders();

            RegisterWithInstanceManager();
        }

        public BaseGenerationInstance Parent { get; private set; }
        public InstanceManager Manager { get; protected set; }
        public MCTileDatabaseSO Database { get; protected set; }
        public CubeGrid CellGrid { get; protected set; }
        [ShowInInspector, ReadOnly]
        public string DisplayName { get; protected set; }
        protected string DateStamp { get; private set; }
        protected string TimeStamp { get; private set; }
        [ShowInInspector, ReadOnly]
        public bool IsSelected { get; set; }
        [ShowInInspector, ReadOnly]
        public GameObject MainObject { get; protected set; }
        [ShowInInspector, ReadOnly]
        public CubeGizmoComponent Gizmo { get; protected set; }
        [ShowInInspector, ReadOnly]
        public Collider[] Colliders { get; protected set; }
        [ShowInInspector, ReadOnly]
        public CubeBound CellBound { get; protected set; }


        protected static Vector3 HalfVector3 => new(.5f,.5f,.5f);
        protected static CellRotation DefaultRotation => (CellRotation)CubeRotation.Identity;
        protected Color GizmoColor => Manager.GetGizmoColor(GizmoTypes,IsSelected);

        public Vector3 CellSize => CellGrid.CellSize;
        public Vector3Int BoundOffset => Vector3Int.zero - CellBound.min;     
        public Vector3 CellCenter => Vector3.Scale(CellSize,HalfVector3);
        public Vector3 LocalCenter => Vector3.Scale(WorldSize,HalfVector3);
        public Vector3Int LocalCenterCell => Vector3Int.FloorToInt(Vector3.Scale(CellBound.size,HalfVector3));  
        public Vector3Int CenterCell => CellBound.min + LocalCenterCell;     
        public Vector3 WorldSize => Vector3.Scale(CellBound.size,CellSize);
        public Vector3 WorldMin => Vector3.Scale(CellBound.min,CellSize);
        public Vector3 WorldMax => Vector3.Scale(CellBound.max,CellSize);
        public Vector3 WorldCenter => WorldMin + LocalCenter;
        public Bounds WorldBounds => new (WorldCenter,WorldSize);

        public abstract ColliderTypes ColliderTypes { get; }
        public abstract GizmoTypes GizmoTypes { get; }

        protected abstract void CreateCellBound(Vector3Int spawnCell,Vector3Int size);
        protected abstract void CreateDisplayName();
        protected abstract void CreateMainObject();
        protected abstract void CreateGizmo();
        protected abstract void CreateColliders();
        protected abstract void RegisterWithInstanceManager();
        public abstract void UpdateDisplayName();
        public abstract void UpdateGizmoEnabled();
        public abstract void UpdateGizmoDimensions();
        public abstract void UpdateGizmoColor();
        public abstract void UpdateCollidersEnabled();
        public abstract bool TryRelocate(Vector3Int newCenterCell);
        public abstract bool TryTranslate(Vector3Int cellOffset);
        public abstract bool TryResize(Vector2 scrollInput,RaycastHit hit,int length = 1);
        public abstract bool TryUnregisterAndDestroy();

        // Default methods => create instance & components
        protected void DefaultCreateDateAndTimeStamps()
        {
            DateStamp = SGUtils.DateStamp();
            TimeStamp = SGUtils.TimeStamp();
        }
        protected void DefaultCreateCellBound(Vector3Int spawnCell,Vector3Int size)
        {
            var center = Vector3.Scale(size,HalfVector3);
            var cellOffsetToMin = Vector3Int.FloorToInt(center);
            var min = spawnCell - cellOffsetToMin;
            CellBound = new CubeBound(min,min + size);
        }
        protected void DefaultCreateDisplayName()
        {
            DisplayName = $"({GetType().Name}) {"{"} Size {CellBound.size} ( Min {CellBound.min} :: Max {CellBound.max} ) {"}"}";
        }
        protected void DefaultCreateMainObject()
        {
            MainObject = new GameObject($"{DisplayName} {DateStamp} {TimeStamp}");
            MainObject.transform.position = WorldMin;
        }
        protected void DefaultCreateGizmo()
        {
            var color = Manager.GetGizmoColor(GizmoTypes,IsSelected);
            CubeGizmoComponent.TryAddCubeGizmo(MainObject,CellBound,CellSize,color,out var newGizmo);
            Gizmo = newGizmo;
            Gizmo.SetEnabled(Manager.IsGizmoEnabled(GizmoTypes,IsSelected));
        }
        protected void DefaultCreateColliders()
        {
            var collider = MainObject.AddComponent<BoxCollider>();
            var worldBounds = WorldBounds;
            collider.center = worldBounds.center - worldBounds.min;
            collider.size = worldBounds.size - new Vector3(.02f,.02f,.02f);
            collider.isTrigger = true;
            collider.enabled = Manager.IsColliderEnabled(ColliderTypes,IsSelected);
            Colliders = new [] { collider };
        }
        protected void DefaultRegisterInstanceWithManager()
        {
            Manager.TryRegisterGameObjectToInstance(MainObject,this);
            Manager.TryRegisterInstance(this);
        }

        // Default methods => update instance
        protected void DefaultUpdateDisplayName()
        {
            if (CellBound != null) 
            {
                DisplayName = $"({GetType().Name}) Size {CellBound.size} :: Min {CellBound.min} :: Max {CellBound.max}";
                if (MainObject != null) 
                {
                    MainObject.name = $"{DisplayName} {DateStamp} {TimeStamp}";
                }
            }
        }
        protected void DefaultUpdateGizmoEnabled()
        {
            var isEnabled = Manager.IsGizmoEnabled(GizmoTypes,IsSelected);
            if (Gizmo != null && Gizmo.IsEnabled != isEnabled)
            {
                Gizmo.SetEnabled(isEnabled);
            }
        }
        protected void DefaultUpdateGizmoDimensions()
        {
            if (Gizmo != null)
            {
                Gizmo.UpdateBounds(WorldBounds);
            }
        }
        protected void DefaultUpdateGizmoColor()
        {
            if (Gizmo != null)
            {
                Gizmo.UpdateColor(GizmoColor);
            }
        }
        protected void DefaultUpdateColliderEnabled()
        {
            if (Colliders == null)
            {
                return;
            }

            var isEnabled = Manager.IsColliderEnabled(ColliderTypes,IsSelected);
            for (int i = 0;i < Colliders.Length;i++)
            {
                if (Colliders[i] != null && Colliders[i].enabled != isEnabled)
                {
                    Colliders[i].enabled = isEnabled;
                }
            }
        }
        protected void DefaultUpdateColliderDimensions()
        {
            if (Colliders == null) 
            {
                return;
            }
            for (int i = 0;i < Colliders.Length;i++)
            {
                if (Colliders[i] is BoxCollider boxCollider)
                {
                    boxCollider.center = LocalCenter;
                    boxCollider.size = WorldSize - new Vector3(.02f,.02f,.02f);
                }
            }
        }

        // Default methods => change grid position of instance
        protected bool DefaultTryRelocate(Vector3Int newCenterCell)
        {
            if (CellBound == null || MainObject == null) 
            {
                return false;
            }

            var centerCell = CenterCell;
            var cellOffset = newCenterCell - centerCell;
            var newMinCell = CellBound.min + cellOffset;
            CellBound = new(newMinCell,newMinCell + CellBound.size);

            var worldOffset = Vector3.Scale(cellOffset,CellSize);
            var newPosition = MainObject.transform.position + worldOffset;
            MainObject.transform.position = newPosition;

            DefaultUpdateDisplayName();
            UpdateGizmoDimensions();
            DefaultUpdateColliderDimensions();

            return true;
        }
        protected bool DefaultTryTranslate(Vector3Int moveVector)
        {
            var newOriginCell = CellBound.min + moveVector;
            var newPosition = MainObject.transform.position + Vector3.Scale(moveVector,CellSize);
            MainObject.transform.position = newPosition;
            CellBound = new (newOriginCell,newOriginCell + CellBound.size);

            DefaultUpdateDisplayName();
            UpdateGizmoDimensions();
            DefaultUpdateColliderDimensions();
            return true;
        }
        protected bool DefaultTryResize(Vector2 scrollInput,RaycastHit hit,int length = 1)
        {
            if (scrollInput.y == 0)
            {
                return false;
            }

            var worldBounds = WorldBounds;
            var faceCenters = new (Directions faceDirs,Vector3 center)[]
            {
                new ( Directions.Right , new ( worldBounds.max.x    , worldBounds.center.y , worldBounds.center.z ) ),
                new ( Directions.Left  , new ( worldBounds.min.x    , worldBounds.center.y , worldBounds.center.z ) ),
                new ( Directions.Up    , new ( worldBounds.center.x , worldBounds.max.y    , worldBounds.center.z ) ),
                new ( Directions.Down  , new ( worldBounds.center.x , worldBounds.min.y    , worldBounds.center.z ) ),
                new ( Directions.Fwd   , new ( worldBounds.center.x , worldBounds.center.y , worldBounds.max.z    ) ),
                new ( Directions.Back  , new ( worldBounds.center.x , worldBounds.center.y , worldBounds.min.z    ) ),
            };

            Directions hitFace;
            int minIndex = 0;
            float minMagnitude = (hit.point - faceCenters[0].center).magnitude;
            for (int i = 1; i < faceCenters.Length;i++)
            {
                var magnitude = (hit.point - faceCenters[i].center).magnitude;
                if (magnitude < minMagnitude)
                {
                    minMagnitude = magnitude;
                    minIndex = i;
                }
            }

            hitFace = minIndex > -1 ? faceCenters[minIndex].faceDirs : Directions.None;
            ///if (hitFace == Directions.None)
            ///{
            ///    return false;
            ///}

            int inputLength = scrollInput.y > 0 ? -length : length;
            var cellOffset = inputLength * hitFace.ToVector();

            var newMin = Directions.LeftDownBack.HasFlags(hitFace) ? CellBound.min + cellOffset : CellBound.min;
            var newMax = Directions.RightUpFwd.HasFlags(hitFace) ? CellBound.max + cellOffset : CellBound.max;
            if (newMax.x <= newMin.x || newMax.y <= newMin.y || newMax.z <= newMin.z)
            {
                return TryTranslate(cellOffset);
            }
            CellBound.min = newMin;
            CellBound.max = newMax;

            if (Directions.LeftDownBack.HasFlags(hitFace))
            {
                var transformOffset = Vector3.Scale(cellOffset,CellSize);
                MainObject.transform.position = MainObject.transform.position + transformOffset;
            }

            DefaultUpdateDisplayName();
            UpdateGizmoDimensions();
            DefaultUpdateColliderDimensions();

            return true;    
        }

        // Default method => unregister & destroy
        protected bool DefaultTryUnregisterAndDestroy()
        {
            if (MainObject == null)
            {
                return true;
            }
            Manager.TryUnregisterGameObjectToInstance(MainObject);
            Manager.TryUnregisterInstance(this);
            GameObject.Destroy(MainObject);
            MainObject = null;
            return true;
        }

    }
}
