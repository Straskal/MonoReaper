using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public sealed class Animator : Component
    {
        public class Animation
        {
            public string Name { get; set; }
            public string ImageFilePath { get; set; }
            public Texture2D Image { get; set; }
            public Rectangle[] Frames { get; set; }
            public float SecPerFrame { get; set; }
            public bool Loop { get; set; }
            public Point? Origin { get; set; }
        }

        private readonly Animation[] animations;

        private Sprite sprite;
        private float lastFrameTime;
        private float timer;

        public Animator(Animation[] animations)
        {
            this.animations = animations ?? throw new ArgumentNullException(nameof(animations));
        }

        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        public bool IsFinished { get; private set; }

        public override void OnLoad(ContentManager content)
        {
            if (animations.Length > 0)
            {
                foreach (var animation in animations)
                {
                    animation.Image = content.Load<Texture2D>(animation.ImageFilePath);
                }

                Play(animations[0].Name);
            }
        }

        public override void OnSpawn()
        {
            sprite = Entity.GetComponent<Sprite>();
        }

        public void Play(string name)
        {
            if (name != CurrentAnimation?.Name || IsFinished)
            {
                var animation = animations.SingleOrDefault(a => a.Name == name);

                if (animation == null)
                {
                    throw new ArgumentException($"Animation {name} does not exist.");
                }

                CurrentAnimation = animations.SingleOrDefault(a => a.Name == name);
                CurrentFrame = 0;
                IsFinished = false;
            }
        }

        public override void OnTick(GameTime gameTime)
        {
            if (!IsFinished)
            {
                timer += gameTime.GetDeltaTime();

                if (timer - lastFrameTime > CurrentAnimation.SecPerFrame)
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

                    lastFrameTime = timer;

                    sprite.Texture = CurrentAnimation.Image;
                    sprite.SourceRectangle = CurrentAnimation.Frames[CurrentFrame];
                }
            }
        }
    }
}
