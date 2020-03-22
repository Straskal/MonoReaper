using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Ogmo;
using Reaper.Engine.Singletons;
using Microsoft.Xna.Framework.Input;

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
                var input = game.Singletons.Get<Input>();

                var move = input.NewAxisAction("move");
                move.AddKeys(Keys.A, Keys.D);

                var jump = input.NewPressedAction("jump");
                jump.AddKey(Keys.Space);

                var attack = input.NewPressedAction("attack");
                attack.AddKey(Keys.Left);

                var toggleFullscreen = input.NewPressedAction("toggleFullscreen");
                toggleFullscreen.AddKey(Keys.F);

                var exitGameAction = input.NewPressedAction("exitGame");
                exitGameAction.AddKey(Keys.Escape);

                game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }
    }
}
