using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Core
{
    public class PlayerBehavior : Behavior
    {
        private AnimationBehavior _animationBehavior;
        private PlatformerBehavior _platformerBehavior;
        private KeyboardState _previousKeyState;

        private Action<float> _currentState;

        public override void OnOwnerCreated()
        {
            _animationBehavior = Owner.GetBehavior<AnimationBehavior>();
            _platformerBehavior = Owner.GetBehavior<PlatformerBehavior>();

            GoToIdle();
        }

        public override void Tick(GameTime gameTime)
        {
            _currentState.Invoke((float)gameTime.ElapsedGameTime.TotalSeconds);

            _previousKeyState = Keyboard.GetState();
        }

        private void GoToIdle() 
        {
            _animationBehavior.Play("Idle");
            _currentState = Idle;
        }

        private void Idle(float elapesedTime) 
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D))
            {
                GoToMove();
            }
            else if (_platformerBehavior.IsOnGround() && keyboardState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
            {
                GoToJump();
            }
            else if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
        }

        private void GoToMove()
        {
            _animationBehavior.Play("Run");
            _currentState = Move;
        }

        private void Move(float elapesedTime)
        {
            float movement = 0;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
                Owner.IsMirrored = false;
            }
            if (_platformerBehavior.IsOnGround() && keyboardState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
            {
                GoToJump();
            }

            _platformerBehavior.Move(movement);

            if (movement == 0f) 
            {
                GoToIdle();
            }
        }

        private void GoToJump()
        {
            _platformerBehavior.Jump();
            _animationBehavior.Play("Idle");
            _currentState = Jump;
        }

        private void Jump(float elapesedTime)
        {
            if (_platformerBehavior.IsFalling()) 
            {
                GoToFall();
                return;
            }

            float movement = 0;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
                Owner.IsMirrored = false;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _platformerBehavior.Jump();
            }

            _platformerBehavior.Move(movement);
        }

        private void GoToFall()
        {
            _animationBehavior.Play("Jump");
            _currentState = Fall;
        }

        private void Fall(float elapesedTime)
        {
            if (_platformerBehavior.IsOnGround()) 
            {
                if (_platformerBehavior.IsMoving()) 
                {
                    GoToMove();
                }
                else 
                {
                    GoToIdle();
                }
            }

            float movement = 0;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -1;
                Owner.IsMirrored = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += 1;
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);
        }
    }
}
