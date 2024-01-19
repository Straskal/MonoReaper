using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class CameraArea
    {
        private bool reset = true;
        private float duration;

        public CameraArea(RectangleF bounds)
        {
            Bounds = bounds;
            Position = new Vector2(Bounds.X, Bounds.Y);
        }

        public Vector2 Position { get; }
        public RectangleF Bounds { get; }

        public void CheckForPlayer(GameTime gameTime)
        {
            var player = Adventure.Instance.World.Find<Player>();

            if (player == null) 
            {
                return;
            }

            if (!Bounds.Contains(player.Position)) 
            {
                reset = true;
                return;
            }

            if (Adventure.Instance.Camera.Position == Bounds.Center) 
            {
                Adventure.Instance.isTransitioningAreas = false;
                return;
            }

            if (!reset) 
            {
                return;
            }

            if (duration == 0f) 
            {
                duration = 6f; 
                Adventure.Instance.isTransitioningAreas = true;
            }

            duration -= gameTime.GetDeltaTime();

            if (Vector2.Distance(Bounds.Center, Adventure.Instance.Camera.Position) < 0.5f)
            {
                Adventure.Instance.Camera.Position = Bounds.Center;
                duration = 0f;
                reset = false;
            }
            else 
            {
                var direction = Bounds.Center - Adventure.Instance.Camera.Position;

                Adventure.Instance.Camera.Position = Adventure.Instance.Camera.Position + direction * (1f / duration);
            }   
        }
    }
}
