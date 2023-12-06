using System;
using System.Linq;
using Engine.Extensions;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public sealed class SpriteSheet : Component
    {
        public class Animation
        {
            public string Name { get; set; }
            public Rectangle[] Frames { get; set; }
            public bool Loop { get; set; }
        }

        private readonly Animation[] _animations;

        private Sprite _sprite;
        private Animation _currentAnimation;
        private int _currentFrame;
        private float _timer;

        public SpriteSheet(Animation[] animations)
        {
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));

            IsUpdateEnabled = true;
        }

        public float Speed { get; set; } = 0.25f; // Default to 4 frames per second
        public Animation CurrentAnimation => _currentAnimation;
        public int CurrentFrame => _currentFrame;
        public bool IsFinished { get; private set; }

        public override void OnSpawn()
        {
            _sprite = Entity.RequireComponent<Sprite>();

            if (_animations.Length > 0)
            {
                Play(_animations[0].Name);
            }
        }

        public void Play(string name)
        {
            if (name != CurrentAnimation?.Name || IsFinished)
            {
                var animation = _animations.SingleOrDefault(a => a.Name == name);

                if (animation == null)
                {
                    throw new ArgumentException($"Animation {name} does not exist.");
                }

                _currentAnimation = _animations.SingleOrDefault(a => a.Name == name);
                _currentFrame = 0;

                IsFinished = false;
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (!IsFinished)
            {
                _timer += gameTime.GetDeltaTime();

                if (_timer > Speed)
                {
                    if (_currentAnimation.Loop)
                    {
                        _currentFrame = (_currentFrame + 1) % _currentAnimation.Frames.Length;
                    }
                    else if (++_currentFrame >= _currentAnimation.Frames.Length)
                    {
                        _currentFrame--;

                        IsFinished = true;
                    }

                    _timer = 0;
                    _sprite.SourceRectangle = _currentAnimation.Frames[_currentFrame];
                }
            }
        }
    }
}
