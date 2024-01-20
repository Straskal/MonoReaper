using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace Adventure
{
    public static class ScreenShake
    {
        private static float duration;

        public static void Shake(float duration) 
        {
            ScreenShake.duration = MathF.Max(duration, ScreenShake.duration);
        }

        public static void Update(GameTime gameTime) 
        {
            if (duration > 0f)
            {
                duration -= gameTime.GetDeltaTime();
                var angle = Random.Shared.Next(360);
                var x = 1f * MathF.Cos(angle);
                var y = 1f * MathF.Sin(angle);
                var position = Adventure.Instance.Camera.Position;
                Adventure.Instance.Camera.Position = position + new Vector2(x, y);
            }
        }
    }
}
