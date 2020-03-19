using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Tiled.Extensions;

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
                IsFullscreen = true,
                IsResizable = true,
                IsBordered = true,
                IsVsyncEnabled = true
            };

            using (var game = MainGameFactory.Create(settings))
            {
                game.LoadTiledLayout("C:/git/MonoReaper/Reaper/Content/Levels/TestLevel.json");
                game.Run();
            }
        }
    }
}
