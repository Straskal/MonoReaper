using System;
using System.Reflection;
using System.Text.RegularExpressions;

using GUI = ImGuiNET.ImGui;

namespace Reaper.Editor
{
    public class PropertyField
    {
        public static void For(object instance, PropertyInfo propertyInfo) 
        {
            Type type = propertyInfo.PropertyType;
            string displayName = DisplayName(propertyInfo.Name);

            switch (type) 
            {
                case Type @float when @float == typeof(float):
                    {
                        float fv = (float)propertyInfo.GetValue(instance);
                        GUI.InputFloat(displayName, ref fv);
                        propertyInfo.SetValue(instance, fv);
                    }
                    break;

                case Type @string when @string == typeof(string):
                    {
                        string strv = (string)propertyInfo.GetValue(instance) ?? string.Empty;
                        GUI.InputText(displayName, ref strv, 512);
                        propertyInfo.SetValue(instance, strv);
                    }
                    break;
            }
        }

        private static string DisplayName(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}
