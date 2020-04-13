using Reaper.Engine;
using Reaper;

namespace Reaper
{
    [Definition]
    public static class OverworldPlayerSpawnPoint
    {
        static OverworldPlayerSpawnPoint()
        {
            DefinitionList.Register(typeof(OverworldPlayerSpawnPoint), Definition);
        }

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
