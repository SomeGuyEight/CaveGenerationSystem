
namespace SlimeGame
{
    /// <summary>
    /// 256 variations of masked corners representing Marching Cubes between two surfaces
    /// <br/> 22 different default tiles
    /// <br/><br/> first two <see cref="int"/>s in each name are the "<see cref="MCTileID"/>" ( ex. '_00' int <see cref="MCTile._00_0000_0000"/>)
    /// <br/> -> The first int indicates the number of corners masked ( ex. '8' in <see cref="_80_1111_1111"/> )
    /// <br/> -> The second int indicates the default tile index within the <see cref="MCTile"/>s with the same quantity of corner masks 
    /// ( ex. '0', '1', &amp; '2' in <see cref="_60_1111_1100"/>, <see cref="_61_1111_1001"/>, <see cref="_62_1110_0111"/>)
    /// </summary>
    public enum MCTile
    {
        // Tile MCTiles
                                /// <summary> MC Tile Name: Void           <br/> MC Tile Index: 00 (22) <br/> Rotation: default <br/> bits: 0b_0000_0000 <br/>  hexidecimal: 0x00 <br/> int: 0     </summary>
        _00_0000_0000 = 0x00,   /// <summary> MC Tile Name: Corner         <br/> MC Tile Index: 01 (22) <br/> Rotation: default <br/> bits: 0b_0000_0001 <br/>  hexidecimal: 0x01 <br/> int: 1     </summary>
        _10_0000_0001 = 0x01,   /// <summary> MC Tile Name: Edge           <br/> MC Tile Index: 02 (22) <br/> Rotation: default <br/> bits: 0b_0000_0011 <br/>  hexidecimal: 0x03 <br/> int: 3     </summary>
        _20_0000_0011 = 0x03,   /// <summary> MC Tile Name: Slant          <br/> MC Tile Index: 03 (22) <br/> Rotation: default <br/> bits: 0b_0000_0110 <br/>  hexidecimal: 0x06 <br/> int: 6     </summary>
        _21_0000_0110 = 0x06,   /// <summary> MC Tile Name: Poles          <br/> MC Tile Index: 04 (22) <br/> Rotation: default <br/> bits: 0b_0001_1000 <br/>  hexidecimal: 0x18 <br/> int: 24    </summary>
        _22_0001_1000 = 0x18,   /// <summary> MC Tile Name: Triangle       <br/> MC Tile Index: 05 (22) <br/> Rotation: default <br/> bits: 0b_0000_0111 <br/>  hexidecimal: 0x07 <br/> int: 7     </summary>
        _30_0000_0111 = 0x07,   /// <summary> MC Tile Name: SlantCorner    <br/> MC Tile Index: 06 (22) <br/> Rotation: default <br/> bits: 0b_0001_0110 <br/>  hexidecimal: 0x15 <br/> int: 22    </summary>
        _31_0001_0110 = 0x16,   /// <summary> MC Tile Name: EdgeCorner     <br/> MC Tile Index: 07 (22) <br/> Rotation: default <br/> bits: 0b_0001_1001 <br/>  hexidecimal: 0x19 <br/> int: 25    </summary>
        _32_0001_1001 = 0x19,   /// <summary> MC Tile Name: Square         <br/> MC Tile Index: 08 (22) <br/> Rotation: default <br/> bits: 0b_0000_1111 <br/>  hexidecimal: 0x0F <br/> int: 15    </summary>
        _40_0000_1111 = 0x0F,   /// <summary> MC Tile Name: Corner         <br/> MC Tile Index: 09 (22) <br/> Rotation: default <br/> bits: 0b_0001_0111 <br/>  hexidecimal: 0x17 <br/> int: 23    </summary>
        _41_0001_0111 = 0x17,   /// <summary> MC Tile Name: ThreeEdges     <br/> MC Tile Index: 10 (22) <br/> Rotation: default <br/> bits: 0b_0001_1011 <br/>  hexidecimal: 0x1B <br/> int: 27    </summary>
        _42_0001_1011 = 0x1B,   /// <summary> MC Tile Name: CornerTriangle <br/> MC Tile Index: 11 (22) <br/> Rotation: default <br/> bits: 0b_0001_1110 <br/>  hexidecimal: 0x1E <br/> int: 30    </summary>
        _43_0001_1110 = 0x1E,   /// <summary> MC Tile Name: TwoEdges       <br/> MC Tile Index: 12 (22) <br/> Rotation: default <br/> bits: 0b_0011_1100 <br/>  hexidecimal: 0x3C <br/> int: 60    </summary>
        _44_0011_1100 = 0x3C,   /// <summary> MC Tile Name: Poles          <br/> MC Tile Index: 13 (22) <br/> Rotation: default <br/> bits: 0b_0110_1001 <br/>  hexidecimal: 0x69 <br/> int: 105   </summary>
        _45_0110_1001 = 0x69,   /// <summary> MC Tile Name: Triangle       <br/> MC Tile Index: 14 (22) <br/> Rotation: default <br/> bits: 0b_1111_1000 <br/>  hexidecimal: 0xF8 <br/> int: 248   </summary>
        _50_1111_1000 = 0xF8,   /// <summary> MC Tile Name: SlantCorner    <br/> MC Tile Index: 15 (22) <br/> Rotation: default <br/> bits: 0b_1110_1001 <br/>  hexidecimal: 0xE9 <br/> int: 233   </summary>
        _51_1110_1001 = 0xE9,   /// <summary> MC Tile Name: EdgeCorner     <br/> MC Tile Index: 16 (22) <br/> Rotation: default <br/> bits: 0b_1110_0110 <br/>  hexidecimal: 0xE6 <br/> int: 230   </summary>
        _52_1110_0110 = 0xE6,   /// <summary> MC Tile Name: Edge           <br/> MC Tile Index: 17 (22) <br/> Rotation: default <br/> bits: 0b_1111_1100 <br/>  hexidecimal: 0x80 <br/> int: 252   </summary>
        _60_1111_1100 = 0xFC,   /// <summary> MC Tile Name: Slant          <br/> MC Tile Index: 18 (22) <br/> Rotation: default <br/> bits: 0b_1111_1001 <br/>  hexidecimal: 0xF9 <br/> int: 249   </summary>
        _61_1111_1001 = 0xF9,   /// <summary> MC Tile Name: Poles          <br/> MC Tile Index: 19 (22) <br/> Rotation: default <br/> bits: 0b_1110_0111 <br/>  hexidecimal: 0xE7 <br/> int: 231   </summary>
        _62_1110_0111 = 0xE7,   /// <summary> MC Tile Name: Corner         <br/> MC Tile Index: 20 (22) <br/> Rotation: default <br/> bits: 0b_1111_1110 <br/>  hexidecimal: 0xFE <br/> int: 254   </summary>
        _70_1111_1110 = 0xFE,   /// <summary> MC Tile Name: Air            <br/> MC Tile Index: 21 (22) <br/> Rotation: default <br/> bits: 0b_1111_1111 <br/>  hexidecimal: 0xFF <br/> int: 255   </summary>
        _80_1111_1111 = 0xFF,    

