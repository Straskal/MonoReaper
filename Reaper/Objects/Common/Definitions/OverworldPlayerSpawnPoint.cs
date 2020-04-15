using Reaper.Engine;

namespace Reaper
{
    public static class OverworldPlayerSpawnPoint
    {
        [Definition]
        public static WorldObjectDefinition Definition() 
        {
            var playerSpawnPoint = new WorldObjectDefinition();
            playerSpawnPoint.LoadFromOgmo((wo, oe) => 
            {
                var layout = wo.Layout;
                var playerInstance = layout.Spawn(OverworldPlayer.Definition(), wo.Position);
                playerInstance.IsMirrored = wo.IsMirrored;
                wo.Destroy();
            });
            return playerSpawnPoint;
        }
    }
}
