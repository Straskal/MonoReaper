﻿using Reaper.Objects;
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
                ViewportWidth = 320,
                ViewportHeight = 180,
                IsFullscreen = false,
                IsResizable = true,
                IsBordered = true,
                IsVsyncEnabled = true
            };

            using (var game = MainGameFactory.Create(settings))
            {
                Definitions.Register();
                InputBindings.Initialize(game.Singletons.Get<Input>());

                game.LoadOgmoLayout("testlevel.json");
                game.Run();
            }
        }
    }
}
