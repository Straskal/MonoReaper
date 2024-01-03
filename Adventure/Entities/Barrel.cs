using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Collision;
using Engine.Graphics;

using static Adventure.Constants;

namespace Adventure.Components
{
    public interface IDamageable
    {
        bool Flammable { get; }
        void Damage(int amount);
    }

    public sealed class Barrel : Entity, IDamageable
    {
        public int Health
        {
            get;
            private set;
        } = 3;

        public bool Flammable 
        {
            get => true;
        }

        public Sprite Sprite 
        {
            get;
            private set;
        }

        public Effect HurtEffect 
        {
            get;
            private set;
        }

        public float HurtTimer 
        {
            get;
            set;
        }

        protected override void OnLoad(ContentManager content)
        {
            HurtEffect = content.Load<Effect>("shaders/SolidColor");
            Collider = new CircleCollider(this, new Vector2(0, 4), 6, EntityLayers.Enemy | EntityLayers.Solid);
            GraphicsComponent = Sprite = new Sprite(this, content.Load<Texture2D>("art/common/barrel"))
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16)
            };
        }

        protected override void OnDestroy()
        {
            DropLoot();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            float delta = gameTime.GetDeltaTime();

            HurtTimer -= delta;

            if (HurtTimer < 0f)
            {
                Sprite.Effect = null;
            }
        }

        public void Damage(int amount)
        {
            Health -= amount;

            if (Health < 0)
            {
                Level.Destroy(this);
            }
            else
            {
                Sprite.Effect = HurtEffect;
                HurtTimer = 0.1f;
            }
        }

        private void DropLoot()
        {
        }
    }
}
