using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// A state that loads a level.
    /// </summary>
    public class LevelLoadState : GameState
    {
        public LevelLoadState(App application, Level level) : base(application)
        {
            Level = level;
        }

        /// <summary>
        /// Gets the level that is being loaded
        /// </summary>
        public Level Level 
        {
            get;
        }

        public override void Start()
        {
            Level.Start();
        }

        public override void Update(GameTime gameTime)
        {
            if (Level.Status == Level.LoadStatus.Loaded && CanStartNextLevel()) 
            {
                Application.Stack.Pop(this);
                Application.Stack.Push(Level);
            }
        }

        /// <summary>
        /// Returns true if the next level can be start.
        /// </summary>
        /// <remarks>This returns true by default, but can be used to delay the level loading.</remarks>
        /// <returns></returns>
        protected virtual bool CanStartNextLevel() 
        {
            return true;
        }
    }
}
