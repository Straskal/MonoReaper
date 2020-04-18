using Reaper.Engine;

namespace Reaper
{
    public static class PlayerSpawnPoint
    {
        [RequiredByLayoutLoad]
        public static WorldObjectDefinition Definition() 
        {
            var playerSpawnPoint = new WorldObjectDefinition();
            playerSpawnPoint.SetTags("spawnPoint");
            playerSpawnPoint.MakeDecal();
            playerSpawnPoint.LoadFromOgmo((wo, oe) => 
            {
                var playerInstance = wo.Layout.Objects.Create(Player.Definition(), wo.Position);
                playerInstance.IsMirrored = wo.IsMirrored;
                wo.Destroy();
            });
            return playerSpawnPoint;
        }
    }
}
