using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace SlimeGame
{
    public class MCTileDatabaseDisplay : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private readonly MCTileDatabaseSO _database;

        [OdinSerialize,ReadOnly]
        private GameObject _tileHolder;
        [OdinSerialize,ReadOnly]
        private GameObject _meshHolder;
        [OdinSerialize,ReadOnly]
        private GameObject _bigTileHolder;

#pragma warning disable
        [Button]
        private void RefreshDatabaseInScene()
        {
#pragma warning restore

            if (_database == null)
            {
                DestroyImmediate(_tileHolder);
                DestroyImmediate(_meshHolder);
                DestroyImmediate(_bigTileHolder);
                return;
            }

            transform.position = Vector3.zero;     

            Dictionary<CellTypes,GameObject> cellTypesToTileHolder = new ()
            {
                { CellTypes.Surface         , new ("Default Tile Bases") },
                { CellTypes.FloorSurface    , new ("Floor Tile Bases"  ) },
                { CellTypes.WallSurface     , new ("Wall Tile Bases"   ) },
                { CellTypes.CeilingSurface  , new ("Ceiling Tile Bases") },
            };
            if (_tileHolder != null)
            {
                DestroyImmediate(_tileHolder);
            }
            _tileHolder = new GameObject("TileBase Objects");
            int currentX;
            var currentZ = 0;
            foreach (var (cellTypes,gameObjects) in _database.GetAllTileObjects())
            {
                var holder = cellTypesToTileHolder[cellTypes];
                currentX = 0;
                foreach (var originalTileObject in gameObjects.Where(x => x != null && x.gameObject != null))
                {
                    var tileObject = Instantiate(originalTileObject,new (currentX,0,0),Quaternion.identity,holder.transform);
                    tileObject.transform.parent = holder.transform;
                    currentX += 2;
                }
                holder.transform.position += new Vector3(0,0,currentZ);
                holder.transform.parent = _tileHolder.transform;
                currentZ += 2;
            }
            _tileHolder.transform.parent = transform;

            Dictionary<CellTypes,GameObject> cellTypesToMeshHolder = new ()
            {
                { CellTypes.Surface         , new ("Default Meshes") },
                { CellTypes.FloorSurface    , new ("Floor Meshes"  ) },
                { CellTypes.WallSurface     , new ("Wall Meshes"   ) },
                { CellTypes.CeilingSurface  , new ("Ceiling Meshes") },
            };
            if (_meshHolder != null)
            {
                DestroyImmediate(_meshHolder);
            }
            _meshHolder = new GameObject("Mesh Objects");
            currentZ = 0;
            foreach (var (cellTypes,gameObjects) in _database.GetAllMeshObjects())
            {
                var holder = cellTypesToMeshHolder[cellTypes];
                currentX = 0;
                foreach (var originalMeshObject in gameObjects)
                {
                    var meshObject = Instantiate(originalMeshObject,new (currentX,0,0),Quaternion.identity,holder.transform);
                    meshObject.transform.parent = holder.transform;
                    currentX += 2;
                }
                holder.transform.position += new Vector3(0,0,currentZ);
                holder.transform.parent = _meshHolder.transform;
                currentZ += 2;
            }
            _meshHolder.transform.parent = transform;

            if (_bigTileHolder != null)
            {
                DestroyImmediate(_bigTileHolder);
            }
            _bigTileHolder = new GameObject("BigTile Objects");
            currentX = 0;
            foreach (var bigTile in _database.GetBigTileObjects().Where(x => x != null))
            {
                var tileObject = Instantiate(bigTile,new (currentX,0,0),Quaternion.identity,_bigTileHolder.transform);
                _bigTileHolder.transform.parent = _bigTileHolder.transform;
                currentX += 4;
            }
            _bigTileHolder.transform.position = new (0,0,currentZ + 4);
            _bigTileHolder.transform.parent = transform;

            transform.position = new Vector3(.5f,.5f,.5f);
        }
    }
}
