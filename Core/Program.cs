using Core.Objects;
using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using System;

namespace Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
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
                game.RunningLayout.Spawn(Definitions.Get("player"), new Vector2(32, 32));
                game.RunningLayout.Spawn(Definitions.Get("other"), new Vector2(64, 32));

                var tile = new WorldObjectDefinition(32, 32);
                tile.AddImage("tiles", new Rectangle(0, 0, 32, 32));
                tile.MakeSolid();

                int num = 320 / 32;

                for (int i = 0; i < num; i++) 
                {
                    game.RunningLayout.Spawn(tile, new Vector2(i * 32, 160));
                }

                game.Run();
            }
        }
    }
}
