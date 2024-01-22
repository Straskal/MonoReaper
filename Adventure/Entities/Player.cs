using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class Player : KinematicEntity
    {
        public const float Speed = 1000f;
        public const float MaxSpeed = 0.75f;

        private AnimatedSprite animatedSprite;
        private CircleCollider hurtArea;
        private Vector2 direction;
        private bool isAiming;
        private PlayerInput input;
        private float attackTimer;

        public void SetInput(PlayerInput input)
        {
            this.input = input;
        }

        public override void Spawn()
        {
            hurtArea = new CircleCollider(this, Vector2.Zero, 6f);
            hurtArea.Layer = BoxLayers.Damageable;
            hurtArea.Enable();

            Collider = new BoxCollider(this, 0f, 4f, 9f, 8f);
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
            var movementInput = input.Move;
            var movementLength = movementInput.LengthSquared();

            if (movementLength > 1f)
            {
                movementInput.Normalize();
            }

            animatedSprite.IsPaused = movementLength == 0f;
            isAiming = input.Aim != Vector2.Zero;

            if (isAiming)
            {
                direction = input.Aim;
            }
            else
            {
                direction = movementLength > 0f ? movementInput : direction;
            }

            attackTimer = MathF.Max(0f, attackTimer - deltaTime);

            if (isAiming && input.Shoot && attackTimer <= 0f)
            {
                attackTimer = 0.5f;
                World.Spawn(new Fireball(direction * 100f * deltaTime), Position);
            }

            var velocity = movementInput * Speed * deltaTime;
            velocity.X = MathHelper.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -MaxSpeed, MaxSpeed);

            SlideMove(velocity);
            hurtArea.Update();
            OverlapTriggers();
            Animate();
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            animatedSprite.Draw(renderer, gameTime);
            DrawCursor(renderer);
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
                    animatedSprite.Play("walk_left");
                else
                    animatedSprite.Play("walk_right");
            }
            else
            {
                if (direction.Y < 0f)
                    animatedSprite.Play("walk_up");
                else
                    animatedSprite.Play("walk_down");
            }
        }

        private void DrawCursor(Renderer renderer)
        {
            var source = input.Shoot
                ? isAiming ? new Rectangle(8, 0, 8, 8) : new Rectangle(0, 0, 8, 8)
                : isAiming ? new Rectangle(16, 0, 8, 8) : new Rectangle(8, 0, 8, 8);

            var cursorOffset = source.Size.ToVector2() / 2f;
            var cursorPosition = Adventure.Instance.Camera.ToWorld(Input.MousePosition) - cursorOffset;

            renderer.Draw(Store.Gfx.Cursor, cursorPosition, source, Color.White);
        }
    }
}
