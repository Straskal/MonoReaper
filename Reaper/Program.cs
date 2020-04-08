using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Ogmo;
using Reaper.Singletons;

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
                IsFullscreen = true,
            };

            using (var game = MainGameFactory.Create(settings))
            {
                Definitions.Register();
                game.Singletons.Register(new InputManager());
                InputBindings.Initialize(game.Singletons.Get<InputManager>());
                game.LoadOgmoLayout("content/layouts/layout1.json");
                game.Run();
            }
        }
    }
}
