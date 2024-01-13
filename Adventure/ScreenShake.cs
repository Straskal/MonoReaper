using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace Adventure
{
    public static class ScreenShake
    {
        private static float duration;
        private static Vector2 position = Vector2.Zero;

        public static void Shake(float duration) 
        {
            ScreenShake.duration = MathF.Max(duration, ScreenShake.duration);
        }

        public static void Update(GameTime gameTime) 
        {
            if (duration > 0f)
            {
                if (position == Vector2.Zero)
                {
                    position = Adventure.Instance.Camera.Position;
                }
                duration -= gameTime.GetDeltaTime();
                var angle = Random.Shared.Next(360);
                var x = position.X + 1f * MathF.Cos(angle);
                var y = position.Y + 1f * MathF.Sin(angle);
                Adventure.Instance.Camera.Position = new Vector2(x, y);
            }
            else if (position != Vector2.Zero)
            {
                Adventure.Instance.Camera.Position = position;
                position = Vector2.Zero;
            }
        }
    }
}
