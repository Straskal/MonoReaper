using Engine;
using Engine.Extensions;
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

    public sealed class Barrel : Entity, IDamageable
    {
        private Vector2 push;

        public int Health { get; private set; } = 3;
        public bool Flammable { get => true; }
        public Sprite Sprite { get; private set; }
        public Effect HurtEffect { get; private set; }
        public float HurtTimer { get; set; }

        public override void Spawn()
        {
            HurtEffect = Adventure.Instance.Content.Load<Effect>("shaders/SolidColor");
            Collider = new CircleCollider(this, new Vector2(0, 3), 6, EntityLayers.Enemy | EntityLayers.Solid);
            //Collider = new BoxCollider(this, 12, 12, EntityLayers.Enemy | EntityLayers.Solid);
            Collider.Enable();
            GraphicsComponent = Sprite = new Sprite(this, Adventure.Instance.Content.Load<Texture2D>("art/common/barrel"))
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16)
            };
        }

        public void Push(Vector2 direction)
        {
            push += direction * 0.5f;
        }

        public override void Update(GameTime gameTime)
        {
            float delta = gameTime.GetDeltaTime();

            HurtTimer -= delta;

            if (HurtTimer < 0f)
            {
                Sprite.Effect = null;
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            if (push != Vector2.Zero)
            {
                Collide(ref push, EntityLayers.Solid);
                push = Vector2.Zero;
            }

            base.PostUpdate(gameTime);
        }

        public void Damage(int amount)
        {
            Health -= amount;

            if (Health < 0)
            {
                World.Destroy(this);
            }
            else
            {
                Sprite.Effect = HurtEffect;
                HurtTimer = 0.1f;
            }
        }
    }
}
