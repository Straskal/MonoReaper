using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Graphics;

namespace Engine
{
    /// <summary>
    /// This is the base class for components.
    /// </summary>
    /// <remarks>
    /// Components hold game logic or rendering logic.
    /// </remarks>
    public abstract class Component
    {
        /// <summary>
        /// Gets the entity that the component belongs to.
        /// </summary>
        /// <remarks>
        /// Entity will only be available if the component has been added to an entity.
        /// </remarks>
        public Entity Entity
        {
            get;
            internal set;
        }

        /// <summary>
        /// The level that the component's entity belongs to.
        /// </summary>
        /// <remarks>
        /// Level will only be available if the component has been added to an entity, and that entity has been spawned into a level.
        /// </remarks>
        public Level Level
        {
            get => Entity.Level;
        }

        /// <summary>
        /// Gets or sets this components Z ordering for drawing.
        /// </summary>
        public int ZOrder
        {
            get;
            set;
        }

        /// <summary>
        /// A callback to handle loading content required for the component.
        /// </summary>
        /// <param name="content"></param>
        public virtual void OnLoad(ContentManager content) { }

        /// <summary>
        /// This method is called on all component's once their entity is spawned into the level.
        /// </summary>
        /// <remarks>
        /// This is a good point to run component initialization logic or query for other components on the component's entity.
        /// </remarks>
        public virtual void OnSpawn() { }

        /// <summary>
        /// This method is called when the component's entity has been destroyed, or the component was removed from the entity.
        /// </summary>
        /// <remarks>
        /// This is a good place for any logic that needs to run when it's entity is destroyed.
        /// </remarks>
        public virtual void OnDestroy() { }

        /// <summary>
        /// This method is called once all entities once the level starts.
        /// </summary>
        /// <remarks>
        /// This is a good place for any logic that needs to query for other entities in the level.
        /// </remarks>
        public virtual void OnStart() { }

        /// <summary>
        /// This method is called when the level ends.
        /// </summary>
        /// <remarks>
        /// This is a good place for logic that needs to run at the end of a level.
        /// </remarks>
        public virtual void OnEnd() { }

        /// <summary>
        /// This methods is called once per frame.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <remarks>
        /// This is a good place for real time game logic.
        /// </remarks>
        public virtual void OnUpdate(GameTime gameTime) { }

        /// <summary>
        /// This method is called once at the end of each frame.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <remarks>
        /// This is a good place for any logic that needs to run after entities have finished processing for the current frame.
        /// </remarks>
        public virtual void OnPostUpdate(GameTime gameTime) { }

        /// <summary>
        /// This method is called once at the end of each frame.
        /// </summary>
        public virtual void OnDraw(Renderer renderer, GameTime gameTime) { }

        /// <summary>
        /// This method is called once at the end of each frame.
        /// </summary>
        public virtual void OnDebugDraw(Renderer renderer, GameTime gameTime) { }
    }
}
