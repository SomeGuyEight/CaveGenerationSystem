using System.Linq;
using System.Collections.Generic;

namespace SlimeGame
{
    public static class MCTileExtensions
    {
        private readonly static HashSet<int> _defaultValues = MCTileHelper.DefaultMCTiles().Select(x => (int)x).ToHashSet();

        private static bool IsDefault(int value) => _defaultValues.Contains(value);
        private static bool IsValidMCTile(int value) => value > -1 && value < 256;

        public static bool IsDefaultMCTile(this MCTile mCTile) => IsDefault((int)mCTile);
        public static bool IsValidMCTile(this MCTile value) => IsValidMCTile((int)value);
        public static bool IsVoidMCTile(this MCTile mCTile) => mCTile == MCTile._00_0000_0000;
        public static bool IsAirMCTile(this MCTile mCTile) => mCTile == MCTile._80_1111_1111;

        public static bool IsVoidOrAirMCTile(this MCTile mCTile,out CellTypes cellTypes)
        {
            if (mCTile.IsVoidMCTile())
            {
                cellTypes = CellTypes.Void;
                return true;
            }
            if (mCTile.IsAirMCTile())
            {
                cellTypes = CellTypes.Air;
                return true;
            }
            cellTypes = CellTypes.None;
            return false;
        }
    }
}