        // 00 Void => 1 variations
        /// _00_0000_0000 = 0x00,    /// <summary> Void:     0b_0000_0000 => 0x00 => 0

        // 10 Corner => 8 variations
        ///_10_0000_0001 = 0x01,    /// <summary> Corner:   0b_0000_0001 => 0x01 => 1
        _10_0000_0010 = 0x02,    /// <summary> Corner:                           2
        _10_0000_0100 = 0x04,    /// <summary> Corner:                           4
        _10_0000_1000 = 0x08,    /// <summary> Corner:                           8
        _10_0001_0000 = 0x10,    /// <summary> Corner:                           16
        _10_0010_0000 = 0x20,    /// <summary> Corner:                           32
        _10_0100_0000 = 0x40,    /// <summary> Corner:                           64
        _10_1000_0000 = 0x80,    /// <summary> Corner:                           128

        // 20 Edge => 12 variations
        ///_20_0000_0011 = 0x03,    /// <summary> Edge:     0b_0000_0011 => 0x03 => 3
        _20_0000_0101 = 0x05,    /// <summary> Edge:                             5
        _20_0000_1010 = 0x0A,    /// <summary> Edge:                             10
        _20_0000_1100 = 0x0C,    /// <summary> Edge:                             12
        _20_0011_0000 = 0x30,    /// <summary> Edge:                             48
        _20_0101_0000 = 0x50,    /// <summary> Edge:                             80
        _20_1010_0000 = 0xA0,    /// <summary> Edge:                             160
        _20_1100_0000 = 0xC0,    /// <summary> Edge:                             192
        _20_0001_0001 = 0x11,    /// <summary> Edge:                             17
        _20_0010_0010 = 0x22,    /// <summary> Edge:                             34
        _20_0100_0100 = 0x44,    /// <summary> Edge:                             68
        _20_1000_1000 = 0x88,    /// <summary> Edge:                             136

