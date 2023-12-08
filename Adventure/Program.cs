
using Adventure;
using Engine;
using Engine.Graphics;

using var application = new App(200, 200, ResolutionScaleMode.Camera);
application.LoadInitialLevel = () => LevelLoader.LoadLevel(application, "Levels/world/level_0");
application.Run();