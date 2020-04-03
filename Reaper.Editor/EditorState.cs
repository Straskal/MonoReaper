using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.ImGui;
using System.Collections.Generic;
using System.Reflection;

using GUI = ImGuiNET.ImGui;

namespace Reaper.Editor
{
    internal class EditBehavior 
    {
        public Behavior Instance { get; set; }
        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
    }

    internal class EditObject 
    {
        public WorldObject Instance { get; set; }
        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
        public List<EditBehavior> Behaviors { get; set; } = new List<EditBehavior>();
    }

    public class EditorState : MainGameState
    {
        private ImGUIRenderer _imGuiRenderer;
        private EditObject _editObject = new EditObject();

        public EditorState()
        {
        }

        public WorldObject CurrentWorldObject { get; private set; }

        public override void Start()
        {
            Game.IsMouseVisible = true;

            _imGuiRenderer = new ImGUIRenderer(Game).Initialize().RebuildFontAtlas();
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            _imGuiRenderer.BeginLayout(gameTime);

            GUI.BeginMainMenuBar();

            if (GUI.BeginMenu("File"))
            {
                if (GUI.MenuItem("New", "Ctrl+N")) { /* Do stuff */ }
                if (GUI.MenuItem("Open", "Ctrl+O")) { /* Do stuff */ }
                if (GUI.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }

                GUI.EndMenu();
            }

            GUI.EndMainMenuBar();

            GUI.Begin("World Object", ImGuiNET.ImGuiWindowFlags.MenuBar);
            if (GUI.BeginMenuBar())
            {
                if (GUI.BeginMenu("File"))
                {
                    if (GUI.MenuItem("New", "Ctrl+N")) { /* Do stuff */ }
                    if (GUI.MenuItem("Open", "Ctrl+O")) { /* Do stuff */ }
                    if (GUI.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }

                    GUI.EndMenu();
                }
                if (GUI.BeginMenu("Behaviors"))
                {
                    if (GUI.MenuItem("Add", "Ctrl+N")) { /* Do stuff */ }

                    GUI.EndMenu();
                }
                GUI.EndMenuBar();

                GUI.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Properties");
                GUI.NewLine();

                foreach (var propertyInfo in _editObject.Properties)
                {
                    PropertyField.For(_editObject.Instance, propertyInfo);
                }

                GUI.NewLine();
                GUI.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Behaviors");
                GUI.NewLine();

                foreach (var behavior in _editObject.Behaviors) 
                {
                    GUI.Text(behavior.Instance.GetType().Name);

                    foreach (var propertyInfo in behavior.Properties)
                    {
                        PropertyField.For(behavior.Instance, propertyInfo);
                    }

                    GUI.NewLine();
                }
            }

            GUI.End();

            _imGuiRenderer.EndLayout();
        }

        public void SetWorldObject(WorldObject worldObjectData) 
        {
            
        }
    }
}