        // 21 Slant => 12 variations
        ///_21_0000_0110 = 0x06,    /// <summary> Slant:    0b_0000_0110 => 0x06 => 6
        _21_0000_1001 = 0x09,    /// <summary> Slant:                            9
        _21_1001_0000 = 0x90,    /// <summary> Slant:                            144
        _21_0110_0000 = 0x60,    /// <summary> Slant:                            96
        _21_0100_0001 = 0x41,    /// <summary> Slant:                            65
        _21_0001_0100 = 0x14,    /// <summary> Slant:                            20
        _21_1000_0010 = 0x82,    /// <summary> Slant:                            130
        _21_0010_1000 = 0x28,    /// <summary> Slant:                            40
        _21_1000_0100 = 0x84,    /// <summary> Slant:                            132
        _21_0100_1000 = 0x48,    /// <summary> Slant:                            72
        _21_0010_0001 = 0x21,    /// <summary> Slant:                            33
        _21_0001_0010 = 0x12,    /// <summary> Slant:                            18

        // 22 Poles => 4 variations
        ///_22_0001_1000 = 0x18,    /// <summary> Poles:    0b_0001_1000 => 0x18 => 24
        _22_0010_0100 = 0x24,    /// <summary> Poles:                            36
        _22_0100_0010 = 0x42,    /// <summary> Poles:                            66
        _22_1000_0001 = 0x81,    /// <summary> Poles:                            129

        // 30 Triangle => 24 variations
        ///_30_0000_0111 = 0x07,    /// <summary> Triangle: 0b_0000_0111 => 0x07 => 7
        _30_0000_1011 = 0x0B,    /// <summary> Triangle:                         11
        _30_0000_1101 = 0x0D,    /// <summary> Triangle:                         13
        _30_0000_1110 = 0x0E,    /// <summary> Triangle:                         14
        _30_0111_0000 = 0x70,    /// <summary> Triangle:                         112
        _30_1011_0000 = 0xB0,    /// <summary> Triangle:                         176
        _30_1101_0000 = 0xD0,    /// <summary> Triangle:                         208
        _30_1110_0000 = 0xE0,    /// <summary> Triangle:                         224
        _30_0001_0101 = 0x15,    /// <summary> Triangle:                         21
        _30_0100_0101 = 0x45,    /// <summary> Triangle:                         69
        _30_0101_0001 = 0x51,    /// <summary> Triangle:                         81
        _30_0101_0100 = 0x54,    /// <summary> Triangle:                         84
        _30_0010_1010 = 0x2A,    /// <summary> Triangle:                         42
        _30_1000_1010 = 0x8A,    /// <summary> Triangle:                         138
        _30_1010_0010 = 0xA2,    /// <summary> Triangle:                         162
        _30_1010_1000 = 0xA8,    /// <summary> Triangle:                         168
        _30_0001_0011 = 0x13,    /// <summary> Triangle:                         19
        _30_0010_0011 = 0x23,    /// <summary> Triangle:                         35
        _30_0011_0001 = 0x31,    /// <summary> Triangle:                         49
        _30_0011_0010 = 0x32,    /// <summary> Triangle:                         50
        _30_0100_1100 = 0x4C,    /// <summary> Triangle:                         76
        _30_1000_1100 = 0x8C,    /// <summary> Triangle:                         140
        _30_1100_0100 = 0xC4,    /// <summary> Triangle:                         196
        _30_1100_1000 = 0xC8,    /// <summary> Triangle:                         200

        // 31 SlantCorner => 8 variations
        ///_31_0001_0110 = 0x16,    /// <summary> SlantCorner: 0b_0001_0110 => 0x15 => 22
        _31_1000_0110 = 0x86,    /// <summary> SlantCorner:                         134
        _31_0010_1001 = 0x29,    /// <summary> SlantCorner:                         41
        _31_0100_1001 = 0x49,    /// <summary> SlantCorner:                         73
        _31_0110_0001 = 0x61,    /// <summary> SlantCorner:                         97
        _31_0110_1000 = 0x68,    /// <summary> SlantCorner:                         104
        _31_1001_0010 = 0x92,    /// <summary> SlantCorner:                         146
        _31_1001_0100 = 0x94,    /// <summary> SlantCorner:                         148

