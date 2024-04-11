using Sylves;

namespace SlimeGame
{
    public static class CellTypesExtensions
    {
        public static bool IsNone(this CellTypes a) => a == 0;
        public static bool IsSingleFlag(this CellTypes a) => a != 0 && (a & (a - 1)) == 0;
        public static bool HasFlags(this CellTypes a,CellTypes b) => (a & b) == b;
        public static CellTypes SharedFlags(this CellTypes a,CellTypes b) => a & b;
        public static CellTypes ToggleFlags(this CellTypes a,CellTypes b) => a ^= b;
        public static CellTypes SetFlags(this CellTypes a,CellTypes b) => a | b;
        public static CellTypes UnsetFlags(this CellTypes a,CellTypes b) => a & (~b);


        public static CellTypes[][,] To1D2D(this CellTypes[,,] inputArray)
        {
            if (inputArray == null)
            {
                return new CellTypes[0][,];
            }
            var max = new Cell(inputArray.GetLength(0),inputArray.GetLength(1),inputArray.GetLength(2));
            var outputArray = new CellTypes[max.z][,];
            for (int i = 0;i < max.z;i++)
            {
                outputArray[i] = new CellTypes[max.x,max.y];
            }
            for (int x = 0;x < max.x;x++)
            {
                for (int y = 0;y < max.y;y++)
                {
                    for (int z = 0;z < max.z;z++)
                    {
                        outputArray[z][x,y] = inputArray[x,y,z];
                    }
                }
            }
            return outputArray;
        }
        public static CellTypes[,,] To3D(this CellTypes[][,] inputArray)
        {
            if (inputArray == null)
            {
                return new CellTypes[0,0,0];
            }
            var max = new Cell(inputArray[0].GetLength(0),inputArray[0].GetLength(1),inputArray.Length);
            var outputArray = new CellTypes[max.x,max.y,max.z];
            for (int x = 0;x < max.x;x++)
            {
                for (int y = 0;y < max.y;y++)
                {
                    for (int z = 0;z < max.z;z++)
                    {
                        outputArray[x,y,z] = inputArray[z][x,y];
                    }
                }
            }
            return outputArray;
        }

        public static CellTypes[][,] DeepClone(this CellTypes[][,] inputArray)
        {
            if (inputArray == null)
            {
                return new CellTypes[0][,];
            }
            if (inputArray[0] == null)
            {
                return new CellTypes[inputArray.Length][,];
            }
            var max = new Cell(inputArray[0].GetLength(0),inputArray[0].GetLength(1),inputArray.Length);
            var outputArray = new CellTypes[max.z][,];
            for (int i = 0;i < inputArray.Length;i++)
            {
                outputArray[i] = new CellTypes[max.x,max.y];
            }
            for (int x = 0;x < max.x;x++)
            {
                for (int y = 0;y < max.y;y++)
                {
                    for (int z = 0;z < max.z;z++)
                    {
                        outputArray[z][x,y] = inputArray[z][x,y];
                    }
                }
            }
            return outputArray;
        }
        public static CellTypes[,,] DeepClone(this CellTypes[,,] inputArray)
        {
            if (inputArray == null)
            {
                return new CellTypes[0,0,0];
            }
            var max = new Cell(inputArray.GetLength(0),inputArray.GetLength(1),inputArray.GetLength(2));
            var outputArray = new CellTypes[max.x,max.y,max.z];
            for (int x = 0;x < max.x;x++)
            {
                for (int y = 0;y < max.y;y++)
                {
                    for (int z = 0;z < max.z;z++)
                    {
                        outputArray[x,y,z] = inputArray[x,y,z];
                    }
                }
            }
            return outputArray;
        }

    }
}
