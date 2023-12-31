using Adventure.Content;
using Engine;
using Engine.Collision;
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
            Collider = new Box(this, Width, Height, BoxLayers.Interactable);
        }

        protected override void OnStart()
        {
            Collider.CollidedWith += OnCollidedWith;
        }

        protected override void OnEnd()
        {
            Collider.CollidedWith -= OnCollidedWith;
        }

        private void OnCollidedWith(Collider body, Collision collision) 
        {
            if (body.LayerMask == EntityLayers.Player) 
            {
                Level.Screens.SetTop(new LevelTransitionScreen(Level.Application, LevelLoader.LoadLevel(Level.Application, LevelPath, SpawnPointId)));
            }
        }
    }
}
