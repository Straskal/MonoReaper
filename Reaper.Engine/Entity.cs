using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Reaper.Engine
{
    public sealed class Entity
    {
        private readonly List<Component> components = new();
        private readonly Dictionary<string, Vector2> points = new();

        public Level Level { get; internal set; }

        public bool IsMirrored { get; set; }

        public bool IsDestroyed { get; internal set; }
        public List<Component> Components => components;

        public Vector2 Position { get; set; }
        public Origin Origin { get; set; }

        public void AddComponent(Component component)
        {
            components.Add(component);

            Level?.AddComponent(this, component);
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);

            Level?.RemoveComponent(component);
        }

        public T GetComponent<T>() where T : class
        {
            var result = default(T);

            foreach (var component in components) 
            {
                if (component is T asT) 
                {
                    result = asT;
                    break;
                }
            }

            return result;
        }

        public T GetComponentOrThrow<T>() where T : class
        {
            return GetComponent<T>() ?? throw new Exception($"Required behavior of type {typeof(T).Name} is missing.");
        }

        public bool TryGetComponent<T>(out T component) where T : class
        {
            component = GetComponent<T>();

            return component != null;
        }

        public void AddPoint(string name, Vector2 point) 
        {
            points[name] = point;
        }

        public Vector2 GetPoint(string name) 
        {
            if (!points.TryGetValue(name, out var point))
            {
                throw new ArgumentException($"Could not find point {name}");
            }

            return point;
        }

        public void Destroy()
        {
            Level.Destroy(this);
        }
    }
}
