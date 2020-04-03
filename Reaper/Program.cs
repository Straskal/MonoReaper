using Reaper.Objects;
using Reaper.Engine;
using System;
using Reaper.Ogmo;
using Reaper.Engine.Singletons;
using Reaper.Editor;

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
                IsFullscreen = false,
            };

            using (var game = MainGameFactory.Create(settings))
            {
                Definitions.Register();
                InputBindings.Initialize(game.Singletons.Get<Input>());
                //game.Singletons.Register(new Dialogue(game));

                //game.ChangeState(new EditorState());
                game.ChangeState(new MainGameState());
                game.LoadOgmoLayout("content/layouts/layout1.json");
                game.Run();
            }
        }
    }
}
