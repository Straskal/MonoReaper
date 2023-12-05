using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core
{
    public sealed class Entity
    {
        private readonly List<Component> _components = new();

        public Entity(Origin origin = Origin.TopLeft)
        {
            Origin = origin;
        }

        public Entity(Origin origin = Origin.TopLeft, params Component[] components)
        {
            Origin = origin;

            _components.AddRange(components);
        }

        public Level Level 
        { 
            get; 
            internal set; 
        }

        public Vector2 Position 
        { 
            get; 
            set; 
        }

        public Origin Origin 
        { 
            get; 
            set; 
        }

        public bool IsDestroyed 
        { 
            get; 
            internal set; 
        }

        internal List<Component> Components => _components;

        public void AddComponent(Component component)
        {
            _components.Add(component);

            Level?.AddComponent(this, component);
        }

        public void RemoveComponent(Component component)
        {
            _components.Remove(component);

            Level?.RemoveComponent(component);
        }

        public T GetComponent<T>() where T : class
        {
            var result = default(T);    

            foreach (var component in _components) 
            {
                if (component is T asT) 
                {
                    result = asT;
                    break;
                }
            }

            return result;
        }

        public bool TryGetComponent<T>(out T component) where T : class
        {
            return (component = GetComponent<T>()) != null;
        }

        public T RequireComponent<T>() where T : class
        {
            return GetComponent<T>() ?? throw new Exception($"Required behavior of type {typeof(T).Name} is missing.");
        }
    }
}
