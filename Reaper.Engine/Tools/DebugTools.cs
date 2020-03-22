using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine.Behaviors;
using System.Collections.Generic;
using System.Linq;

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

        public static void Draw(SpriteBatch batch, IEnumerable<WorldObject> worldObjects)
        {
            if (!_draw)
                return;

            DrawBounds(batch, worldObjects);
            DrawLOS(batch, worldObjects);
        }

        private static void DrawBounds(SpriteBatch batch, IEnumerable<WorldObject> worldObjects) 
        {
            foreach (var wo in worldObjects) 
            {
                Rectangle destination = new Rectangle(
                    (int)(wo.Position.X - wo.Origin.X), 
                    (int)(wo.Position.Y - wo.Origin.Y), 
                    wo.Bounds.Width,
                    wo.Bounds.Height);

                batch.Draw(_texture, destination, null, new Color(150, 0, 0, 50));
            }
        }

        private static void DrawLOS(SpriteBatch batch, IEnumerable<WorldObject> worldObjects)
        {
            foreach (var losBehavior in worldObjects.Where(wo => wo.GetBehavior<LineOfSightBehavior>() != null).Select(wo => wo.GetBehavior<LineOfSightBehavior>()))
            {
                var wo = losBehavior.Owner;
                var bounds = wo.Bounds;
                var mirrored = wo.IsMirrored;
                var distance = losBehavior.Distance;

                Rectangle ray = new Rectangle(
                    mirrored ? (int)wo.Position.X - distance : (int)wo.Position.X,
                    bounds.Top,
                    distance,
                    bounds.Height);

                batch.Draw(_texture, ray, null, new Color(0, 50, 0, 50));
            }
        }
    }
}
