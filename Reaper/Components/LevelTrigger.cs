using Reaper.Engine;

namespace Reaper.Components
{
    public sealed class LevelTrigger : Component
    {
        public readonly string LevelName;
        public readonly string SpawnPoint;

        public LevelTrigger(string level, string spawnPoint) 
        {
            LevelName = level;
            SpawnPoint = spawnPoint;
        }
    }
}
