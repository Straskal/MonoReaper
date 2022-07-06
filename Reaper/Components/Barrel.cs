using Core;
using Core.Collision;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using static Reaper.Constants;

namespace Reaper.Components
{
    public interface IDamageable 
    {
        void Damage(int amount);
    }

    public sealed class Barrel : Component, IDamageable
    {
        private int health = 3;

        private Sprite _sprite;
        private Effect _hurtEffect;
        private float _hurtTimer;

        public override void OnLoad(ContentManager content)
        {
            Phial.Preload(content);

            var texture = content.Load<Texture2D>("art/common/barrel");

            _hurtEffect = content.Load<Effect>("shaders/SolidColor");

            Entity.AddComponent(_sprite = new Sprite(texture, new Rectangle(0, 0, 16, 16)));
            Entity.AddComponent(new Box(16, 16, true, EntityLayers.Enemy));
        }

        public override void OnDestroy()
        {
            var loot = new Entity(Origin.BottomCenter);
            loot.AddComponent(new Phial());
            Level.Spawn(loot, Entity.Position);
        }

        public override void OnPostTick(GameTime gameTime)
        {
            float delta = gameTime.GetDeltaTime();

            _hurtTimer -= delta;

            if (_hurtTimer < 0f)
            {
                _sprite.Effect = null;
            }
        }

        public void Damage(int amount)
        {
            health -= amount;

            if (health < 0)
            {
                Level.Destroy(Entity);
            }
            else 
            {
                _sprite.Effect = _hurtEffect;
                _hurtTimer = 0.1f;
            }
        }
    }
}