        // 32 EdgeCorner => 24 variations
        ///_32_0001_1001 = 0x19,    /// <summary> EdgeCorner: 0b_0001_1001 => 0x19 => 25
        _32_0100_0011 = 0x43,    /// <summary> EdgeCorner:                         67
        _32_1000_0011 = 0x83,    /// <summary> EdgeCorner:                         131
        _32_0010_0101 = 0x25,    /// <summary> EdgeCorner:                         37
        _32_1000_0101 = 0x85,    /// <summary> EdgeCorner:                         133
        _32_0001_1010 = 0x1A,    /// <summary> EdgeCorner:                         26
        _32_0100_1010 = 0x4A,    /// <summary> EdgeCorner:                         74
        _32_0001_1100 = 0x1C,    /// <summary> EdgeCorner:                         28
        _32_0010_1100 = 0x2C,    /// <summary> EdgeCorner:                         44
        _32_0011_0100 = 0x34,    /// <summary> EdgeCorner:                         52
        _32_0011_1000 = 0x38,    /// <summary> EdgeCorner:                         56
        _32_0101_0010 = 0x52,    /// <summary> EdgeCorner:                         82
        _32_0101_1000 = 0x58,    /// <summary> EdgeCorner:                         88
        _32_1010_0001 = 0xA1,    /// <summary> EdgeCorner:                         161
        _32_1010_0100 = 0xA4,    /// <summary> EdgeCorner:                         164
        _32_1100_0001 = 0xC1,    /// <summary> EdgeCorner:                         193
        _32_1100_0010 = 0xC2,    /// <summary> EdgeCorner:                         194
        _32_1001_0001 = 0x91,    /// <summary> EdgeCorner:                         145
        _32_0010_0110 = 0x26,    /// <summary> EdgeCorner:                         38
        _32_0110_0010 = 0x62,    /// <summary> EdgeCorner:                         98
        _32_0100_0110 = 0x46,    /// <summary> EdgeCorner:                         70
        _32_0110_0100 = 0x64,    /// <summary> EdgeCorner:                         100
        _32_1000_1001 = 0x89,    /// <summary> EdgeCorner:                         137
        _32_1001_1000 = 0x98,    /// <summary> EdgeCorner:                         152

        // 40 => 6 variations
        ///_40_0000_1111 = 0x0F,    /// <summary> Square: 0b_0000_1111 => 0x0F => 15
        _40_0011_0011 = 0x33,    /// <summary> Square:                         51
        _40_0110_0110 = 0x66,    /// <summary> Square:                         102
        _40_1001_1001 = 0x99,    /// <summary> Square:                         153
        _40_1100_1100 = 0xCC,    /// <summary> Square:                         204
        _40_1111_0000 = 0xF0,    /// <summary> Square:                         240

        // 41 => 8 variations
        ///_41_0001_0111 = 0x17,    /// <summary> Corner: 0b_0001_0111 => 0x17 => 23
        _41_0010_1011 = 0x2B,    /// <summary> Corner:                         43
        _41_0100_1101 = 0x4D,    /// <summary> Corner:                         77
        _41_1000_1110 = 0x8E,    /// <summary> Corner:                         142
        _41_0111_0001 = 0x71,    /// <summary> Corner:                         113
        _41_1011_0010 = 0xB2,    /// <summary> Corner:                         178
        _41_1101_0100 = 0xD4,    /// <summary> Corner:                         212
        _41_1110_1000 = 0xE8,    /// <summary> Corner:                         232

