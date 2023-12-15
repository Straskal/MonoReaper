using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Graphics
{
    /// <summary>
    /// This class represents a sprite with animations. It handles playing the current animation and rendering animation frame.
    /// </summary>
    public sealed class AnimatedSprite : Sprite
    {
        private readonly Animation[] _animations;

        private float _timer;

        public AnimatedSprite(Texture2D texture, IEnumerable<Animation> animations) : base(texture)
        {
            if (animations == null) 
            {
                throw new ArgumentNullException(nameof(animations));
            }

            if (!animations.Any())
            {
                throw new ArgumentException($"{nameof(animations)} must have at least one animation");
            }            

            _animations = animations.ToArray();
        }

        /// <summary>
        /// Gets or sets the animation speed. Speed is measured in seconds.
        /// </summary>
        /// <remarks>
        /// Defaults to 4 frames per second
        /// </remarks>
        public float Speed 
        { 
            get; 
            set; 
        } = Animation.FpsToSpeed(4);

        /// <summary>
        /// Gets the current animation
        /// </summary>
        public Animation CurrentAnimation 
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current frame number
        /// </summary>
        public int CurrentFrame 
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if the current animation has finished
        /// </summary>
        public bool IsFinished 
        { 
            get; 
            private set; 
        }

        public override void OnSpawn()
        {
            Play(_animations[0].Name);
        }

        public void Play(string animationName)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(animationName, CurrentAnimation?.Name) || IsFinished) 
            {
                foreach (var animation in _animations) 
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
            if (animationNameHashCode != CurrentAnimation?.NameHashCode || IsFinished)
            {
                foreach (var animation in _animations)
                {
                    if (CurrentAnimation?.NameHashCode == animationNameHashCode)
                    {
                        ResetAnimation(animation);
                        return;
                    }
                }

                throw new ArgumentException($"Animation {animationNameHashCode} does not exist.");
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (IsFinished) 
            {
                return;
            }

            if ((_timer += gameTime.GetDeltaTime()) <= Speed) 
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
                    IsFinished = true;
                }
            }
            else 
            {
                CurrentFrame++;
            }

            SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
            _timer = 0;
        }

        private void ResetAnimation(Animation animation)
        {
            CurrentAnimation = animation;
            CurrentFrame = 0;
            //SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
            IsFinished = false;
        }
    }
}
