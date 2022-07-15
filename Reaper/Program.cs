using Core;
using System;
using System.IO;

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
                game.LoadOgmoLevel("content/layouts/level_0.json");
                //game.LoadOgmoLayout(args[0]);
                try
                {
                    game.Run();
                }
                catch (Exception e) 
                {
                    File.WriteAllText("crash.txt", e.Message);
                }
            }
        }
    }
}
