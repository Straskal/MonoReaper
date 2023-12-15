
using Engine;
using Engine.Graphics;
using Adventure;

using var application = new App(256, 256, ResolutionScaleMode.Viewport);
application.Screens.Push(new RootScreen(application));
application.Run();