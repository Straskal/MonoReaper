namespace Reaper.Ogmo
{
    public class OgmoEntityValues 
    {
        public int DrawOrder { get; set; }
        public string Level { get; set; }
    }

    public class OgmoEntity
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int OriginX { get; set; }
        public int OriginY { get; set; }
        public bool FlippedX { get; set; }
        public OgmoEntityValues Values { get; set; }
    }
}
