using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using static Adventure.Constants;

namespace Adventure.Components
{
    public class Player : Entity
    {
        public const float Speed = 1000f;
        public const float MaxSpeed = 0.75f;

        private AnimatedSprite animatedSprite;
        private Vector2 direction = Vector2.One;
        private Vector2 velocity = Vector2.Zero;

        protected override void OnLoad(ContentManager content)
        {
            Fireball.Preload(content);
        }

        protected override void OnSpawn()
        {
            Collider = new CircleCollider(this, new Vector2(0f, 0f), 6f);
            Collider.Layer = EntityLayers.Player;
            Collider.AddResolver(EntityLayers.Projectile, CollisionResolvers.Ignore);
            GraphicsComponent = animatedSprite = new AnimatedSprite(this, SharedContent.Graphics.Player, PlayerAnimations.Frames);
        }

        protected override void OnStart()
        {
            Level.Camera.Position = Position;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();
            var movementInput = Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);
            var movementLength = movementInput.LengthSquared();

            // Normalize movement input so that the player doesn't travel faster diagonally.
            if (movementLength > 1f)
            {
                movementInput.Normalize();
            }

            animatedSprite.IsPaused = movementLength == 0f;
            direction = movementLength > 0f ? movementInput : direction;
            velocity = movementInput * Speed * deltaTime;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);

            const int LAYER = EntityLayers.Solid | BoxLayers.Damage | BoxLayers.Trigger;
            const int IGNORE = EntityLayers.PlayerProjectile;

            Collide(velocity, LAYER, IGNORE);
            Animate();
            HandleInteractInput(deltaTime);
            CameraFollow();
        }

        private void CameraFollow()
        {
            Level.Camera.Position = Vector2.SmoothStep(Level.Camera.Position, Position, 0.15f);
        }

        private void HandleInteractInput(float deltaTime)
        {
            if (Input.IsKeyPressed(Keys.E))
            {
                ShootFireball(deltaTime);
            }
        }

        private void ShootFireball(float deltaTime)
        {
            Level.Spawn(new Fireball(direction * 100f * deltaTime), Collider.Bounds.Center + direction);
        }

        protected override void OnCollision(Entity other, Collision collision)
        {
            if (!Collider.IsMoving)
            {
                return;
            }

            //if (other is Barrel barrel)
            //{
            //    barrel.Push(-collision.Normal);
            //}
        }

        private void Animate()
        {
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X < 0f)
                {
                    animatedSprite.Play("walk_left");
                }
                else
                {
                    animatedSprite.Play("walk_right");
                }
            }
            else
            {
                if (direction.Y < 0f)
                {
                    animatedSprite.Play("walk_up");
                }
                else
                {
                    animatedSprite.Play("walk_down");

                }
            }
        }
    }
}
