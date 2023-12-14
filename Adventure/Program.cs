
using Adventure;
using Engine;
using Engine.Graphics;

using var application = new App(256, 256, ResolutionScaleMode.Camera);
application.Stack.Push(new RootState());
application.Run();