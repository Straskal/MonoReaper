using Microsoft.Xna.Framework;

namespace Core
{
    public static class GameTimeExtensions
    {
        public static float GetDeltaTime(this GameTime gameTime) 
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
