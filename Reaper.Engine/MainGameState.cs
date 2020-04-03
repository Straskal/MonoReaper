using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    public class MainGameState
    {
        public MainGame Game { get; internal set; }

        public virtual void Start() { }
        public virtual void End() { }

        public virtual void Tick(GameTime gameTime) 
        {
            Game.Singletons.Tick(gameTime);
            Game.CurrentLayout.Tick(gameTime);
            Game.CurrentLayout.PostTick(gameTime);
        }

        public virtual void Draw(GameTime gameTime) 
        {
            Game.CurrentLayout.Draw();
        }
    }
}
