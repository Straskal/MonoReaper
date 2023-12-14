
using Adventure;
using Engine;
using Engine.Graphics;

using var application = new App(256, 256, ResolutionScaleMode.Viewport);
application.Stack.Push(new RootState(application));
application.Run();