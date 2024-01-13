using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class LevelArea : Entity
    {
        private static LevelArea current;
        private Player player;

        public LevelArea(RectangleF bounds)
        {
            Bounds = bounds;
            Position = new Vector2(Bounds.X, Bounds.Y);
        }

        public RectangleF Bounds { get; }

        public override void Start()
        {
            player = World.Find<Player>();

            if (player.Overlaps(Bounds))
            {
                current = this;
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            if (player.Overlaps(Bounds))
            {
                current ??= this;
            }
            else if (current == this)
            {
                current = null;
            }

            if (current == this) 
            {
                Adventure.Instance.Camera.Position = Vector2.SmoothStep(Adventure.Instance.Camera.Position, Bounds.Center, 0.16f);
            }

            base.PostUpdate(gameTime);
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.Purple);
        }
    }
}
