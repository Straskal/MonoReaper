using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// Singletons exist once per game and receive callbacks similiar to behaviors.
    /// 
    /// They're good for services that the game may require. i.e. inventory system, layout management, etc...
    /// </summary>
    public abstract class Singleton
    {
        public Singleton(MainGame game) 
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public MainGame Game { get; }

        /// <summary>
        /// Called once per frame, before behaviors.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Tick(GameTime gameTime) { }

        public virtual void Draw(LayoutView view) { }
    }
}
