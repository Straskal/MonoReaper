using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
    public abstract class GameState
    {
        /// <summary>
        /// Gets whether or not the state below should update
        /// </summary>
        public virtual bool ShouldUpdateBelow
        {
            get;
        } = true;

        /// <summary>
        /// Gets whether or not the state below should draw
        /// </summary>
        public virtual bool ShouldDrawBelow
        {
            get;
        } = true;

        /// <summary>
        /// Gets the application
        /// </summary>
        public App Application 
        {
            get;
            internal set;
        }

        public GameStateStack Stack 
        {
            get => Application.Stack;
        }

        /// <summary>
        /// Called when the state is pushed to the stack
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Called when the state is popped from the stack
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Called at the end of every frame
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
        }
    }
}
