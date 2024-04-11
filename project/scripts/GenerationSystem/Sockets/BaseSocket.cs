using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SlimeGame 
{
    [Serializable,ShowOdinSerializedPropertiesInInspector]
    public abstract class BaseSocket
    {
        public BaseSocket(Directions directions,Vector3Int cell)
        {
            _directions = directions;
            _cell = cell;
        }

        [OdinSerialize,LabelWidth(100),PropertyOrder(8)]
        protected Directions _directions;

        [OdinSerialize,LabelWidth(100),PropertyOrder(8)]
        protected Vector3Int _cell;

        [ShowInInspector, ReadOnly, LabelWidth(100), PropertyOrder(-8)]
        public abstract SocketTypes SocketTypes { get; }

        /// <summary>
        /// Directions is relative to local rotation
        /// </summary>
        public Directions Directions { get { return _directions; } set { _directions = value; } }
        /// <summary>
        /// Cell is relative to local rotation &amp; bounds
        /// </summary>
        public Vector3Int Cell { get { return _cell; } set { _cell = value; } }

        public abstract BaseSocket DeepClone(Vector3Int? offset = null);
        public abstract BaseSocketInstance GetSocketInstance(InstanceManager manager,MCTileDatabaseSO database,TileInstance parentTile,Vector3Int? offset = null);

        public static BaseSocket[] DeepClone(BaseSocket[] sockets)
        {
            if (sockets == null)
            {
                return new BaseSocket[0];
            }
            var clones = new BaseSocket[sockets.Length];
            for (int i = 0;i < sockets.Length;i++)
            {
                var socket = sockets[i];
                if (socket != null)
                {
                    clones[i] = socket.SocketTypes.NewSocket(socket._directions,socket._cell);
                }                
            }
            return clones;
        }

    }
}