        // 42 => 24 variations
        ///_42_0001_1011 = 0x1B,    /// <summary> ThreeEdges: 0b_0001_1011 => 0x1B => 27
        _42_0010_0111 = 0x27,    /// <summary> ThreeEdges:                         39
        _42_0100_1110 = 0x4E,    /// <summary> ThreeEdges:                         78
        _42_1000_1101 = 0x8D,    /// <summary> ThreeEdges:                         141
        _42_0001_1101 = 0x1D,    /// <summary> ThreeEdges:                         29
        _42_0100_0111 = 0x47,    /// <summary> ThreeEdges:                         71
        _42_0010_1110 = 0x2E,    /// <summary> ThreeEdges:                         46
        _42_1000_1011 = 0x8B,    /// <summary> ThreeEdges:                         139
        _42_1011_0001 = 0xB1,    /// <summary> ThreeEdges:                         177
        _42_0111_0010 = 0x72,    /// <summary> ThreeEdges:                         114
        _42_1110_0100 = 0xE4,    /// <summary> ThreeEdges:                         228
        _42_1101_1000 = 0xD8,    /// <summary> ThreeEdges:                         216
        _42_0111_0100 = 0x74,    /// <summary> ThreeEdges:                         116
        _42_1101_0001 = 0xD1,    /// <summary> ThreeEdges:                         209
        _42_1110_0010 = 0xE2,    /// <summary> ThreeEdges:                         226
        _42_1011_1000 = 0xB8,    /// <summary> ThreeEdges:                         184
        _42_0101_0011 = 0x53,    /// <summary> ThreeEdges:                         83
        _42_0011_0101 = 0x35,    /// <summary> ThreeEdges:                         53
        _42_1010_0011 = 0xA3,    /// <summary> ThreeEdges:                         163
        _42_0011_1010 = 0x3A,    /// <summary> ThreeEdges:                         58
        _42_0101_1100 = 0x5C,    /// <summary> ThreeEdges:                         92
        _42_1100_0101 = 0xC5,    /// <summary> ThreeEdges:                         197
        _42_1010_1100 = 0xAC,    /// <summary> ThreeEdges:                         172
        _42_1100_1010 = 0xCA,    /// <summary> ThreeEdges:                         202

        // 43 => 24 variations
        ///_43_0001_1110 = 0x1E,    /// <summary> CornerTriangle: 0b_0001_1110 => 0x1E => 30
        _43_0010_1101 = 0x2D,    /// <summary> CornerTriangle:                         45
        _43_0100_1011 = 0x4B,    /// <summary> CornerTriangle:                         75
        _43_1000_0111 = 0x87,    /// <summary> CornerTriangle:                         135
        _43_0111_1000 = 0x78,    /// <summary> CornerTriangle:                         120
        _43_1011_0100 = 0xB4,    /// <summary> CornerTriangle:                         180
        _43_1101_0010 = 0xD2,    /// <summary> CornerTriangle:                         210
        _43_1110_0001 = 0xE1,    /// <summary> CornerTriangle:                         225
        _43_1001_0101 = 0x95,    /// <summary> CornerTriangle:                         149
        _43_0110_0101 = 0x65,    /// <summary> CornerTriangle:                         101
        _43_0101_1001 = 0x59,    /// <summary> CornerTriangle:                         89
        _43_0101_0110 = 0x56,    /// <summary> CornerTriangle:                         86
        _43_0110_1010 = 0x6A,    /// <summary> CornerTriangle:                         106
        _43_1001_1010 = 0x9A,    /// <summary> CornerTriangle:                         154
        _43_1010_0110 = 0xA6,    /// <summary> CornerTriangle:                         166
        _43_1010_1001 = 0xA9,    /// <summary> CornerTriangle:                         169
        _43_1001_0011 = 0x93,    /// <summary> CornerTriangle:                         147
        _43_0110_0011 = 0x63,    /// <summary> CornerTriangle:                         99
        _43_0011_1001 = 0x39,    /// <summary> CornerTriangle:                         57
        _43_0011_0110 = 0x36,    /// <summary> CornerTriangle:                         54
        _43_0110_1100 = 0x6C,    /// <summary> CornerTriangle:                         108
        _43_1001_1100 = 0x9C,    /// <summary> CornerTriangle:                         156
        _43_1100_0110 = 0xC6,    /// <summary> CornerTriangle:                         198
        _43_1100_1001 = 0xC9,    /// <summary> CornerTriangle:                         201

        // 44 => 6 variations
        ///_44_0011_1100 = 0x3C,    /// <summary> TwoEdges: 0b_0011_1100 => 0x3C => 60
        _44_1100_0011 = 0xC3,    /// <summary> TwoEdges:                         195
        _44_0101_0101 = 0x55,    /// <summary> TwoEdges:                         85
        _44_1010_1010 = 0xAA,    /// <summary> TwoEdges:                         170
        _44_1010_0101 = 0xA5,    /// <summary> TwoEdges:                         165
        _44_0101_1010 = 0x5A,    /// <summary> TwoEdges:                         90

