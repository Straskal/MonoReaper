using Engine.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public sealed class Animator
    {
        private readonly Sprite sprite;
        private readonly Animation[] animations;
        private float timer;

        public Animator(Sprite sprite, IEnumerable<Animation> animations)
        {
            this.sprite = sprite;

            if (animations == null)
            {
                throw new ArgumentNullException(nameof(animations));
            }

            if (!animations.Any())
            {
                throw new ArgumentException($"{nameof(animations)} must have at least one animation");
            }

            this.animations = animations.ToArray();
            ResetAnimation(this.animations[0]);
        }

        public float Speed { get; set; } = Animation.FpsToSpeed(4);
        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        public bool CurrentAnimationFinished { get; private set; }
        public bool IsPaused { get; set; }

        public void Play(string animationName)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(animationName, CurrentAnimation?.Name) || CurrentAnimationFinished)
            {
                foreach (var animation in animations)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(animation.Name, animationName))
                    {
                        ResetAnimation(animation);
                        return;
                    }
                }

                throw new ArgumentException($"Animation {animationName} does not exist.");
            }
        }

        public void Play(int animationNameHashCode)
        {
            if (animationNameHashCode != CurrentAnimation?.HashCode || CurrentAnimationFinished)
            {
                foreach (var animation in animations)
                {
                    if (CurrentAnimation?.HashCode == animationNameHashCode)
                    {
                        ResetAnimation(animation);
                        return;
                    }
                }

                throw new ArgumentException($"Animation {animationNameHashCode} does not exist.");
            }
        }

        public void RunFrame(GameTime gameTime)
        {
            if (IsPaused || CurrentAnimationFinished || (timer += gameTime.GetDeltaTime()) <= Speed)
            {
                return;
            }

            if (CurrentFrame + 1 >= CurrentAnimation.Frames.Length)
            {
                if (CurrentAnimation.Loop)
                {
                    CurrentFrame = 0;
                }
                else
                {
                    CurrentAnimationFinished = true;
                }
            }
            else
            {
                CurrentFrame++;
            }

            sprite.SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
            timer = 0;
        }

        private void ResetAnimation(Animation animation)
        {
            CurrentAnimation = animation;
            CurrentFrame = 0;
            sprite.SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
            CurrentAnimationFinished = false;
        }
    }
}
