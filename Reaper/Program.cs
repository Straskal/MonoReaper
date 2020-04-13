using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Ogmo;
using Reaper.Singletons;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Reaper
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            InvokeStaticDefinitionCtors();

            var settings = new GameSettings
            {
                ViewportWidth = 640,
                ViewportHeight = 360,
                IsFullscreen = false,
            };

            using (var game = new MainGame(settings))
            {
                game.Singletons.Register(new InputManager(game));
                game.Singletons.Register(new GlobalInputHandler(game));
                InputBindings.Initialize(game.Singletons.Get<InputManager>());
                game.LoadOgmoLayout("content/layouts/dungeon.json");
                //game.LoadOgmoLayout(args[0]);
                game.Run();
            }
        }

        private static void InvokeStaticDefinitionCtors() 
        {
            var definitionClasses = typeof(Program).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(DefinitionAttribute), true).Length > 0);
            foreach (var definitionClass in definitionClasses)
            {
                RuntimeHelpers.RunClassConstructor(definitionClass.TypeHandle);
            }
        }
    }
}
