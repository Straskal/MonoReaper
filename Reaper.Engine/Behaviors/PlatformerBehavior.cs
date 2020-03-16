using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine.Behaviors
{
    public class PlatformerBehavior : Behavior
    {
        private float _jumpTime;
        private float _movement;
        private bool _jumpRequested;
        private bool _wasJumping;
        private bool _isOnGround;
        private Vector2 _velocity;

        public PlatformerBehavior(WorldObject owner) : base(owner)
        {
        }

        public float Acceleration { get; set; } = 1500f;
        public float MaxSpeed { get; set; } = 115f;
        public float Drag { get; set; } = 0.8f;
        public float MaxJumpTime { get; set; } = 0.5f;
        public float JumpVelocity { get; set; } = -1200;
        public float JumpControl { get; set; } = 0.14f;
        public float GravityAcceleration { get; set; } = 1350f;
        public float MaxFallSpeed { get; set; } = 400f;
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
            return _isOnGround;
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
            _velocity.X += Acceleration * _movement * elapsedTime;
            _velocity.X *= Drag;
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);
            _isOnGround = Owner.Layout.TestOverlapOffset(Owner, 0, GroundBufferInPixels);
        }

        private void SimilateJump(float elapsedTime)
        {
            if (_jumpRequested)
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

            if (Owner.MoveYAndCollide(_velocity.Y * elapsedTime, out var worldObject) && worldObject.Bounds.Bottom < Owner.Bounds.Top) 
            {
                _velocity.Y = 0f;
                _jumpTime = 0f;
            }

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
