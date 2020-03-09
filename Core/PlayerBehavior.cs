using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core
{
    public class PlayerBehavior : Behavior, PlatformerBehavior.IAnimationCallbacks
    {
        private AnimatedBehavior _animatedBehavior;
        private PlatformerBehavior _platformerBehavior;

        public override void OnOwnerCreated()
        {
            _animatedBehavior = Owner.GetBehavior<AnimatedBehavior>();
            _platformerBehavior = Owner.GetBehavior<PlatformerBehavior>();
            _platformerBehavior.SetAnimationCallbacks(this);
        }

        public override void Tick(GameTime gameTime)
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
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _platformerBehavior.Jump();
            }

            if (movement == 0f)
            {
                _animatedBehavior.Play("Idle");
            }
            else
            {
                _animatedBehavior.Play("Run");
            }

            _platformerBehavior.Move(movement);
        }

        public void OnMoved()
        {
            _animatedBehavior.Play("Run");
        }

        public void OnStopped()
        {
            _animatedBehavior.Play("Idle");
        }

        public void OnJumped()
        {
            _animatedBehavior.Play("Jump");
        }

        public void OnLanded()
        {
            
        }
    }
}
