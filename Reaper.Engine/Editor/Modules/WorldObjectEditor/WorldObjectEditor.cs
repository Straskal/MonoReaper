using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Engine.Editor.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using GUI = ImGuiNET.ImGui;

namespace Reaper.Engine.Editor.Tools
{
    public class WorldObjectEditor : EditorModule
    {
        private WorldObject _currentWorldObject;
        private WorldObjectField _worldObjectField;
        private List<Type> _behaviorTypes;
        private int _selectedWorldObject;
        private bool _isSaving;

        private MainGame Game => EditorState.Game;

        public WorldObjectEditor(EditorState editorState) : base(editorState) { }

        public WorldObject CurrentWorldObject
        {
            get => _currentWorldObject;
            set => SetWorldObject(value);
        }

        public override void Start(GameTime gameTime) 
        {
            var builtinBehaviors = GetType().Assembly.GetTypes().Where(type => typeof(Behavior).IsAssignableFrom(type));
            var gameBehaviors = Assembly.Load("Reaper").GetTypes().Where(type => typeof(Behavior).IsAssignableFrom(type)); ;

            _behaviorTypes = new List<Type>();
            _behaviorTypes.AddRange(builtinBehaviors);
            _behaviorTypes.AddRange(gameBehaviors);
        }

        public override void Draw(GameTime gameTime)
        {
            GUI.Begin("World Object", ImGuiNET.ImGuiWindowFlags.MenuBar);

            MenuBar();

            if (CurrentWorldObject != null)
                _worldObjectField.Draw();

            GUI.End();

            GUI.Begin("World Object List");
            GUI.PushItemWidth(-1);
            GUI.ListBox("ok", ref _selectedWorldObject, new[]
            {
                "player.json",
                "enemy.json",
                "npc.json",
                "spawnPoint.json"
            }, 4);
            GUI.PopItemWidth();
            GUI.End();

            if (_isSaving)
            {
                GUI.BeginPopupModal("Saved");
                GUI.Text("Ur shit was saved, guy!");
                if (GUI.Button("Oh hell yeah"))
                {
                    GUI.CloseCurrentPopup();
                    _isSaving = false;
                }
                GUI.EndPopup();
            }
        }

        public void SetWorldObject(WorldObject worldObject)
        {
            _currentWorldObject = worldObject;
            _worldObjectField = WorldObjectField.For(EditorState, worldObject);

            Game.CurrentLayout.View.ClampToLayout = false;
            Game.CurrentLayout.View.Position = CurrentWorldObject.Position;
        }

        private void MenuBar()
        {
            GUI.BeginMenuBar();
            if (GUI.BeginMenu("File"))
            {
                if (GUI.MenuItem("New", "Ctrl+N"))
                {
                    CurrentWorldObject = Game.CurrentLayout.Spawn(new WorldObjectType(), Vector2.Zero);
                }
                if (GUI.MenuItem("Save", "Ctrl+S"))
                {
                    var serializer = new DataContractJsonSerializer(
                            CurrentWorldObject.Type.GetType(),
                            _behaviorTypes);

                    using (var stream = File.Create($"{Directory.GetCurrentDirectory()}/wos/{CurrentWorldObject.Type.Name}.json"))
                        serializer.WriteObject(stream, CurrentWorldObject.Type);

                    _isSaving = true;
                }

                GUI.EndMenu();
            }
            GUI.EndMenuBar();
        }
    }
}
