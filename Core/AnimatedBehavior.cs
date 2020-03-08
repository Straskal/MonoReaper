using ItsGood;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class AnimationParams
    {
        public AnimatedBehavior.AnimationInfo[] Animations { get; set; }
    }

    public class AnimatedBehavior : Behavior<AnimationParams>
    {
        public struct AnimationInfo
        {
            public string Name;
            public float SecPerFrame;
            public AnimationFrameInfo[] Frames;
        }

        public struct AnimationFrameInfo
        {
            public Rectangle Source;
        }

        private AnimationInfo[] _animations;
        private AnimationInfo _currentAnimation;
        private int _currentFrame;
        private float _lastFrameTime;
        private float _time;

        public override void Initialize()
        {
            _animations = State.Animations;
            _currentAnimation = _animations[0];
        }

        public void Play(string name)
        {
            if (_currentAnimation.Name == name)
                return;

            _currentAnimation = _animations.Single(anim => anim.Name == name);
            _currentFrame = 0;
        }

        public override void Tick(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _time += dt;

            if (_time - _lastFrameTime > _currentAnimation.SecPerFrame)
            {
                _currentFrame++;
                if (_currentFrame == _currentAnimation.Frames.Length)
                    _currentFrame = 0;

                _lastFrameTime = _time;
            }

            Owner.Source = _currentAnimation.Frames[_currentFrame].Source;
        }
    }
}
