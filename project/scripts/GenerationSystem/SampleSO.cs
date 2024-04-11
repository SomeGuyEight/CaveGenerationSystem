using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Sirenix.OdinInspector;
using Sylves;
using Tessera;

namespace SlimeGame
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    [CreateAssetMenu(fileName = "new Tile Sample",menuName = "Slime Game/Tile Sample",order = 0)]
    public class SampleSO : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public static SampleSO SaveNewSOAndSamplePrefab(string tileSamplePath,string sampleObjectPath,MCTileDatabaseSO database,TesseraGenerator generator,TileProperties properties,CellTypes[,,] cellTypesArrays)
        {
            var newSample = ScriptableObject.CreateInstance<SampleSO>();
            var name = GetNewTileSampleName(properties);
            newSample._properties = properties;
            newSample._name = name;
            newSample._cellTypesArrays = cellTypesArrays.To1D2D();
            newSample._database = database;
            newSample._surfaceGenerator = generator;
            newSample.HandleSamplePrefab(sampleObjectPath);
            string path = tileSamplePath + $"/{newSample._name}.asset";
            AssetDatabase.CreateAsset(newSample,path);
            AssetDatabase.SaveAssets();
            return newSample;
        }
#endif

        [SerializeField]
        private string _name;

        [SerializeField]
        private TileProperties _properties;

        [SerializeField,ReadOnly]
        private CellTypes[][,] _cellTypesArrays = new CellTypes[0][,];

        [SerializeField,ReadOnly]
        private GameObject _samplePrefab;

        [SerializeField,ReadOnly]
        private MCTileDatabaseSO _database;

        [SerializeField,ReadOnly]
        private TesseraGenerator _surfaceGenerator;

        public string Name { get { return _name; } }
        public TileProperties Propeties { get { return _properties; } }
        public TileTypes Types { get { return _properties.Types; } }
        public TileSubTypes SubTypes { get { return _properties.SubTypes; } }
        public TileSizes Sizes { get { return _properties.Sizes; } }
        public TileShapes Shapes { get { return _properties.Shapes; } }
        public GameObject SamplePrefab { get { return _samplePrefab; } }

#if UNITY_EDITOR
        private static string GetNewTileSampleName(TileProperties properties)
        {
            return $"Tile Sample for Adjacent Model - {properties.Types} {properties.SubTypes} {properties.Sizes} {properties.Shapes} - {SGUtils.DateTimeStamp()}";
        }
        private void HandleSamplePrefab(string prefabPath)
        {
            var cellSize = _surfaceGenerator.cellSize;
            var cellCenter = Vector3.Scale(cellSize,new Vector3(.5f,.5f,.5f));
            Vector3Int size = new (_cellTypesArrays[0].GetLength(0),_cellTypesArrays[0].GetLength(1),_cellTypesArrays.Length);
            var boundCenter = Vector3.Scale(size,new Vector3(.5f,.5f,.5f));
            var generatorBound = new Bounds(boundCenter, size);
            _surfaceGenerator.bounds = generatorBound;
            var constraintBuilder = _surfaceGenerator.GetInitialConstraintBuilder();
            var costraints = _cellTypesArrays.BuildAirAndVoidConstraints(constraintBuilder,_database);
            TesseraCompletion completion = null;
            _surfaceGenerator.Generate(new TesseraGenerateOptions()
            {
                initialConstraints = costraints,
                onComplete = c => completion = c
            });
            if (completion == null)
            {
                throw new Exception("Tessera Completion not successfully returned to TileSample");
            }

            var tempObject = new GameObject($"Sample Objects ({_name})");
            var voidName = _database.VoidTile.name;
            var coreName = _database.AirTile.name;
            for (int x = 0;x < size.x;x++)
            {
                for (int y = 0;y < size.y;y++)
                {
                    for (int z = 0;z < size.z;z++)
                    {
                        var types = _cellTypesArrays[z][x,y];
                        if (types.HasFlags(CellTypes.Surface))
                        {
                            continue;
                        }
                        GameObject prefab;
                        if (types.HasFlags(CellTypes.Air))
                        {
                            prefab = _database.GetSampleObject(MCTile._80_1111_1111,CellTypes.Air,out var areCellTypesSynced,out var actualCellTypes);
                            if (!areCellTypesSynced)
                            {
                                /// TODO: add method to correct
                                Debug.Log("CellSubTypes not synced");
                            }
                        }
                        else
                        {
                            prefab = _database.GetSampleObject(MCTile._00_0000_0000,CellTypes.Void,out var areCellTypesSynced,out var actualCellTypes);
                            if (!areCellTypesSynced)
                            {
                                /// TODO: add method to correct
                                Debug.Log("CellSubTypes not synced");
                            }
                        }
                        var position = Vector3.Scale(new Vector3(x,y,z),cellSize) + cellCenter;
                        var newObject = Instantiate(prefab,position,Quaternion.identity,tempObject.transform);
                        newObject.name = prefab.name;
                    }
                }
            }

            var instances = completion.tileInstances;
            foreach (var tileInstance in instances)
            {
                var name = tileInstance.Tile.name;
                var cell = _surfaceGenerator.CellGrid.FindCell(tileInstance.Position - cellCenter) ?? throw new Exception("null cell returned from Cell Grid");
                var cellTypes = _cellTypesArrays[cell.z][cell.x,cell.y];
                var prefab = _database.GetTileSampleObject(tileInstance.Tile,cellTypes,out _,out var areCellTypesSynced,out _);
                if (!areCellTypesSynced)
                {
                    /// TODO: add method to correct
                    Debug.Log("CellSubTypes not synced");
                }
                var newObject = Instantiate(prefab,tileInstance.Position,tileInstance.Rotation,tempObject.transform);
                newObject.transform.localScale = tileInstance.LocalScale;
                newObject.name = prefab.name;
            }
            _samplePrefab = PrefabUtility.SaveAsPrefabAsset(tempObject,prefabPath + $"/{tempObject.name}.prefab");
            Destroy(tempObject);
        }
#endif
    }
}
