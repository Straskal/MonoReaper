using System.Collections.Generic;

namespace Adventure.Content
{
    public sealed class EntityFields : Dictionary<string, string>
    {
        public EntityFields() : base()
        {
        }

        public EntityFields(Dictionary<string, string> keyValuePairs) : base(keyValuePairs)
        {
        }

        public string GetString(string name)
        {
            return this[name];
        }
    }
}
