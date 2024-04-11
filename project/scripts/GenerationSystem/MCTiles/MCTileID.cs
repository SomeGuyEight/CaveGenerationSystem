
namespace SlimeGame
{
    /// <summary>
    /// 22 default <see cref="MCTile"/> first 3 character ID's
    /// <br/> -> each <see cref="MCTileID"/> value equals the default <see cref="MCTile"/> value
    /// </summary>
    public enum MCTileID
    {
        /// <summary> Void ( 1 variation ): <br/> ->     0b_0000_0000 == 0x00 == 0 </summary>
        _00 = 0x00,
        /// <summary> Corner ( 8 variations ): <br/> ->   0b_0000_0001 == 0x01 == 1 </summary>
        _10 = 0x01,
        /// <summary> Edge ( 12 variations ): <br/> ->     0b_0000_0011 == 0x03 == 3 </summary>
        _20 = 0x03,
        /// <summary> Slant ( 12 variations ): <br/> ->    0b_0000_0110 == 0x06 == 6 </summary>
        _21 = 0x06,
        /// <summary> Poles ( 4 variations ): <br/> ->    0b_0001_1000 == 0x18 == 24 </summary>
        _22 = 0x18,
        /// <summary> Triangle ( 24 variations ): <br/> -> 0b_0000_0111 == 0x07 == 7 </summary>
        _30 = 0x07,
        /// <summary> SlantCorner ( 8 variations ): <br/> -> 0b_0001_0110 == 0x15 == 22 </summary>
        _31 = 0x16,
        /// <summary> EdgeCorner ( 24 variations ): <br/> -> 0b_0001_1001 == 0x19 == 25 </summary>
        _32 = 0x19,
        /// <summary> Square ( 6 variations ): <br/> -> 0b_0000_1111 == 0x0F == 15 </summary>
        _40 = 0x0F,
        /// <summary> Corner ( 8 variations ): <br/> -> 0b_0001_0111 == 0x17 == 23 </summary>
        _41 = 0x17,
        /// <summary> ThreeEdges ( 24 variations ): <br/> -> 0b_0001_1011 == 0x1B == 27 </summary>
        _42 = 0x1B,
        /// <summary> CornerTriangle ( 24 variations ): <br/> -> 0b_0001_1110 == 0x1E == 30 </summary>
        _43 = 0x1E,
        /// <summary> TwoEdges ( 6 variations ): <br/> -> 0b_0011_1100 == 0x3C == 60 </summary>
        _44 = 0x3C,
        /// <summary> Poles ( 2 variations ): <br/> -> 0b_0110_1001 == 0x69 == 105 </summary>
        _45 = 0x69,
        /// <summary> Triangle ( 24 variations ): <br/> -> 0b_1111_1000 == 0xF8 == 248 </summary>
        _50 = 0xF8,
        /// <summary> SlantCorner ( 8 variations ): <br/> -> 0b_1110_1001 == 0xE9 == 233 </summary>
        _51 = 0xE9,
        /// <summary> EdgeCorner ( 24 variations ): <br/> -> 0b_1110_0110 == 0xE6 == 230 </summary>
        _52 = 0xE6,
        /// <summary> Edge ( 12 variations ): <br/> -> 0b_1111_1100 == 0x80 == 252 </summary>
        _60 = 0xFC,
        /// <summary> Slant ( 12 variations ): <br/> -> 0b_1111_1001 == 0xF9 == 249 </summary>
        _61 = 0xF9,
        /// <summary> Poles ( 4 variations ): <br/> -> 0b_1110_0111 == 0xE7 == 231 </summary>
        _62 = 0xE7,
        /// <summary> Corner ( 8 variations ): <br/> -> 0b_1111_1110 == 0xFE == 254 </summary>
        _70 = 0xFE,
        /// <summary> Air ( 1 variation ): <br/> -> 0b_1111_1111 == 0xFF == 255 </summary>
        _80 = 0xFF,
    }
}
