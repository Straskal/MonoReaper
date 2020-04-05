using Reaper.Engine.Behaviors;
using System.Collections.Generic;

using GUI = ImGuiNET.ImGui;

namespace Reaper.Engine.Editor.Tools
{
    internal class WorldObjectField
    {
        private WorldObjectField() { }

        public WorldObject Instance { get; set; }
        public List<PropertyField> Properties { get; set; } = new List<PropertyField>();
        public List<BehaviorField> Behaviors { get; set; } = new List<BehaviorField>();
        public EditorState EditorState { get; set; }

        public static WorldObjectField For(EditorState editorState, WorldObject worldObject)
        {
            var editableObject = new WorldObjectField { EditorState = editorState, Instance = worldObject };

            foreach (var property in worldObject.Type.GetType().GetProperties())
            {
                var field = PropertyField.For(worldObject.Type, property);
                if (field != null)
                    editableObject.Properties.Add(field);
            }

            foreach (var behavior in worldObject.Type.Behaviors)
                editableObject.Behaviors.Add(BehaviorField.FromBehavior(behavior));

            return editableObject;
        }

        public void Draw()
        {
            WorldObjectPropertyFields();
            WorldObjectBehaviorFields();
            AddBehaviorButton();

            Instance.UpdateBBox();
        }

        private void WorldObjectPropertyFields()
        {
            GUI.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Properties");
            GUI.NewLine();

            foreach (var propertyField in Properties)
                propertyField.Draw();
        }

        private void WorldObjectBehaviorFields()
        {
            GUI.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Behaviors");
            GUI.NewLine();

            foreach (var behavior in Behaviors)
            {
                GUI.Text(behavior.Instance.GetType().Name);
                GUI.SameLine(GUI.GetWindowWidth() - 60);
                GUI.Button("Remove");

                foreach (var properyField in behavior.Properties)
                    properyField.Draw();

                GUI.NewLine();
            }

            GUI.NewLine();
        }

        private void AddBehaviorButton()
        {
            GUI.SameLine(GUI.GetWindowWidth() - 60);
            if (GUI.Button("Add")) 
            {
                Instance.AddBehavior<PlatformerBehavior>();
                Behaviors.Add(BehaviorField.FromBehavior(Instance.GetBehavior<PlatformerBehavior>()));
            }
        }
    }
}
