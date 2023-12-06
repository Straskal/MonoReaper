
using Adventure;
using Engine;

using (var game = new App())
{
    //game.LoadOgmoLayout("content/layouts/layout1.json");
    //game.LoadOgmoLevel("content/layouts/level_0.json");
    //game.LoadOgmoLayout(args[0]);
    game.LoadInitialLevel = () => LevelLoader.LoadLevel(game, "Levels/world/level_0");
    //game.LoadLevel("Levels/world/level_0");
    game.Run();
}
