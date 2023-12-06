
using Adventure;
using Engine;

using var application = new App();
application.LoadInitialLevel = () => LevelLoader.LoadLevel(application, "Levels/world/level_0");
application.Run();