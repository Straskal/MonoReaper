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
            Box.Width = Width;
            Box.Height = Height;
            Box.LayerMask = BoxLayers.Interactable;
        }

        protected override void OnStart()
        {
            Box.CollidedWith += OnCollidedWith;
        }

        protected override void OnEnd()
        {
            Box.CollidedWith -= OnCollidedWith;
        }

        private void OnCollidedWith(Box body, Collision collision) 
        {
            if (body.LayerMask == EntityLayers.Player) 
            {
                Level.Screens.SetTop(new LevelTransitionScreen(Level.Application, LevelLoader.LoadLevel(Level.Application, LevelPath, SpawnPointId)));
            }
        }
    }
}
