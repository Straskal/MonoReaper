namespace Reaper
{
    public class OgmoEntityValues
    {
        public int DrawOrder { get; set; }
        public string Level { get; set; }
        public string SpawnPointName { get; set; }
        public string SpawnPoint { get; set; }
        public string Door { get; set; }
        public string Id { get; set; }
        public bool Horizontal { get; set; }
    }

    public class OgmoEntity
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FlippedX { get; set; }
        public OgmoEntityValues Values { get; set; }
    }
}
