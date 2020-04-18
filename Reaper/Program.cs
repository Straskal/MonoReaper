using Reaper.Engine;
using System;

namespace Reaper
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var settings = new GameSettings
            {
                ViewportWidth = 640,
                ViewportHeight = 360,
                IsFullscreen = false,
            };

            using (var game = new MainGame(settings))
            {
                game.Singletons.Register(new InputManager(game));
                game.Singletons.Register(new GlobalInputHandler(game));
                InputBindings.Initialize(game.Singletons.Get<InputManager>());
                game.LoadOgmoLayout("content/layouts/dungeontest.json");
                //game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
