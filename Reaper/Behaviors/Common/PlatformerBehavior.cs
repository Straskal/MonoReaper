﻿using Microsoft.Xna.Framework;
using Reaper.Engine;
using System;

namespace Reaper.Behaviors.Common
{
    /// <summary>
    /// Generic platforming behavior.
    /// </summary>
    public class PlatformerBehavior : Behavior
    {
        private float _jumpTime;
        private float _movement;
        private bool _jumpRequested;
        private bool _wasJumping;
        private bool _isOnGround;
        private bool _canJump;
        private Vector2 _velocity;

        public PlatformerBehavior(WorldObject owner) : base(owner) { }

        public float Acceleration { get; set; } = 1700f;
        public float MaxSpeed { get; set; } = 300f;
        public float Drag { get; set; } = 0.8f;
        public float MaxJumpTime { get; set; } = 0.55f;
        public float JumpVelocity { get; set; } = -1500;
        public float JumpControl { get; set; } = 0.09f;
        public float GravityAcceleration { get; set; } = 2100f;
        public float MaxFallSpeed { get; set; } = 275f;
        public int GroundBufferInPixels { get; set; } = 1;

        public Vector2 Velocity 
        {
            get => _velocity;
            set => _velocity = value;
        }

        public void Move(float movement)
        {
            _movement = movement;
        }

        public void Jump()
        {
            _jumpRequested = true;
        }

        public bool IsMoving() 
        {
            return Math.Abs(_velocity.X) > 0.1f;
        }

        public bool IsJumping() 
        {
            return _velocity.Y < 0f;
        }

        public bool IsFalling()
        {
            return _velocity.Y > 0f;
        }

        public bool IsOnGround()
        {
            return _isOnGround;
        }

        public bool CanJump()
        {
            return _canJump;
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SimulateMovement(elapsedTime);
            SimulateJump(elapsedTime);
            SimulateGravity(elapsedTime);
            ApplyVelocity(elapsedTime);

            ResetInputValues();
        }

        private void SimulateMovement(float elapsedTime) 
        {
            _velocity.X += Acceleration * _movement * elapsedTime;
            _velocity.X *= Drag;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);
            _isOnGround = Layout.Grid.IsCollidingAtOffset(Owner, 0f, GroundBufferInPixels);
            _canJump = !Layout.Grid.IsCollidingAtOffset(Owner, 0f, -1f);
        }

        private void SimulateJump(float elapsedTime)
        {
            if (_jumpRequested && _canJump)
            {
                if (!_wasJumping && _isOnGround) 
                {
                    _jumpTime += elapsedTime;
                }
                else if (_jumpTime > 0f) 
                {
                    _jumpTime += elapsedTime;
                }

                if (_jumpTime > 0f && _jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    _velocity.Y = JumpVelocity * (1f - (float)Math.Pow(_jumpTime / MaxJumpTime, JumpControl));
                }
                else
                {
                    _jumpTime = 0f;
                }
            }
            else
            {
                _jumpTime = 0f;
            }
        }

        private void SimulateGravity(float elapsedTime)
        {
            _velocity.Y = MathHelper.Clamp(_velocity.Y + GravityAcceleration * elapsedTime, JumpVelocity, MaxFallSpeed);
        }

        private void ApplyVelocity(float elapsedTime)
        {
            if (Owner.MoveXAndCollide(_velocity.X * elapsedTime, out _)) 
            {
                _velocity.X = 0f;
            }

            if (Owner.MoveYAndCollide(_velocity.Y * elapsedTime, out var worldObject) && worldObject.Bounds.Bottom <= Owner.Bounds.Top) 
            {
                _velocity.Y = 0f;
                _jumpTime = 0f;
            }
        }

        private void ResetInputValues()
        {
            // If the position remains the same, then we're still on a ledge.
            // Let's set our velocity to zero so that when we walk off of a ledge, we don't shoot down to the ground.
            if (Owner.Position.Y == Owner.PreviousPosition.Y)
            {
                _velocity.Y = 0f;
            }

            _wasJumping = _jumpRequested;

            _movement = 0;
            _jumpRequested = false;
        }
    }
}