using Reaper.Engine;
using System;

namespace Reaper
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var settings = new AppSettings
            {
                ViewportWidth = 640,
                ViewportHeight = 360,
                IsFullscreen = false,
            };

            using (var game = new App(settings))
            {
                //game.LoadOgmoLayout("content/layouts/layout1.json");
                game.LoadOgmoLayout("content/layouts/level_0.json");
                //game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
