using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Collision;
using Engine.Graphics;

using static Adventure.Constants;

namespace Adventure.Components
{
    public class Player : Entity
    {
        // When the player is moving, it should collide with enemies and solid entities.
        // Could also eventually add other layers here. Like health pickups, interactables, hazards, etc..
        private const int MovementCollisionLayerMask = EntityLayers.Enemy | EntityLayers.Solid | BoxLayers.Interactable;

        public const float Speed = 1000f;
        public const float MaxSpeed = 0.85f;

        private Body _body;
        private AnimatedSprite _sprite;

        private Vector2 _direction = Vector2.One;
        private Vector2 _velocity = Vector2.Zero;

        public override void OnLoad(ContentManager content)
        {
            Fireball.Preload(content);

            AddComponent(_sprite = new AnimatedSprite(SharedContent.Graphics.Player, PlayerAnimations.Frames));
            AddComponent(_body = new Body(12, 16, EntityLayers.Player));
        }

        public override void OnStart()
        {
            Level.Camera.Position = Position;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();
            var movementInput = Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);
            var movementLength = movementInput.LengthSquared();

            // Normalize movement input so that the player doesn't travel faster diagonally.
            if (movementLength > 1f)
            {
                movementInput.Normalize();
            }

            _sprite.IsPaused = movementLength == 0f;
            _direction = movementLength > 0f ? movementInput : _direction;
            _velocity = movementInput * Speed * deltaTime;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);
            _velocity.Y = MathHelper.Clamp(_velocity.Y, -MaxSpeed, MaxSpeed);

            Move();
            Animate();
            HandleInteractInput(deltaTime);
            CameraFollow(); 
        }

        private void Move() 
        {
            _body.MoveAndCollide(ref _velocity, MovementCollisionLayerMask, HandleCollision);
        }

        private void CameraFollow() 
        {
            Level.Camera.Position = Vector2.SmoothStep(Level.Camera.Position, Position, 0.15f);
        }

        private void HandleInteractInput(float deltaTime) 
        {
            if (Input.IsKeyPressed(Keys.E))
            {
                Spawn(new Fireball(_direction * 100f * deltaTime), _body.CalculateBounds().Center + _direction * 10f);
            }
        }

        private static Vector2 HandleCollision(Collision collision)
        {
            if (collision.Box.IsSolid())
            {
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
                    _sprite.Play("walk_left");
                }
                else
                {
                    _sprite.Play("walk_right");
                }
            }
            else
            {
                if (_direction.Y < 0f)
                {
                    _sprite.Play("walk_up");
                }
                else
                {
                    _sprite.Play("walk_down");

                }
            }
        }
    }
}
