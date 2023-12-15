
using Adventure;
using Engine;
using Engine.Graphics;

using var application = new App(256, 256, ResolutionScaleMode.Viewport);
application.Screens.Push(new RootScreen(application));
application.Run();