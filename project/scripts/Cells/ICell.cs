using Sylves;

namespace SlimeGame
{
    public interface ICell
    {   
        public Cell Cell { get; set; }
        public CellTypes CellTypes { get; set; }
        public bool Update { get; set; }
    }
}
