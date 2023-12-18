using Engine;
using Engine.Collision;
using Adventure.Content;

using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class LevelTrigger : Component
    {
        private Box _box;

        public LevelTrigger(int width, int height, EntityFields fields)
        {
            // Concat should be in the level reader.
            LevelPath = "Levels/" + fields.GetString("LevelPath");
            SpawnPointId = fields.GetString("PlayerSpawnId");
            Width = width;
            Height = height;
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
