using Microsoft.Xna.Framework;

namespace Reaper.Engine
{
    public static class GameTimeExtensions
    {
        public static float GetDeltaTime(this GameTime gameTime) 
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
