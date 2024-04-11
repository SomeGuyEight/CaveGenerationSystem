using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SlimeGame 
{
    [Serializable,ShowOdinSerializedPropertiesInInspector]
    public class TileProperties
    {
        public TileProperties()
        {

        }
        public TileProperties(TileTypes types,TileSubTypes subTypes,TileSizes sizes,TileShapes shapes) 
        {
            _types = types;
            _subTypes = subTypes;
            _sizes = sizes;
            _shapes = shapes;
        }

        [OdinSerialize]
        private TileTypes _types;

        [OdinSerialize]
        private TileSubTypes _subTypes;

        [OdinSerialize]
        private TileSizes _sizes;

        [OdinSerialize]
        private TileShapes _shapes;

        public TileTypes Types { get { return _types; } set { _types = value; } }
        public TileSubTypes SubTypes { get { return _subTypes; } set { _subTypes = value; } }
        public TileSizes Sizes { get { return _sizes; } set { _sizes = value; } }
        public TileShapes Shapes { get { return _shapes; } set { _shapes = value; } }

        public TileProperties DeepClone() 
        {
            return new TileProperties()
            {
                Types = _types,
                SubTypes = _subTypes,
                Sizes = _sizes,
                Shapes = _shapes,
            };
        }
    }
}
