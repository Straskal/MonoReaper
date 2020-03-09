﻿using Microsoft.Xna.Framework;
using System;

namespace ItsGood.Builtins
{
    public class PlatformerBehavior : Behavior
    {
        private const float MOVE_ACCELERATION = 2000f;
        private const float MAX_MOVE_SPEED = 100f;
        private const float DRAG = 0.8f;
        private const float MAX_JUMP_TIME = 0.35f;
        private const float JUMP_VELOCITY = -1500;
        private const float JUMP_CONTROL = 0.14f;
        private const float GRAVITY_ACCELERATION = 1400f;
        private const float MAX_FALL_SPEED = 400f;
        private const int GROUNDED_BUFFER_IN_PX = 3;

        private float _jumpTime;
        private float _movement;
        private bool _jumpRequested;
        private bool _wasJumping;

        private Vector2 _velocity;

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
            return _velocity.X != 0f;
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
            return Owner.Layout.Grid.TestOverlap(Owner, new Vector2(0, GROUNDED_BUFFER_IN_PX)) != null;
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SimulateMovement(elapsedTime);
            SimilateJump(elapsedTime);
            SimulateGravity(elapsedTime);
            ApplyVelocity(elapsedTime);

            ResetInputValues();
        }

        private void SimulateMovement(float elapsedTime) 
        {
            _velocity.X += MOVE_ACCELERATION * _movement * elapsedTime;
            _velocity.X *= DRAG;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
        }

        private void SimilateJump(float elapsedTime)
        {
            if (_jumpRequested)
            {
                if (!_wasJumping && IsOnGround()) 
                {
                    _jumpTime += elapsedTime;
                }
                else if (_jumpTime > 0f) 
                {
                    _jumpTime += elapsedTime;
                }

                if (_jumpTime > 0f && _jumpTime <= MAX_JUMP_TIME)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    _velocity.Y = JUMP_VELOCITY * (1.0f - (float)Math.Pow(_jumpTime / MAX_JUMP_TIME, JUMP_CONTROL));
                }
                else
                {
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                _jumpTime = 0.0f;
            }
        }

        private void SimulateGravity(float elapsedTime)
        {
            _velocity.Y = MathHelper.Clamp(_velocity.Y + GRAVITY_ACCELERATION * elapsedTime, -MAX_FALL_SPEED, MAX_FALL_SPEED);
        }

        private void ApplyVelocity(float elapsedTime)
        {
            Owner.MoveX(_velocity.X * elapsedTime);
            Owner.MoveY(_velocity.Y * elapsedTime);
            Owner.UpdateBBox();
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
