using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Actions;

namespace Adventure
{
    internal class RootState : GameState
    {
        private PressedAction _toggleFullscreenAction;
        private PressedAction _quitAction;
        private PressedAction _pauseAction;
        private PressedAction _resetAction;

        // Cache pause state.
        private PauseState _pauseState;

        public override void Start()
        {
            Application.Window.Title = "Adventure Game 2000";
            Application.Window.AllowUserResizing = true;

            _toggleFullscreenAction = Input.NewPressedAction(Keys.F);
            _quitAction = Input.NewPressedAction(Keys.Escape);
            _pauseAction = Input.NewPressedAction(Keys.P);
            _resetAction = Input.NewPressedAction(Keys.Space);
            _pauseState = new PauseState();

            // Push the first level onto the stack.
            Stack.Push(new LevelLoadState(LevelLoader.LoadLevel(Application, "Levels/world/level_0")));
            //Stack.Push(LevelLoader.LoadLevel(Application, "Levels/world/level_0"));
        }

        public override void Update(GameTime gameTime)
        {
            if (_toggleFullscreenAction.WasPressed())
            {
                Application.GraphicsDeviceManager.ToggleFullScreen();
                Application.GraphicsDeviceManager.ApplyChanges();
            }

            if (_quitAction.WasPressed())
            {
                Application.Exit();
            }

            if (_resetAction.WasPressed())
            {
                if (Stack.Top is Level level) 
                {
                    Stack.Pop(level);
                    Stack.Push(new LevelLoadState(LevelLoader.LoadLevel(Application, "Levels/world/level_0")));
                }
            }

            if (_pauseAction.WasPressed())
            {
                if (Stack.Top == _pauseState)
                {
                    Stack.Pop(_pauseState);
                }
                else 
                {
                    Stack.Push(_pauseState);
                }              
            }
        }
    }
}
