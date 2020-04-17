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

        /// <summary>
        /// The world object that this behavior belongs to.
        /// </summary>
        public WorldObject Owner { get; }

        /// <summary>
        /// The layout that the behavior is active in.
        /// </summary>
        public Layout Layout => Owner.Layout;

        /// <summary>
        /// The game...
        /// </summary>
        public MainGame Game => Layout.Game;

        /// <summary>
        /// First method called on a behavior. This is a good place for initialization and loading content.
        /// </summary>
        public virtual void Load() { }

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
        /// <param name="renderer"></param>
        public virtual void Draw(Renderer renderer) { }

        /// <summary>
        /// Called only in debug mode.
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void DebugDraw(Renderer renderer) { }

        /// <summary>
        /// Called when the owner has been destroyed.
        /// </summary>
        public virtual void OnOwnerDestroyed() { }

        /// <summary>
        /// Called when the layout has ended.
        /// </summary>
        public virtual void OnLayoutEnded() { }
    }
}
