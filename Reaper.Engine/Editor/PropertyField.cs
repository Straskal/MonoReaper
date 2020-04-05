using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

using GUI = ImGuiNET.ImGui;

namespace Reaper.Engine.Editor
{
    public abstract class PropertyField
    {
        protected readonly object _instance;
        protected readonly PropertyInfo _propertyInfo;

        public PropertyField(object instance, PropertyInfo propertyInfo)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        protected string DisplayName => Regex.Replace(_propertyInfo.Name, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");

        public static PropertyField For(object instance, PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.PropertyType;

            switch (type)
            {
                case Type @float when @float == typeof(float):
                    return new FloatField(instance, propertyInfo);

                case Type @int when @int == typeof(int):
                    return new IntField(instance, propertyInfo);

                case Type @string when @string == typeof(string):
                    return new StringField(instance, propertyInfo);

                case Type point when point == typeof(Point):
                    return new PointField(instance, propertyInfo);

                case Type spatialType when spatialType == typeof(SpatialType):
                    return new SpatialTypeField(instance, propertyInfo);
            }

            return null;
        }

        public abstract void Draw();
    }

    public class FloatField : PropertyField
    {
        public FloatField(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }

        public override void Draw()
        {
            float fv = (float)_propertyInfo.GetValue(_instance);
            if (GUI.InputFloat(DisplayName, ref fv))
                _propertyInfo.SetValue(_instance, fv);
        }
    }

    public class IntField : PropertyField
    {
        public IntField(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }

        public override void Draw()
        {
            int iv = (int)_propertyInfo.GetValue(_instance);
            if (GUI.InputInt(DisplayName, ref iv))
                _propertyInfo.SetValue(_instance, iv);
        }
    }

    public class StringField : PropertyField
    {
        public StringField(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }

        public override void Draw()
        {
            string strv = (string)_propertyInfo.GetValue(_instance) ?? string.Empty;
            if (GUI.InputText(DisplayName, ref strv, 512))
                _propertyInfo.SetValue(_instance, strv);
        }
    }

    public class PointField : PropertyField
    {
        public PointField(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) { }

        public override void Draw()
        {
            Point pv = (Point)_propertyInfo.GetValue(_instance);
            if (GUI.InputInt($"{DisplayName} X", ref pv.X) || GUI.InputInt($"{DisplayName} Y", ref pv.Y))
                _propertyInfo.SetValue(_instance, pv);
        }
    }

    public class SpatialTypeField : PropertyField
    {
        private readonly string[] _items;

        private int _current;

        public SpatialTypeField(object instance, PropertyInfo propertyInfo) : base(instance, propertyInfo) 
        {
            _items = new string[]
            {
                SpatialType.Pass.ToString(),
                SpatialType.Overlap.ToString(),
                SpatialType.Solid.ToString()
            };

            SpatialType type = (SpatialType)propertyInfo.GetValue(instance);

            for (int i = 0; i < _items.Length - 1; i++) 
            {
                if (type.ToString() == _items[i])
                {
                    _current = i;
                    break;
                }
            }
        }

        public override void Draw()
        {
            if (GUI.Combo(DisplayName, ref _current, _items, _items.Length))
                _propertyInfo.SetValue(_instance, Enum.Parse(typeof(SpatialType), _items[_current]));
        }
    }
}
