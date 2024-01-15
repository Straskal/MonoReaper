using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class CameraArea : Entity
    {
        private Player player;
        private bool isMoving;
        private float duration;

        public CameraArea(RectangleF bounds)
        {
            Bounds = bounds;
            Position = new Vector2(Bounds.X, Bounds.Y);
        }

        public RectangleF Bounds { get; }

        public override void Start()
        {
            player = World.First<Player>();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            if (Bounds.Contains(player.Position))
            {
                if (!isMoving) 
                {
                    duration = 5f;
                    isMoving = true;
                }
                duration -= gameTime.GetDeltaTime();
                if (duration < 1f)
                {
                    duration = 1f;
                }
                var direction = Bounds.Center - Adventure.Instance.Camera.Position;
                Adventure.Instance.Camera.Position = Adventure.Instance.Camera.Position + direction * (1f / duration);
            }
            else 
            {
                isMoving = false;
            }
        }

        public override void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.Purple);
        }
    }
}
