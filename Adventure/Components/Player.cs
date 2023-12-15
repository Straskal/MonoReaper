using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Collision;
using Engine.Graphics;
using Engine.Extensions;

using static Adventure.Constants;

namespace Adventure.Components
{
    public class Player : Component
    {
        // When the player is moving, it should collide with enemies and solid entities.
        // Could also eventually add other layers here. Like health pickups, interactables, hazards, etc..
        private const int MovementCollisionLayerMask = EntityLayers.Enemy | EntityLayers.Solid;

        public const float Speed = 1000f;
        public const float MaxSpeed = 0.85f;

        private Body _body;
        private AnimatedSprite _spriteSheet;

        private Vector2 _direction = Vector2.One;
        private Vector2 _velocity = Vector2.Zero;

        public override void OnLoad(ContentManager content)
        {
            Entity.AddComponent(_body = new Body(12, 16, EntityLayers.Player));
            Entity.AddComponent(_spriteSheet = new AnimatedSprite(SharedContent.Gfx.Player, PlayerAnimations.Frames));
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

            _spriteSheet.CurrentAnimation.Loop = movementLength > 0f;
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
            Level.Camera.Position = Vector2.SmoothStep(Level.Camera.Position, Entity.Position, 0.15f);
        }

        private void HandleInteractInput(float deltaTime) 
        {
            if (Input.IsKeyPressed(Keys.E))
            {
                Level.Spawn(new Fireball(_direction * 100f * deltaTime), _body.CalculateBounds().Center + _direction * 10f);
            }
        }

        private Vector2 HandleCollision(Hit hit)
        {
            if (hit.Other.Entity.TryGetComponent<LevelTrigger>(out var transition))
            {
                App.Instance.LoadLevel(transition.LevelName, transition.SpawnPoint);
            }

            if (hit.Other.IsSolid())
            {
                return hit.Slide();
            }

            return hit.Ignore();
        }

        private void Animate()
        {
            if (Math.Abs(_direction.X) > Math.Abs(_direction.Y))
            {
                if (_direction.X < 0f)
                {
                    _spriteSheet.Play("walk_left");
                }
                else
                {
                    _spriteSheet.Play("walk_right");
                }
            }
            else
            {
                if (_direction.Y < 0f)
                {
                    _spriteSheet.Play("walk_up");
                }
                else
                {
                    _spriteSheet.Play("walk_down");

                }
            }
        }
    }
}
