using Microsoft.Xna.Framework;

namespace Engine.Extensions
{
    public static class GameTimeExtensions
    {
        public static float GetDeltaTime(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
