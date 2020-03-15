using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace ItsGood.Builtins
{

    public class SpriteSheetBehavior : Behavior
    {
        public class Params
        {
            public Animation[] Animations { get; set; }
        }

        public struct Frame
        {
            public Frame(int x, int y, int width, int height)
            {
                Source = new Rectangle(x, y, width, height);
            }

            public Rectangle Source;
        }

        public class Animation
        {
            public string Name { get; set; }
            public string ImageFilePath { get; set; }
            public Texture2D Image { get; set; }
            public Frame[] Frames { get; set; }
            public float SecPerFrame { get; set; }
            public bool Loop { get; set; }
        }

        private readonly Animation[] _animations;

        private float _lastFrameTime;
        private float _time;

        public SpriteSheetBehavior(WorldObject owner, Animation[] animations) : base(owner)
        {
            _animations = animations ?? throw new ArgumentNullException(nameof(animations));
        }

        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        public bool IsFinished { get; private set; }

        public override void Load(ContentManager contentManager)
        {
            foreach (var animation in _animations) 
            {
                animation.Image = contentManager.Load<Texture2D>(animation.ImageFilePath);
            }

            Play(_animations[0].Name);
        }

        public void Play(string name)
        {
            if (CurrentAnimation?.Name == name)
                return;

            CurrentAnimation = _animations.Single(anim => anim.Name == name);
            CurrentFrame = 0;
            IsFinished = false;
        }

        public override void Tick(GameTime gameTime)
        {
            if (IsFinished)
                return;

            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _time += elapsedTime;

            if (_time - _lastFrameTime > CurrentAnimation.SecPerFrame)
            {
                CurrentFrame++;

                if (CurrentFrame == CurrentAnimation.Frames.Length)
                {
                    if (CurrentAnimation.Loop) 
                    {
                        CurrentFrame = 0;
                    }
                    else 
                    {
                        CurrentFrame--;
                        IsFinished = true;
                        return;
                    }
                }

                _lastFrameTime = _time;
            }
        }

        public override void Draw(LayoutView view)
        {
            Frame frame = CurrentAnimation.Frames[CurrentFrame];

            int xPosition = Owner.IsMirrored
                ? (int)Owner.Position.X - (frame.Source.Width - Owner.Origin.X)
                : (int)Owner.Position.X - Owner.Origin.X;

            Rectangle dest = new Rectangle(xPosition, (int)Owner.Position.Y - Owner.Origin.Y, frame.Source.Width, frame.Source.Height);

            view.Draw(CurrentAnimation.Image, frame.Source, dest, Color.White, Owner.IsMirrored);
        }
    }
}
