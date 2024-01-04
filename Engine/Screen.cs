using System;
using Microsoft.Xna.Framework;

namespace Engine
{
    public abstract class Screen
    {
        public Screen(App application) 
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        public App Application 
        {
            get;
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
        }
    }
}
