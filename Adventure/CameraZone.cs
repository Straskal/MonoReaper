using Adventure.Entities;
using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Adventure
{
    public class CameraZone
    {
        private readonly RectangleF bounds;
        private bool reset = true;

        public CameraZone(RectangleF bounds)
        {
            this.bounds = bounds;
        }

        public void CheckForPlayer()
        {
            var player = Adventure.Instance.World.Find<Player>();

            if (player == null) 
            {
                return;
            }

            if (!bounds.Contains(player.Position)) 
            {
                reset = true;
                return;
            }

            if (reset)
            {
                Adventure.Instance.Coroutines.Start(Transition());
                reset = false;
            }
            else if (!Adventure.IsTransitioningAreas)
            {
                Adventure.Instance.Camera.Position = bounds.Center;
            }
        }

        private IEnumerator Transition()
        {
            Adventure.IsTransitioningAreas = true;

            var duration = 6f;

            while (Vector2.Distance(bounds.Center, Adventure.Instance.Camera.Position) > 0.5f)
            {
                var direction = bounds.Center - Adventure.Instance.Camera.Position;
                Adventure.Instance.Camera.Position = Adventure.Instance.Camera.Position + direction * (1f / duration);
                duration -= Adventure.Time.GetDeltaTime();
                yield return null;
            }

            Adventure.Instance.Camera.Position = bounds.Center;
            Adventure.IsTransitioningAreas = false;
        }
    }
}
