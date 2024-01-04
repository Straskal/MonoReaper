using Microsoft.Xna.Framework;

namespace Engine
{
    public class LevelLoadingScreen : Screen
    {
        public LevelLoadingScreen(App application, Level level) : base(application)
        {
            Level = level;
        }

        public Level Level 
        {
            get;
        }

        public override void Start()
        {
            // Kick off the level load.
            Level.Start();
        }

        public override void Update(GameTime gameTime)
        {
            if (Level.Status == LevelLoadStatus.Loaded && CanStartNextLevel()) 
            {
                Application.ChangeScreen(Level);
            }
        }

        protected virtual bool CanStartNextLevel() 
        {
            return true;
        }
    }
}
