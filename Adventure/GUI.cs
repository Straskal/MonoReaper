using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine;

namespace Adventure
{
    internal static class GUI
    {
        public static BackBuffer BackBuffer
        {
            get;
            set;
        }

        public static Renderer Renderer
        {
            get;
            set;
        }

        private static int _hoverId;
        private static int _activeId;
        private static bool _mouseDown;

        public static void Start()
        {
            _mouseDown = Mouse.GetState().LeftButton == ButtonState.Pressed;
            _hoverId = 0;
        }

        public static bool PrimaryButton(int id, string text, int x, int y)
        {
            return Button(id, text, x, y, Color.LightBlue, Color.White, Color.CadetBlue);
        }

        public static bool DangerButton(int id, string text, int x, int y)
        {
            return Button(id, text, x, y, Color.Red, Color.Pink, Color.DarkRed);
        }

        public static bool Checkbox(int id, bool isChecked, int x, int y)
        {
            var checkboxRectangle = new Rectangle(x, y, 10, 10);

            if (checkboxRectangle.Contains(Input.MousePosition))
            {
                _hoverId = id;

                if (_mouseDown)
                {
                    _activeId = id;
                }

                if (_activeId == id && !_mouseDown)
                {
                    isChecked = !isChecked; 
                    _activeId = 0;
                }
            }
            else if (_activeId == id)
            {
                _activeId = 0;
            }

            if (isChecked)
            {
                Renderer.DrawRectangle(checkboxRectangle, Color.White);
            }
            else 
            {
                Renderer.DrawRectangleOutline(checkboxRectangle, Color.White);
            }

            return isChecked;
        }

        public static bool Button(int id, string text, int x, int y, Color idle, Color hover, Color active)
        {
            var pressed = false;
            Color color;
            var size = Store.Fonts.Default.MeasureString(text);
            size.Round();
            var buttonRectangle = new Rectangle(x, y, (int)size.X, (int)size.Y);
            buttonRectangle.Inflate(5, 0);

            if (buttonRectangle.Contains(Input.MousePosition))
            {
                _hoverId = id;

                if (_mouseDown)
                {
                    _activeId = id;
                }

                if (_activeId == id && !_mouseDown)
                {
                    pressed = true;
                }
            }
            else if (_activeId == id)
            {
                _activeId = 0;
            }

            if (_hoverId == id)
            {
                color = hover;
            }
            else
            {
                color = idle;

                if (_activeId == id)
                {
                    _activeId = -1;
                }
            }

            if (_activeId == id)
            {
                color = active;
            }

            Renderer.DrawRectangle(buttonRectangle, color);
            Renderer.DrawString(Store.Fonts.Default, text, new Vector2(x, y), Color.Black);

            if (pressed) 
            {
                _activeId = 0;
            }

            return pressed;
        }
    }
}
