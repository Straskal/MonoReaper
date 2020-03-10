using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace ItsGood.Builtins
{

    public class AnimationBehavior : Behavior<AnimationBehavior.Params>
    {
        public class Params
        {
            public Animation[] Animations { get; set; }
        }

        public struct AnimationFrame
        {
            public Rectangle Source;
        }

        public class Animation
        {
            public string Name { get; set; }
            public string ImageFilePath { get; set; }
            public Texture2D Image { get; set; }
            public AnimationFrame[] Frames { get; set; }
            public float SecPerFrame { get; set; }
            public bool Loop { get; set; }
        }

        private Animation[] _animations;
        private float _lastFrameTime;
        private float _time;

        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        public bool IsFinished { get; private set; }

        public override void Initialize()
        {
            _animations = State.Animations;

            var content = Owner.Layout.Game.Content;

            foreach (var animation in _animations) 
            {
                animation.Image = content.Load<Texture2D>(animation.ImageFilePath);
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
            Owner.Image = CurrentAnimation.Image;
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
                        IsFinished = true;
                        return;
                    }
                }

                _lastFrameTime = _time;
            }

            Owner.Source = CurrentAnimation.Frames[CurrentFrame].Source;
        }
    }
}
