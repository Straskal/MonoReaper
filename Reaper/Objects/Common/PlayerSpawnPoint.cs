using Reaper.Engine;

namespace Reaper.Objects.Common
{
    public static class PlayerSpawnPoint
    {
        public static WorldObjectDefinition Method() 
        {
            var playerSpawnPoint = new WorldObjectDefinition();

            playerSpawnPoint.AddBehavior(wo => new OnLoad(wo, () => 
            {
                var layout = wo.Layout;

                var playerInstance = layout.Spawn(Definitions.Get("playerInstance"), wo.Position);
                playerInstance.IsMirrored = wo.IsMirrored;
                playerInstance.ZOrder = wo.ZOrder;
            }));

            return playerSpawnPoint;
        }
    }
}
