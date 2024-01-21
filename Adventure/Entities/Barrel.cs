using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public sealed class Barrel : KinematicEntity, IDamageable
    {
        private Vector2 push;

        public int Health { get; private set; } = 3;
        public bool Flammable => true;
        public Sprite Sprite { get; private set; }
        public float HurtTimer { get; set; }

        public override void Spawn()
        {
            Health = Random.Shared.Next(2, 5);
            Collider = new BoxCollider(this, 16, 16);
            Collider.Layer = EntityLayers.Enemy | EntityLayers.Solid;
            Collider.Enable();
            GraphicsComponent = Sprite = new Sprite(this, Store.Gfx.Barrel)
            {
                SourceRectangle = new Rectangle(0, 0, 16, 16)
            };
        }

        public void Push(Vector2 direction)
        {
            push += direction;
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
                SlideMove(push);
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
                Sprite.Effect = Store.Vfx.SolidColor;
                HurtTimer = 0.1f;
            }
        }

        public override void Destroy()
        {
            World.Spawn(new Explosion(), Position);
            base.Destroy();
        }
    }
}
