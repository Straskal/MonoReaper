using Reaper.Engine;
using Reaper.Ogmo;

namespace Reaper.Objects.Common
{
    public static class PlayerSpawnPoint
    {
        public static WorldObjectDefinition Method() 
        {
            var playerSpawnPoint = new WorldObjectDefinition();

            playerSpawnPoint.LoadFromOgmo((wo, oe) => 
            {
                // Spawn a player instance and destroy self.
                var layout = wo.Layout;

                var playerInstance = layout.Spawn(Definitions.Get("playerInstance"), wo.Position);
                playerInstance.IsMirrored = wo.IsMirrored;
                playerInstance.ZOrder = wo.ZOrder;

                wo.Destroy();
            });

            return playerSpawnPoint;
        }
    }
}
