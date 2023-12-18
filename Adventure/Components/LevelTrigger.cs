using Engine;
using Engine.Collision;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class LevelTrigger : Component
    {
        private Box _box;

        public LevelTrigger(string level, int width, int height)
        {
            LevelName = level;
            Width = width;
            Height = height;
        }

        public string LevelName { get; }
        public int Width { get; }
        public int Height { get; }

        public override void OnSpawn()
        {
            Entity.AddComponent(_box = new Box(Width, Height));
            _box.LayerMask = BoxLayers.Interactable;
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
            Level.Screens.SetTop(new LevelTransitionScreen(Level.Application, LevelLoader.LoadLevel(Level.Application, "Levels/world/level_0")));
        }
    }
}
