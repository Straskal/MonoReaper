using System.Collections.Generic;

namespace Adventure.Content
{
    public sealed class EntityDataFields : Dictionary<string, string>
    {
        public EntityDataFields() : base()
        {
        }

        public EntityDataFields(Dictionary<string, string> keyValuePairs) : base(keyValuePairs)
        {
        }

        public string GetString(string name)
        {
            return this[name];
        }
    }
}
