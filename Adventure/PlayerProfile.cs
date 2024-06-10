namespace Adventure
{
    public class PlayerProfile
    {
        // Used by server
        public int PeerId { get; set; }
        public int Id { get; set; }
        public bool IsProfileLoaded { get; set; }
        public string Name { get; set; }
    }
}
