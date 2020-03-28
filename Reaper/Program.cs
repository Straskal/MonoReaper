using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Ogmo;
using Reaper.Engine.Singletons;

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
                InputBindings.Initialize(game.Singletons.Get<Input>());
                //game.Singletons.Register(new Dialogue(game));

                game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
