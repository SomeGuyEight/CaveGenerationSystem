using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SlimeGame
{
    [Serializable, ShowOdinSerializedPropertiesInInspector]
    [CreateAssetMenu(fileName = "new TileSO",menuName = "Slime Game/Default TileSO",order = 0)]
    public class TileSO : SerializedScriptableObject
    {
        public static TileSO GenericTile => GetNewInstance(new(0,0,0,0),null,null);
        public static TileSO GetNewInstance(TileProperties properties,CellTypes[,,] cellTypesArray,BaseSocket[] sockets)
        {
            TileSO newTile = ScriptableObject.CreateInstance<TileSO>();
            var convertedArray = cellTypesArray != null ? cellTypesArray.To1D2D() : TileHelper.DefaultCellTypesArrays;
            Initialize(newTile,properties,convertedArray,sockets);
            return newTile;
        }
        /// <summary>
        /// ( ! ) Here for other constructors
        /// <br/> -> They are removed from here right now because they are not finalized with this system
        /// </summary>
        private static void Initialize(TileSO newTile,TileProperties properties,CellTypes[][,] cellTypeArray,BaseSocket[] sockets)
        {
            newTile._properties = properties.DeepClone();
            newTile._cellTypesArrays = cellTypeArray;
            newTile._sockets = sockets ?? new BaseSocket[0];
            newTile.UpdateName();
        }

#pragma warning disable
        [Title("General"),OdinSerialize,ReadOnly,LabelWidth(150)]
        private string _name;

        [OdinSerialize,ReadOnly,LabelWidth(150)]
        private string _dateTimeStamp = SGUtils.DateTimeStamp();
#pragma warning restore
        [OdinSerialize,ReadOnly,LabelWidth(150)]
        private Vector3Int _arraySize = Vector3Int.zero;

        [Title("Tile Properties")]
        [OdinSerialize,HideLabel]
        private TileProperties _properties;

        [Title("Cell Types Arrays","( ! ) Note: The Y-axis is inverted below -> index 0 will be Y-min & index ^1 will be Y-max")]
        [TableMatrix(HorizontalTitle = "X axis",VerticalTitle = "Y axis",IsReadOnly = true),ListDrawerSettings(NumberOfItemsPerPage = 1)]
        [OdinSerialize,HideLabel,ShowInInspector,ReadOnly]
        private CellTypes[][,] _cellTypesArrays;

        [Title("Sockets")]
        [OdinSerialize,HideLabel,ListDrawerSettings(ShowFoldout = true)]
        private BaseSocket[] _sockets;

#pragma warning disable
        [Button("Apply Default Array")]
        private void ApplyDefaultArrayButton() => _cellTypesArrays = TileHelper.DefaultCellTypesArrays;

        [Button("Apply Default Sockets")]
        private void ApplyDefaultSocketsButton() => _sockets = this.GetDefaultSockets();

        [Button("Update Date Time Stamp")]
        private void UpdateDateTimeStampButton() => UpdateDateTimeStamp();

        [Button("Update Name")]
        private void UpdateNameButton() => UpdateName();
#pragma warning restore

        private CellTypes[,,] _cachedArrays;

        public string Name { get { return _name; } }
        public string DateTimeStamp { get { return _dateTimeStamp; } }
        public TileProperties Properties {  get { return _properties; } }
        public BaseSocket[] Sockets { get { return _sockets; } }
        public Vector3Int ArraySize { get { return _arraySize; } }

        public TileTypes Types       => _properties == null ? TileTypes.None    : _properties.Types;
        public TileSubTypes SubTypes => _properties == null ? TileSubTypes.None : _properties.SubTypes;
        public TileSizes TileSizes   => _properties == null ? TileSizes.None    : _properties.Sizes;
        public TileShapes Shapes     => _properties == null ? TileShapes.None   : _properties.Shapes;
        public Vector3 Center => Vector3.Scale(_arraySize,new Vector3(.5f,.5f,.5f));
        public CellTypes[,,] CellTypesArrays => _cachedArrays ??= _cellTypesArrays.To3D();

        private string UpdateDateTimeStamp() => _dateTimeStamp = SGUtils.DateTimeStamp();
        private string UpdateName() => _name = $"{Types} - Size {TileSizes} - Shape {Shapes}";
        private void UpdateArraySize()
        {
            if (_cellTypesArrays == null)
            {
                _arraySize = Vector3Int.zero;
            }
            else if (_cellTypesArrays[0] == null)
            {
                _arraySize = new Vector3Int(0,0,_cellTypesArrays.Length);
            }
            else
            {
                _arraySize = new Vector3Int(_cellTypesArrays[0].GetLength(0),_cellTypesArrays[0].GetLength(1),_cellTypesArrays.Length);
            }
        }
        public void UpdateTile(TileProperties properties,CellTypes[,,] cellTypesArrays,BaseSocket[] sockets,bool updateName = true)
        {
            _properties = properties.DeepClone();
            _cellTypesArrays = cellTypesArrays.To1D2D();
            _sockets = BaseSocket.DeepClone(sockets);
            UpdateArraySize();
            if (updateName)
            {
                UpdateName();
            }
        }

        protected override void OnBeforeSerialize()
        {
            UpdateArraySize();
        }

        protected override void OnAfterDeserialize()
        {
            UpdateArraySize();
        }

    }
}
