namespace Reaper.Ogmo.Models
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
        public OgmoEntity[] Entities { get; set; }
    }
}
