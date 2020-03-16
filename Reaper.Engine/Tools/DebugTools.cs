using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Reaper.Engine.Tools
{
    public static class DebugTools
    {
        private static Texture2D _texture;

        private static bool _draw = false;
        private static KeyboardState _previous;

        public static void Initialize(GraphicsDevice gpu) 
        {
            _texture = new Texture2D(gpu, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        public static void Tick() 
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.OemTilde) && _previous.IsKeyUp(Keys.OemTilde))
            {
                _draw = !_draw;
            }

            _previous = ks;
        }

        public static void DrawBounds(SpriteBatch batch, IEnumerable<WorldObject> worldObjects) 
        {
            if (!_draw)
                return;

            foreach (var wo in worldObjects) 
            {
                Rectangle destination = new Rectangle((int)(wo.Position.X - wo.Origin.X), (int)(wo.Position.Y - wo.Origin.Y), wo.Bounds.Width, wo.Bounds.Height);

                batch.Draw(_texture, destination, null, new Color(255, 0, 0, 50));
            }
        }
    }
}
