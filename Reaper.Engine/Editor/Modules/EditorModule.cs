using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine.Editor.Modules
{
    public abstract class EditorModule
    {
        protected readonly EditorState EditorState;

        public EditorModule(EditorState editorState)
        {
            EditorState = editorState ?? throw new ArgumentNullException(nameof(editorState));
        }

        public virtual void Start(GameTime gameTime) { }
        public virtual void Tick(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}
