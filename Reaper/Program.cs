using Core;
using System;

namespace Reaper
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new App())
            {
                //game.LoadOgmoLayout("content/layouts/layout1.json");
                game.LoadOgmoLayout("content/layouts/level_0.json");
                //game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
