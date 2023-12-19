namespace Engine
{
    /// <summary>
    /// Base class for origin transform implementations
    /// </summary>
    public abstract class Origin
    {
        public static Origin TopLeft 
        {
            get;
        } = new TopLeftOrigin();

        public static Origin Center 
        { 
            get;
        } = new CenterOrigin();

        /// <summary>
        /// Applies the origin to the given rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public abstract RectangleF Transform(RectangleF rectangle);

        /// <summary>
        /// Unapplies the origin from the given rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public abstract RectangleF Invert(RectangleF rectangle);

        /// <summary>
        /// Applies the origin to the given rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public RectangleF Tranform(float x, float y, float width, float height)
        {
            return Transform(new RectangleF(x, y, width, height));
        }

        /// <summary>
        /// Unapplies the origin from the given rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public RectangleF Invert(float x, float y, float width, float height)
        {
            return Invert(new RectangleF(x, y, width, height));
        }

        private class TopLeftOrigin : Origin
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

        private class CenterOrigin : Origin
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
}
