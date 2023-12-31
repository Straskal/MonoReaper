
using Engine;
using Microsoft.Xna.Framework;
using Engine.Collision;
using Engine.Graphics;
using Adventure;

//var path = new SegmentF(Vector2.UnitY, Vector2.UnitY + new Vector2(30f, 0.5f));
//var rectangle = new RectangleF(30f, 1f, 10f, 10f);
//var circle = new CircleF(10f, 0f, 5f);
//var circle2 = new CircleF(30f, 0f, 5f);

//var intersected = Intersection.MovingCircleVsCircle(circle, path, circle2, out var time, out var contact, out var normal);
//intersected = Intersection.MovingCircleVsRectangle(circle, path, rectangle, out time, out contact, out normal);

//var e = intersected;

using var application = new App(256, 256, ResolutionScaleMode.Viewport);
application.Screens.Push(new RootScreen(application));
application.Run();