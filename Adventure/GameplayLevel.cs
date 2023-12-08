using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;
using Engine.Actions;
using Engine.Graphics;

namespace Adventure
{
    internal class GameplayLevel : Level
    {
        private readonly PressedAction toggleDebug;
        private readonly PressedAction toggleFullscreen;
        private readonly PressedAction quit;

        private SpriteFont _spriteFont;

        public GameplayLevel(int cellSize, int width, int height) : base(cellSize, width, height)
        {
            toggleDebug = Input.NewPressedAction(Keys.OemTilde);
            toggleFullscreen = Input.NewPressedAction(Keys.F);
            quit = Input.NewPressedAction(Keys.Escape);
        }

        public override void Start()
        {
            _spriteFont = Content.Load<SpriteFont>("Fonts/Font");

            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (toggleDebug.WasPressed())
            {
                App.Instance.ToggleDebug();
            }

            if (toggleFullscreen.WasPressed())
            {
                App.Instance.ToggleFullscreen();
            }

            if (quit.WasPressed())
            {
                App.Instance.Exit();
            }
        }

        public override void Draw(bool debug)
        {
            base.Draw(debug);

            if (debug) 
            {
                var mouseWorldPosition = Camera.ToWorld(Mouse.GetState().Position.ToVector2());

                foreach (var box in Partition.Query(mouseWorldPosition)) 
                {
                    if (box.CalculateBounds().Contains(mouseWorldPosition)) 
                    {
                        Renderer.BeginDraw(Camera.TransformationMatrix);
                        Renderer.DrawRectangle(box.CalculateBounds().ToXnaRect(), new Color(Color.Blue, 0.2f));
                        Renderer.DrawString(_spriteFont, box.CalculateBounds().Position.ToString(), mouseWorldPosition, Color.White);
                        Renderer.EndDraw();
                        break;
                    }
                }
            }
        }
    }
}
