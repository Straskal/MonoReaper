namespace Reaper.Ogmo.Models
{
    public class OgmoEntityValues 
    {
        public int DrawOrder { get; set; }
        public string Level { get; set; }
        public string SpawnPointName { get; set; }
        public string SpawnPoint { get; set; }
    }
    
    public class OgmoEntity
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FlippedX { get; set; }
        public OgmoEntityValues Values { get; set; }
    }
}