        // 45 => 2 variations
        ///_45_0110_1001 = 0x69,    /// <summary> Poles: 0b_0110_1001 => 0x69 => 105
        _45_1001_0110 = 0x96,    /// <summary> Poles:                         150

        // 50 => 24 variations
        ///_50_1111_1000 = 0xF8,    /// <summary> Triangle: 0b_1111_1000 => 0xF8 => 248
        _50_1111_0100 = 0xF4,    /// <summary> Triangle:                         244
        _50_1111_0010 = 0xF2,    /// <summary> Triangle:                         242
        _50_1111_0001 = 0xF1,    /// <summary> Triangle:                         241
        _50_1000_1111 = 0x8F,    /// <summary> Triangle:                         143
        _50_0100_1111 = 0x4F,    /// <summary> Triangle:                         79
        _50_0010_1111 = 0x2F,    /// <summary> Triangle:                         47
        _50_0001_1111 = 0x1F,    /// <summary> Triangle:                         31
        _50_1110_1010 = 0xEA,    /// <summary> Triangle:                         234
        _50_1011_1010 = 0xBA,    /// <summary> Triangle:                         186
        _50_1010_1110 = 0xAE,    /// <summary> Triangle:                         174
        _50_1010_1011 = 0xAB,    /// <summary> Triangle:                         171
        _50_0111_0101 = 0x75,    /// <summary> Triangle:                         117
        _50_1101_0101 = 0xD5,    /// <summary> Triangle:                         213
        _50_0101_1101 = 0x5D,    /// <summary> Triangle:                         93
        _50_0101_0111 = 0x57,    /// <summary> Triangle:                         87
        _50_1110_1100 = 0xEC,    /// <summary> Triangle:                         236
        _50_1101_1100 = 0xDC,    /// <summary> Triangle:                         220
        _50_1100_1110 = 0xCE,    /// <summary> Triangle:                         206
        _50_1100_1101 = 0xCD,    /// <summary> Triangle:                         205
        _50_1011_0011 = 0xB3,    /// <summary> Triangle:                         179
        _50_0111_0011 = 0x73,    /// <summary> Triangle:                         115
        _50_0011_1011 = 0x3B,    /// <summary> Triangle:                         59
        _50_0011_0111 = 0x37,    /// <summary> Triangle:                         55

        // 51 => 8 variations
        ///_51_1110_1001 = 0xE9,    /// <summary> SlantCorner: 0b_1110_1001 => 0xE9 => 233
        _51_0111_1001 = 0x79,    /// <summary> SlantCorner:                         121
        _51_1101_0110 = 0xD6,    /// <summary> SlantCorner:                         214
        _51_1011_0110 = 0xB6,    /// <summary> SlantCorner:                         182
        _51_1001_1110 = 0x9E,    /// <summary> SlantCorner:                         158
        _51_1001_0111 = 0x97,    /// <summary> SlantCorner:                         151
        _51_0110_1101 = 0x6D,    /// <summary> SlantCorner:                         109
        _51_0110_1011 = 0x6B,    /// <summary> SlantCorner:                         107

        // 52 => 24 variations
        ///_52_1110_0110 = 0xE6,    /// <summary> EdgeCorner: 0b_1110_0110 => 0xE6 => 230
        _52_1011_1100 = 0xBC,    /// <summary> EdgeCorner:                         188
        _52_0111_1100 = 0x7C,    /// <summary> EdgeCorner:                         124
        _52_1101_1010 = 0xDA,    /// <summary> EdgeCorner:                         218
        _52_0111_1010 = 0x7A,    /// <summary> EdgeCorner:                         122
        _52_1110_0101 = 0xE5,    /// <summary> EdgeCorner:                         229
        _52_1011_0101 = 0xB5,    /// <summary> EdgeCorner:                         181
        _52_1110_0011 = 0xE3,    /// <summary> EdgeCorner:                         227
        _52_1101_0011 = 0xD3,    /// <summary> EdgeCorner:                         211
        _52_1100_1011 = 0xCB,    /// <summary> EdgeCorner:                         203
        _52_1100_0111 = 0xC7,    /// <summary> EdgeCorner:                         199
        _52_1010_1101 = 0xAD,    /// <summary> EdgeCorner:                         173
        _52_1010_0111 = 0xA7,    /// <summary> EdgeCorner:                         167
        _52_0101_1110 = 0x5E,    /// <summary> EdgeCorner:                         94
        _52_0101_1011 = 0x5B,    /// <summary> EdgeCorner:                         91
        _52_0011_1110 = 0x3E,    /// <summary> EdgeCorner:                         62
        _52_0011_1101 = 0x3D,    /// <summary> EdgeCorner:                         61
        _52_0110_1110 = 0x6E,    /// <summary> EdgeCorner:                         110
        _52_1101_1001 = 0xD9,    /// <summary> EdgeCorner:                         217
        _52_1001_1101 = 0x9D,    /// <summary> EdgeCorner:                         157
        _52_1011_1001 = 0xB9,    /// <summary> EdgeCorner:                         185
        _52_1001_1011 = 0x9B,    /// <summary> EdgeCorner:                         155
        _52_0111_0110 = 0x76,    /// <summary> EdgeCorner:                         118
        _52_0110_0111 = 0x67,    /// <summary> EdgeCorner:                         103

