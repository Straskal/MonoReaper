using Core;
using Core.Graphics;

namespace Reaper.Components
{
    public sealed class Barrel : Component
    {
        public override void OnDestroy()
        {
            var loot = new Entity(Origin.BottomCenter);

            loot.AddComponent(new Sprite("art/common/phial", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16)));

            Level.Spawn(loot, Entity.Position);
        }
    }
}
