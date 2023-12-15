using System;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
    /// <summary>
    /// A game screen represents the current screens that the player can see.
    /// </summary>
    /// <remarks>
    /// Game screens are placed onto a stack. An example of usage is game -> pause screen -> settings screen -> audio settings screen
    /// Screens placed on top get to decide if the screens below can update and or draw.
    /// The root screen placed into the stack can never be popped.
    /// </remarks>
    public abstract class Screen
    {
        public Screen(App application) 
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

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
        }

        public ScreenStack Screens 
        {
            get => Application.Screens;
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
