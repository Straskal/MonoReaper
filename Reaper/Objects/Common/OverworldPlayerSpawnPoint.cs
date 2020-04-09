using Reaper.Engine;
using Reaper.Ogmo;

namespace Reaper.Objects.Common
{
    [Definition]
    public static class OverworldPlayerSpawnPoint
    {
        static OverworldPlayerSpawnPoint()
        {
            Definitions.Register("overworld_player_spawn", Definition);
        }

        public static WorldObjectDefinition Definition() 
        {
            var playerSpawnPoint = new WorldObjectDefinition();
            playerSpawnPoint.LoadFromOgmo((wo, oe) => 
            {
                var layout = wo.Layout;
                var playerInstance = layout.Spawn(OverworldPlayer.Definition(), wo.Position);
                playerInstance.IsMirrored = wo.IsMirrored;
                playerInstance.ZOrder = wo.ZOrder;
                wo.Destroy();
            });
            return playerSpawnPoint;
        }
    }
}
