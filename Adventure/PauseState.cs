using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adventure
{
    internal class PauseState : GameState
    {
        public PauseState(App application) : base(application)
        {
        }

        public override bool ShouldUpdateBelow
        {
            // Don't update the states below. This is what pauses the game.
            get => false;
        }

        public override bool ShouldDrawBelow
        {
            // Draw the states below
            get => true;
        }

        private Rectangle _buttonRectangle;
        private Color _buttonColor;
        private bool _buttonHot;
        private bool _buttonActive;

        public override void Start()
        {
            _buttonRectangle = new Rectangle(100, 150, 75, 20);
            _buttonColor = Color.Blue;
        }

        public override void Update(GameTime gameTime)
        {
            var mousePosition = Mouse.GetState().Position;
            var screenPosition = Application.Screen.ToVirtualScreen(mousePosition.ToVector2());

            _buttonColor = Color.LightBlue;
            _buttonHot = false;

            if (_buttonRectangle.Contains(screenPosition)) 
            {
                _buttonHot = true;
            }

            if (_buttonHot) 
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    _buttonActive = true;
                }

                if (_buttonActive && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    Application.Stack.PopAll();
                    Application.Stack.Push(new MainMenuState(Application));
                }
            }

            if (!_buttonHot) 
            {
                _buttonActive = false;
            }

            if (_buttonHot) 
            {
                _buttonColor = Color.Blue;
            }

            if (_buttonActive)
            {
                _buttonColor = Color.DarkBlue;
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.BeginDraw();
            renderer.DrawRectangle(0, 0, Application.ResolutionWidth, Application.ResolutionHeight, new Color(Color.Black, 0.4f));
            renderer.DrawString(SharedContent.Font, "Paused", 100f, 100f, Color.White);
            renderer.DrawRectangle(_buttonRectangle, _buttonColor);
            renderer.DrawString(SharedContent.Font, "Main menu", _buttonRectangle.X, _buttonRectangle.Y, Color.Black);
            renderer.EndDraw();
        }
    }
}
