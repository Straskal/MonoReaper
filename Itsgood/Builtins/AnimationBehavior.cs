using Microsoft.Xna.Framework;
using System.Linq;

namespace ItsGood.Builtins
{

    public class AnimationBehavior : Behavior<AnimationBehavior.Params>
    {
        public class Params
        {
            public Animation[] Animations { get; set; }
        }

        public struct Animation
        {
            public string Name;
            public float SecPerFrame;
            public AnimationFrame[] Frames;
            public bool Loops;
        }

        public struct AnimationFrame
        {
            public Rectangle Source;
        }

        private Animation[] _animations;
        private float _lastFrameTime;
        private float _time;
        private bool _isFinished;

        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }

        public override void Initialize()
        {
            _animations = State.Animations;
            CurrentAnimation = _animations[0];
        }

        public void Play(string name)
        {
            if (CurrentAnimation.Name == name)
                return;

            CurrentAnimation = _animations.Single(anim => anim.Name == name);
            CurrentFrame = 0;
            _isFinished = false;
        }

        public override void Tick(GameTime gameTime)
        {
            if (_isFinished)
                return;

            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _time += elapsedTime;

            if (_time - _lastFrameTime > CurrentAnimation.SecPerFrame)
            {
                CurrentFrame++;

                if (CurrentFrame == CurrentAnimation.Frames.Length)
                {
                    if (CurrentAnimation.Loops) 
                    {
                        CurrentFrame = 0;
                    }
                    else 
                    {
                        _isFinished = true;
                        return;
                    }
                }

                _lastFrameTime = _time;
            }

            Owner.Source = CurrentAnimation.Frames[CurrentFrame].Source;
        }
    }
}
