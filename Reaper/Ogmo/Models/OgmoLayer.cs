using System.Collections.Generic;

namespace Reaper
{
    public class OgmoLayer
    {
        public string Name { get; set; }
        public int GridCellWidth { get; set; }
        public int GridCellHeight { get; set; }
        public int GridCellsX { get; set; }
        public int GridCellsY { get; set; }
        public string Tileset { get; set; }
        public int[] Data { get; set; }
        public List<OgmoEntity> Entities { get; set; }
    }
}
