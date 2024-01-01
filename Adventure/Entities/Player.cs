using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using static Adventure.Constants;

namespace Adventure.Components
{
    public class Player : Entity
    {
        private const int MovementCollisionLayerMask = EntityLayers.Enemy | EntityLayers.Solid | BoxLayers.Interactable;

        public const float Speed = 1000f;
        public const float MaxSpeed = 0.75f;

        private Vector2 _direction = Vector2.One;
        private Vector2 _velocity = Vector2.Zero;

        protected override void OnLoad(ContentManager content)
        {
            Fireball.Preload(content);
            Collider = new CircleCollider(this, Vector2.Zero, 6f, EntityLayers.Player);
            //Collider = new BoxCollider(this, 12, 16, EntityLayers.Player);
            GraphicsComponent = AnimatedSprite = new AnimatedSprite(this, SharedContent.Graphics.Player, PlayerAnimations.Frames);
        }

        public AnimatedSprite AnimatedSprite 
        { 
            get; 
            private set; 
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

            AnimatedSprite.IsPaused = movementLength == 0f;

            _direction = movementLength > 0f ? movementInput : _direction;
            _velocity = movementInput * Speed * deltaTime;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -MaxSpeed, MaxSpeed);

            MoveAndCollide(ref _velocity, MovementCollisionLayerMask, HandleCollision);
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
            Level.Spawn(new Fireball(_direction * 100f * deltaTime), Collider.Bounds.Center + _direction);
        }

        private static Vector2 HandleCollision(Collision collision)
        {
            if (collision.Collider.IsSolid())
            {
                if (collision.Collider.Entity is Barrel barrel)
                {
                    var velocity = -collision.Normal * 0.5f;
                    collision.Collider.MoveAndCollide(ref velocity, MovementCollisionLayerMask, coll => coll.Slide());
                }

                return collision.Slide();
            }

            return collision.Ignore();
        }

        private void Animate()
        {
            if (Math.Abs(_direction.X) > Math.Abs(_direction.Y))
            {
                if (_direction.X < 0f)
                {
                    AnimatedSprite.Play("walk_left");
                }
                else
                {
                    AnimatedSprite.Play("walk_right");
                }
            }
            else
            {
                if (_direction.Y < 0f)
                {
                    AnimatedSprite.Play("walk_up");
                }
                else
                {
                    AnimatedSprite.Play("walk_down");

                }
            }
        }
    }
}
