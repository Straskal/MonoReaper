using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Graphics;

namespace Adventure
{
    internal class MainMenuState : GameState
    {
        private int _hotId = -1;
        private int _activeId = -1;
        private Vector2 _mousePosition;
        private bool _mouseDown;

        public MainMenuState(App application) : base(application)
        {
        }

        public override void Update(GameTime gameTime)
        {
            _mousePosition = Application.Screen.ToVirtualScreen(Mouse.GetState().Position.ToVector2());
            _mouseDown = Mouse.GetState().LeftButton == ButtonState.Pressed;        
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            _hotId = 0;
            renderer.BeginDraw();
            DrawGameTitle(renderer);
            DrawButtons(renderer);
            renderer.EndDraw();
        }

        private void DrawGameTitle(Renderer renderer) 
        {
            renderer.DrawString(SharedContent.Font, "Super great game 2000", new Vector2(50f, 10f), Color.White);
        }

        private void DrawButtons(Renderer renderer) 
        {
            if (Button(1, "Start", new Rectangle(100, 120, 50, 20), Color.Blue, Color.LightBlue, Color.DarkBlue, renderer))
            {
                Stack.SetTop(new LevelTransitionState(Application, LevelLoader.LoadLevel(Application, "Levels/world/level_0")));
            }

            if (Button(2, "Exit", new Rectangle(100, 150, 50, 20), Color.Red, Color.Pink, Color.DarkRed, renderer))
            {
                Application.Exit();
            }
        }

        private bool Button(int id, string text, Rectangle bounds, Color normal, Color hot, Color active, Renderer renderer) 
        {
            bool pressed = false;

            Color color;

            if (bounds.Contains(_mousePosition)) 
            {
                _hotId = id;
            }

            if (_hotId == id)
            {
                if (_mouseDown)
                {
                    _activeId = id;
                }

                if (_activeId == id && !_mouseDown)
                {
                    pressed = true;
                }
            }

            if (_hotId == id)
            {
                color = hot;
            }
            else 
            {
                color = normal;

                if (_activeId == id) 
                {
                    _activeId = -1;
                }
            }

            if (_activeId == id) 
            {
                color = active;
            }

            renderer.DrawRectangle(bounds, color);
            renderer.DrawString(SharedContent.Font, text, new Vector2(bounds.X, bounds.Y), Color.Black);

            return pressed;
        }
    }
}
