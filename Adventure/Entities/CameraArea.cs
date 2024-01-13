using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class CameraArea : Entity
    {
        private Player player;

        public CameraArea(RectangleF bounds)
        {
            Bounds = bounds;
            Position = new Vector2(Bounds.X, Bounds.Y);
        }

        public RectangleF Bounds { get; }

        public override void Start()
        {
            player = World.Find<Player>();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            if (Bounds.Contains(player.Position)) 
            {
                Adventure.Instance.Camera.Position = Vector2.SmoothStep(Adventure.Instance.Camera.Position, Bounds.Center, 0.17f);
            }
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.Purple);
        }
    }
}
