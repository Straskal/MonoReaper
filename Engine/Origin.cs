using Engine.Collision;

namespace Engine
{
    public abstract class Origin
    {
        public static Origin TopLeft { get; } = new TopLeftOrigin();
        public static Origin Center { get; } = new CenterOrigin();

        public abstract RectangleF Transform(RectangleF rectangle);
        public abstract RectangleF Invert(RectangleF rectangle);

        public RectangleF Tranform(float x, float y, float width, float height)
        {
            return Transform(new RectangleF(x, y, width, height));
        }

        public RectangleF Invert(float x, float y, float width, float height)
        {
            return Invert(new RectangleF(x, y, width, height));
        }
    }

    public class TopLeftOrigin : Origin
    {
        public override RectangleF Transform(RectangleF rectangle)
        {
            return rectangle;
        }

        public override RectangleF Invert(RectangleF rectangle)
        {
            return rectangle;
        }
    }

    public class CenterOrigin : Origin
    {
        public override RectangleF Transform(RectangleF rectangle)
        {
            rectangle.X -= rectangle.Width * 0.5f;
            rectangle.Y -= rectangle.Height * 0.5f;
            return rectangle;
        }

        public override RectangleF Invert(RectangleF rectangle)
        {
            rectangle.X += rectangle.Width * 0.5f;
            rectangle.Y += rectangle.Height * 0.5f;
            return rectangle;
        }
    }
}
