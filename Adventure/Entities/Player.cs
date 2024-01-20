using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class Player : KinematicEntity
    {
        public const float Speed = 1000f;
        public const float MaxSpeed = 0.75f;

        private AnimatedSprite animatedSprite;
        private Vector2 direction = Vector2.One;
        private bool isAiming;

        public override void Spawn()
        {
            Collider = new BoxCollider(this, 0, 4, 10, 8);
            Collider.Layer = EntityLayers.Player;
            Collider.Enable();
            GraphicsComponent = animatedSprite = new AnimatedSprite(this, Store.Gfx.Player, PlayerAnimations.Frames)
            {
                DrawOrder = 5
            };
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();
            var movementInput = Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);
            var movementLength = movementInput.LengthSquared();
            
            if (movementLength > 1f)
            {
                movementInput.Normalize();
            }

            animatedSprite.IsPaused = movementLength == 0f;

            if (isAiming)
            {
                var mousePosition = Adventure.Instance.Camera.ToWorld(Input.MousePosition) - new Vector2(4, 4);
                direction = mousePosition - Position;
                direction.Normalize();
            }
            else 
            {
                direction = movementLength > 0f ? movementInput : direction;
            }

            var velocity = movementInput * Speed * deltaTime;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);

            SlideMove(velocity);
            Animate();
            HandleInteractInput(deltaTime);
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            base.Draw(renderer, gameTime);
            DrawCursor(renderer);
        }

        private void HandleInteractInput(float deltaTime)
        {
            isAiming = Input.IsMouseRightDown();

            if (isAiming) 
            {
                if (Input.IsMouseLeftPressed())
                {
                    var mousePosition = Adventure.Instance.Camera.ToWorld(Input.MousePosition);
                    direction = mousePosition - Position;
                    direction.Normalize();
                    World.Spawn(new Fireball(direction * 100f * deltaTime), Position);
                }
            }

            if (Input.IsKeyPressed(Keys.E))
            {
                ShootFireball(deltaTime);
            }
        }

        private void ShootFireball(float deltaTime)
        {
            World.Spawn(new Fireball(direction * 100f * deltaTime), Position);
        }

        public override void OnCollision(Entity other, Collision collision)
        {
            if (!IsMoving)
            {
                return;
            }

            if (other is Barrel barrel)
            {
                barrel.Push(-collision.Intersection.Normal);
            }
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

        private void DrawCursor(Renderer renderer)
        {
            var source = Input.IsMouseLeftDown()
                ? isAiming ? new Rectangle(8, 0, 8, 8) : new Rectangle(0, 0, 8, 8)
                : isAiming ? new Rectangle(16, 0, 8, 8) : new Rectangle(8, 0, 8, 8);

            var cursorOffset = source.Size.ToVector2() / 2f;
            var cursorPosition = Adventure.Instance.Camera.ToWorld(Input.MousePosition) - cursorOffset;

            renderer.Draw(Store.Gfx.Cursor, cursorPosition, source);
        }
    }
}
