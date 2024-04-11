using System;
using System.Collections.Generic;
using Tessera;
using UnityEngine;

namespace SlimeGame
{
    public static class MCTileHelper
    {
        static MCTileHelper() 
        {
            var mCTileIDs = Enum.GetValues(typeof(MCTileID));
            _iDStringToID = new(mCTileIDs.Length);
            _defaultTileToString = new(mCTileIDs.Length);
            _defaultStringToTile = new(mCTileIDs.Length);
            foreach (MCTileID id in mCTileIDs)
            {                
                var mCTile = (MCTile)id;
                var mCTileString = mCTile.ToString();
                _iDStringToID.Add(id.ToString(),id);
                _defaultTileToString.Add(mCTile,mCTileString);
                _defaultStringToTile.Add(mCTileString,mCTile);
            }

            var mCTileTypes = Enum.GetValues(typeof(MCTile));
            _anyTileToID = new(mCTileTypes.Length);
            _anyStringToTile = new (mCTileTypes.Length);
            _anyTileToString = new(mCTileTypes.Length);
            foreach (MCTile tile in mCTileTypes)
            {
                var mCString = tile.ToString();
                _anyTileToID.Add(tile,_iDStringToID[GetIDPrefix(mCString)]);
                _anyTileToString.Add(tile,mCString);
                _anyStringToTile.Add(mCString,tile);
            }
        }

        private readonly static Dictionary<MCTile,(RotationGroupType rotationGroup,bool isRefelctable,bool isSymetric)> _tileOrientationValues = new ()
        {
            { MCTile._00_0000_0000, ( RotationGroupType.None, false, true  ) },
            { MCTile._10_0000_0001, ( RotationGroupType.All , false, true  ) },
            { MCTile._20_0000_0011, ( RotationGroupType.All , false, true  ) },
            { MCTile._21_0000_0110, ( RotationGroupType.All , false, true  ) },
            { MCTile._22_0001_1000, ( RotationGroupType.XZ  , false, true  ) },
            { MCTile._30_0000_0111, ( RotationGroupType.All , false, true  ) },
            { MCTile._31_0001_0110, ( RotationGroupType.All , false, true  ) },
            { MCTile._32_0001_1001, ( RotationGroupType.All , false, true  ) },
            { MCTile._40_0000_1111, ( RotationGroupType.All , false, true  ) },
            { MCTile._41_0001_0111, ( RotationGroupType.All , false, true  ) },
            { MCTile._42_0001_1011, ( RotationGroupType.All , true , true  ) },
            { MCTile._43_0001_1110, ( RotationGroupType.All , false, true  ) },
            { MCTile._44_0011_1100, ( RotationGroupType.All , false, true  ) },
            { MCTile._45_0110_1001, ( RotationGroupType.XY  , false, true  ) },
            { MCTile._50_1111_1000, ( RotationGroupType.All , false, true  ) },
            { MCTile._51_1110_1001, ( RotationGroupType.All , false, true  ) },
            { MCTile._52_1110_0110, ( RotationGroupType.All , false, true  ) },
            { MCTile._60_1111_1100, ( RotationGroupType.All , false, true  ) },
            { MCTile._61_1111_1001, ( RotationGroupType.All , false, true  ) },
            { MCTile._62_1110_0111, ( RotationGroupType.XZ  , false, true  ) },
            { MCTile._70_1111_1110, ( RotationGroupType.All , false, true  ) },
            { MCTile._80_1111_1111, ( RotationGroupType.None, false, true  ) },
        };

        private readonly static Dictionary<string,MCTileID> _iDStringToID;
        private readonly static Dictionary<MCTile,string> _defaultTileToString;
        private readonly static Dictionary<string,MCTile> _defaultStringToTile;
        private readonly static Dictionary<MCTile,MCTileID> _anyTileToID;
        private readonly static Dictionary<MCTile,string> _anyTileToString;
        private readonly static Dictionary<string,MCTile> _anyStringToTile;

        private static string GetTilePrefix(string name) => name != null ? name.Substring(0,13) : string.Empty;
        private static string GetIDPrefix(string name)   => name != null ? name.Substring(0, 3) : string.Empty;

        public static HashSet<MCTile> DefaultMCTiles()
        { 
            HashSet <MCTile> mCTiles = new (_defaultTileToString.Count);
            foreach (MCTile tile in _defaultTileToString.Keys)
            {
                mCTiles.Add(tile);
            }
            return mCTiles;
        }
        public static bool IsDefaultMCTileObject(GameObject go,out MCTile mCTile)
        {
            return IsDefaultMCTile(go != null ? go.name : string.Empty,out mCTile);
        }

        public static bool IsDefaultMCTile(string name,out MCTile mCTile)
        {
            return _defaultStringToTile.TryGetValue(name != null ? name.Substring(0,13) : string.Empty,out mCTile);
        }
        public static bool IsDefaultMCTile(string name)
        {
            return _defaultStringToTile.ContainsKey(name != null ? name.Substring(0,13) : string.Empty);
        }

        public static MCTile? GetMCTile(string name)
        {
            if (_anyStringToTile.TryGetValue(GetTilePrefix(name),out var validMCTile))
            {
                return validMCTile;
            }
            return null;
        }

        public static bool TryGetOrientationValues(this MCTile mCTile,out RotationGroupType rotationGroup,out bool isReflectable,out bool isSymetric)
        {
            if (mCTile.IsDefaultMCTile() && _tileOrientationValues.TryGetValue(mCTile,out var tuple))
            {
                rotationGroup   = tuple.rotationGroup;
                isReflectable   = tuple.isRefelctable;
                isSymetric      = tuple.isSymetric;
                return true;
            }
            rotationGroup   = RotationGroupType.None;
            isReflectable   = false;
            isSymetric      = false;
            return false;
        }

    }
}
