using Engine;
using Engine.Collision;
using Adventure.Content;

using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class LevelTrigger : Component
    {
        private Box _box;

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

        public override void OnSpawn()
        {
            Entity.AddComponent(_box = new Box(Width, Height) 
            {
                LayerMask = BoxLayers.Interactable
            });
        }

        public override void OnStart()
        {
            _box.CollidedWith += OnCollidedWith;
        }

        public override void OnEnd()
        {
            _box.CollidedWith -= OnCollidedWith;
        }

        private void OnCollidedWith(Body body, Collision collision) 
        {
            Level.Screens.SetTop(new LevelTransitionScreen(Level.Application, LevelLoader.LoadLevel(Level.Application, LevelPath, SpawnPointId)));
        }
    }
}
