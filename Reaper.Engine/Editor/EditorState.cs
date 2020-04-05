using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine.Editor.Tools;
using Reaper.ImGui;

using GUI = ImGuiNET.ImGui;

namespace Reaper.Engine.Editor
{
    public class EditorState : MainGameState
    {
        private ImGUIRenderer _imGuiRenderer;
        private WorldObjectEditor _woEditor;
        private float _previousScroll;

        public override void Start()
        {
            _imGuiRenderer = new ImGUIRenderer(Game).Initialize().RebuildFontAtlas();

            GUI.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

            Game.IsMouseVisible = true;
            Game.Window.AllowUserResizing = true;
            Game.GpuManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Game.GpuManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Game.GpuManager.ApplyChanges();

            _woEditor = new WorldObjectEditor(this);
            _woEditor.Start(null);
        }

        public override void Tick(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue > _previousScroll) 
            {
                Game.CurrentLayout.View.Zoom += 1f;
            }
            if (mouseState.ScrollWheelValue < _previousScroll)
            {
                Game.CurrentLayout.View.Zoom -= 1f;
            }

            _previousScroll = mouseState.ScrollWheelValue;

            Game.CurrentLayout.SyncFrame();

            base.Tick(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.CurrentLayout.Draw();
            Game.CurrentLayout.DebugDraw();

            _imGuiRenderer.BeginLayout(gameTime);

            MenuBar();

            _woEditor.Draw(gameTime);
            _imGuiRenderer.EndLayout();
        }

        private static void MenuBar()
        {
            GUI.BeginMainMenuBar();

            if (GUI.BeginMenu("File"))
            {
                if (GUI.MenuItem("New", "Ctrl+N")) { /* Do stuff */ }
                if (GUI.MenuItem("Open", "Ctrl+O")) { /* Do stuff */ }
                if (GUI.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }

                GUI.EndMenu();
            }

            GUI.EndMainMenuBar();
        }
    }
}
