using System.Collections.Generic;

namespace Reaper.Engine.Editor.Tools
{
    internal class BehaviorField
    {
        private BehaviorField() { }

        public Behavior Instance { get; set; }
        public List<PropertyField> Properties { get; set; } = new List<PropertyField>();

        public static BehaviorField FromBehavior(Behavior behavior) 
        {
            var editableBehavior = new BehaviorField { Instance = behavior };

            foreach (var property in editableBehavior.Instance.GetType().GetProperties())
            {
                var field = PropertyField.For(behavior, property);
                if (field != null)
                    editableBehavior.Properties.Add(field);
            }

            return editableBehavior;
        }
    }
}
