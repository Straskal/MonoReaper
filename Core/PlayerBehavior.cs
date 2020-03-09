using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core
{
    public class PlayerBehavior : Behavior, PlatformerBehavior.IAnimationCallbacks
    {
        private AnimationBehavior _animationBehavior;
        private PlatformerBehavior _platformerBehavior;

        public override void OnOwnerCreated()
        {
            _animationBehavior = Owner.GetBehavior<AnimationBehavior>();
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

            _platformerBehavior.Move(movement);
        }

        public void OnMoved()
        {
            _animationBehavior.Play("Run");
        }

        public void OnStopped()
        {
            _animationBehavior.Play("Idle");
        }

        public void OnJumped()
        {
            _animationBehavior.Play("Jump");
        }

        public void OnFall()
        {
            _animationBehavior.Play("Jump");
        }

        public void OnLanded()
        {
            _animationBehavior.Play("Idle");
        }
    }
}
