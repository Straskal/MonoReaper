using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Adventure.Constants;

namespace Adventure.Components
{
    public interface IDamageable
    {
        bool Flammable { get; }
        void Damage(int amount);
    }

    public sealed class Barrel : Component, IDamageable
    {
        private int health = 3;

        private Sprite _sprite;
        private Effect _hurtEffect;
        private float _hurtTimer;

        public bool Flammable => true;

        public override void OnLoad(ContentManager content)
        {
            // Preload loot that drops when barrel is destroyed.
            Phial.Preload(content);
            _hurtEffect = content.Load<Effect>("shaders/SolidColor");
            Entity.AddComponent(_sprite = new Sprite(content.Load<Texture2D>("art/common/barrel"), new Rectangle(0, 0, 16, 16)));
            Entity.AddComponent(new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Solid));
        }

        public override void OnDestroy()
        {
            DropLoot();
        }

        public override void OnUpdate(GameTime gameTime)
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

        private void DropLoot()
        {
        }
    }
}
