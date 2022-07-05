using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public sealed class Animator : Component
    {
        public class Animation
        {
            public Animation(string name, Texture2D texture, Rectangle[] frames, float secondsPerFrame, bool loop) 
            {
                Name = name;
                Texture = texture;
                Frames = frames;
                SecPerFrame = secondsPerFrame;
                Loop = loop;
            }

            public Animation() 
            {
            }

            public string Name { get; set; }
            public Texture2D Texture { get; set; }
            public Rectangle[] Frames { get; set; }
            public float SecPerFrame { get; set; }
            public bool Loop { get; set; }
        }

        private readonly Animation[] _animations;

        private Sprite _sprite;
        private float _lastFrameTime;
        private float _timer;

        public Animator(Animation[] animations)
        {
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
        }

        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
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

                CurrentAnimation = _animations.SingleOrDefault(a => a.Name == name);
                CurrentFrame = 0;
                IsFinished = false;
            }
        }

        public override void OnTick(GameTime gameTime)
        {
            if (!IsFinished)
            {
                _timer += gameTime.GetDeltaTime();

                if (_timer - _lastFrameTime > CurrentAnimation.SecPerFrame)
                {
                    if (CurrentAnimation.Loop)
                    {
                        CurrentFrame = (CurrentFrame + 1) % CurrentAnimation.Frames.Length;
                    }
                    else if (++CurrentFrame >= CurrentAnimation.Frames.Length)
                    {
                        CurrentFrame--;
                        IsFinished = true;
                    }

                    _lastFrameTime = _timer;

                    _sprite.Texture = CurrentAnimation.Texture;
                    _sprite.SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
                }
            }
        }
    }
}
