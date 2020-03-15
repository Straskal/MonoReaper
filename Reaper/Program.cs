using Reaper.Extensions;
using Reaper.Objects;
using Reaper.Engine;
using System;

namespace Reaper
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Definitions.Register();

            var settings = new GameSettings 
            {
                ViewportWidth = 320,
                ViewportHeight = 180,
                IsFullscreen = false,
                IsResizable = true,
                IsBordered = true,
                IsVsyncEnabled = true
            };

            using (var game = MainGameFactory.Create(settings))
            {
                game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
