
using Adventure;
using Engine;

using var application = new App(200, 200);
application.LoadInitialLevel = () => LevelLoader.LoadLevel(application, "Levels/world/level_0");
application.Run();