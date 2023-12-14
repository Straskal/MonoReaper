using System.Collections;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// A state that loads a level.
    /// </summary>
    public class LevelLoadState : GameState
    {
        public LevelLoadState(Level level) 
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

        /// <summary>
        /// Returns true if the the level finished loading
        /// </summary>
        public bool IsFinishedLoading 
        {
            get;
            private set;
        }

        public override void Start()
        {
            Application.StartCoroutine(LoadLevel());
        }

        public override void Update(GameTime gameTime)
        {
            if (IsFinishedLoading) 
            {
                Application.Stack.Pop(this);
                Application.Stack.Push(Level);
            }
        }

        private IEnumerator LoadLevel() 
        {
            yield return Level.Load();
            IsFinishedLoading = true;
        }
    }
}
