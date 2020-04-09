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
        public Layout Layout => Owner.Layout;
        public MainGame Game => Layout.Game;

        /// <summary>
        /// First method called on a behavior. This is a good place for initialization.
        /// </summary>
        /// <param name="contentManager"></param>
        public virtual void Load(ContentManager contentManager) { }

        /// <summary>
        /// Called once all other behaviors on the owner have been loaded.
        /// </summary>
        public virtual void OnOwnerCreated() { }

        /// <summary>
        /// Called when the layout has started. At this point, you can safely access other world objects in the layout.
        /// </summary>
        public virtual void OnLayoutStarted() { }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Tick(GameTime gameTime) { }

        /// <summary>
        /// Called once per frame after all world objects have been ticked. This is a good spot for camera movement.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void PostTick(GameTime gameTime) { }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        /// <param name="view"></param>
        public virtual void Draw(LayoutView view) { }

        /// <summary>
        /// Called only in debug mode.
        /// </summary>
        /// <param name="view"></param>
        public virtual void DebugDraw(LayoutView view) { }

        /// <summary>
        /// Called when the owner has been destroyed.
        /// </summary>
        public virtual void OnOwnerDestroyed() { }
    }
}
