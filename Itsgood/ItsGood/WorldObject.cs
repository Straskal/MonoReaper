using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{
    public class WorldObject
    {
        public WorldObject(Layout layout) 
        {
            Layout = layout;
        }

        public Layout Layout { get; }
        public string ImageFilePath { get; set; }
        public Texture2D Image { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public bool IsMirrored { get; set; }

        public Rectangle Bounds => 
            new Rectangle((int)(Position.X - Source.Width * 0.5), 
                (int)(Position.Y - Source.Height * 0.5), 
                Source.Width, Source.Height
            );

        internal List<Behavior> Behaviors { get; } = new List<Behavior>();
        internal bool MarkedForDestroy { get; private set; }

        public T GetBehavior<T>() where T : Behavior 
        {
            return Behaviors.FirstOrDefault(behavior => behavior is T) as T;
        }

        internal void MarkForDestroy() 
        {
            MarkedForDestroy = true;
        }
    }
}
