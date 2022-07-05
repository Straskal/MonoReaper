using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Core;
using Core.Graphics;

namespace Reaper.Components
{
    public sealed class Phial : Component
    {
        public static void Preload(ContentManager content) 
        {
            content.Load<Texture2D>("art/common/phial");
        }

        public override void OnLoad(ContentManager content)
        {
            var texture = content.Load<Texture2D>("art/common/phial");

            Entity.AddComponent(new Sprite(texture, new Rectangle(0, 0, 16, 16)));
        }
    }
}
