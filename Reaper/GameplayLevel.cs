using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Reaper
{
    internal class GameplayLevel : Level
    {
        private readonly PressedAction toggleDebug;
        private readonly PressedAction toggleFullscreen;
        private readonly PressedAction quit;

        public GameplayLevel(int cellSize, int width, int height) : base(cellSize, width, height)
        {
            toggleDebug = Input.NewPressedAction(Keys.OemTilde);
            toggleFullscreen = Input.NewPressedAction(Keys.F);
            quit = Input.NewPressedAction(Keys.Escape);
        }

        public override void Start()
        {
            //var negativeEffect = content.Load<Effect>("shaders/negative");
            var distortionEffect = content.Load<Effect>("shaders/distortion");

            //AddPostProcessingEffect(new NegativePostProcessEffect(negativeEffect));
            AddPostProcessingEffect(new DistortionPostProcessingEffect(distortionEffect));

            base.Start();
        }

        public override void Tick(GameTime gameTime)
        {
            base.Tick(gameTime);

            if (toggleDebug.WasPressed()) 
            {
                App.Current.ToggleDebug();
            }

            if (toggleFullscreen.WasPressed())
            {
                App.ToggleFullscreen();
            }

            if (quit.WasPressed()) 
            {
                App.Current.Exit();
            }
        }
    }
}
