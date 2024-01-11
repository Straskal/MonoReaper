using Adventure.Content;
using Engine;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class LevelTrigger : Entity
    {
        public LevelTrigger(EntityData entityData)
        {
            // Concat should be in the level reader.
            LevelPath = "Levels/" + entityData.Fields.GetString("LevelPath");
            SpawnPointId = entityData.Fields.GetString("PlayerSpawnId");
            Width = entityData.Width;
            Height = entityData.Height;
        }

        public string LevelPath { get; }
        public string SpawnPointId { get; }
        public int Width { get; }
        public int Height { get; }

        protected override void OnSpawn()
        {
            Collider = new BoxCollider(this, Width, Height, EntityLayers.Trigger);
            Collider.Enable();
        }

        protected override void OnCollision(Entity other, Collision collision) 
        {
            if (other is Player) 
            {
                Level.Application.ChangeScreen(new LevelTransitionScreen(Level.Application, LevelLoader.LoadLevel(Level.Application, LevelPath, SpawnPointId)));
            }
        }
    }
}
