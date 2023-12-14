using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Actions;

namespace Adventure
{
    /// <summary>
    /// The root state acts as the entry point to the application and overall game controller.
    /// The root state cannot be popped from the stack.
    /// </summary>
    internal class RootState : GameState
    {
        private PressedAction _toggleFullscreenAction;
        private PressedAction _quitAction;
        private PressedAction _pauseAction;
        private PressedAction _resetAction;

        // Cache pause state.
        private PauseState _pauseState;

        public RootState(App application) : base(application)
        {
        }

        public override void Start()
        {
            Application.Window.Title = "Adventure Game 2000";
            Application.Window.AllowUserResizing = true;

            _toggleFullscreenAction = Input.NewPressedAction(Keys.F);
            _quitAction = Input.NewPressedAction(Keys.Escape);
            _pauseAction = Input.NewPressedAction(Keys.P);
            _resetAction = Input.NewPressedAction(Keys.Space);
            _pauseState = new PauseState(Application);

            LoadlevelWithoutTransition();
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
                    LoadlevelWithTransition();
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

        private void LoadlevelWithoutTransition()
        {
            // If a level is pushed directly to the stack without a transition, it will just wait to update and draw until it's finished loading.
            Stack.Push(LevelLoader.LoadLevel(Application, "Levels/world/level_0"));
        }

        private void LoadlevelWithTransition()
        {
            // If a level is loaded with a transition, the transition gets to decice when the level is pushed to the stack.
            Stack.Push(new LevelTransitionState(Application, LevelLoader.LoadLevel(Application, "Levels/world/level_0")));
        }
    }
}
