using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// Behaviors define the actions of a world object.
    /// </summary>
    public abstract class Behavior
    {
        public Behavior(WorldObject owner) 
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public WorldObject Owner { get; }

        public virtual void Load(ContentManager contentManager) { }
        public virtual void OnOwnerCreated() { }
        public virtual void OnLayoutStarted() { }
        public virtual void Tick(GameTime gameTime) { }
        public virtual void PostTick(GameTime gameTime) { }
        public virtual void Draw(LayoutView view) { }
        public virtual void OnOwnerDestroyed() { }
    }
}