        // 60 => 12 variations
        ///_60_1111_1100 = 0xFC,    /// <summary> Edge: 0b_1111_1100 => 0x80 => 252
        _60_1111_1010 = 0xFA,    /// <summary> Edge:                         250
        _60_1111_0101 = 0xF5,    /// <summary> Edge:                         245
        _60_1111_0011 = 0xF3,    /// <summary> Edge:                         243
        _60_1100_1111 = 0xCF,    /// <summary> Edge:                         207
        _60_1010_1111 = 0xAF,    /// <summary> Edge:                         175
        _60_0101_1111 = 0x5F,    /// <summary> Edge:                         95
        _60_0011_1111 = 0x3F,    /// <summary> Edge:                         63
        _60_1110_1110 = 0xEE,    /// <summary> Edge:                         238
        _60_1101_1101 = 0xDD,    /// <summary> Edge:                         221
        _60_1011_1011 = 0xBB,    /// <summary> Edge:                         187
        _60_0111_0111 = 0x77,    /// <summary> Edge:                         119

        // 61 => 12 variations
        ///_61_1111_1001 = 0xF9,    /// <summary> Slant: 0b_1111_1001 => 0xF9 => 249
        _61_1111_0110 = 0xF6,    /// <summary> Slant:                         246
        _61_1001_1111 = 0x9F,    /// <summary> Slant:                         159
        _61_0110_1111 = 0x6F,    /// <summary> Slant:                         111
        _61_1011_1110 = 0xBE,    /// <summary> Slant:                         190
        _61_1110_1011 = 0xEB,    /// <summary> Slant:                         235
        _61_0111_1101 = 0x7D,    /// <summary> Slant:                         125
        _61_1101_0111 = 0xD7,    /// <summary> Slant:                         215
        _61_0111_1011 = 0x7B,    /// <summary> Slant:                         123
        _61_1011_0111 = 0xB7,    /// <summary> Slant:                         183
        _61_1101_1110 = 0xDE,    /// <summary> Slant:                         222
        _61_1110_1101 = 0xED,    /// <summary> Slant:                         237

        // 62 => 4 variations
        ///_62_1110_0111 = 0xE7,    /// <summary> Poles: 0b_1110_0111 => 0xE7 => 231
        _62_1101_1011 = 0xDB,    /// <summary> Poles:                         219
        _62_1011_1101 = 0xBD,    /// <summary> Poles:                         189
        _62_0111_1110 = 0x7E,    /// <summary> Poles:                         126

        // 70 => 8 variations
        ///_70_1111_1110 = 0xFE,    /// <summary> Corner: 0b_1111_1110 => 0xFE => 254
        _70_1111_1101 = 0xFD,    /// <summary> Corner:                         253
        _70_1111_1011 = 0xFB,    /// <summary> Corner:                         251
        _70_1111_0111 = 0xF7,    /// <summary> Corner:                         247
        _70_1110_1111 = 0xEF,    /// <summary> Corner:                         239
        _70_1101_1111 = 0xDF,    /// <summary> Corner:                         223
        _70_1011_1111 = 0xBF,    /// <summary> Corner:                         191
        _70_0111_1111 = 0x7F,    /// <summary> Corner:                         127

        // 80 => 1 variations
        ///_80_1111_1111 = 0xFF,    /// <summary> Air: 0b_1111_1111 => 0xFF => 255
    }
}
