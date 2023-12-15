using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// An entity is an object that has a physical position in a level
    /// </summary>
    /// <remarks>
    /// Entities are composed of components, which hold the actual game and rendering logic.
    /// </remarks>
    public class Entity
    {
        public Entity(Origin origin = Origin.TopLeft)
        {
            Origin = origin;
        }

        public Entity(Origin origin = Origin.TopLeft, params Component[] components)
        {
            Origin = origin;
            Components.AddRange(components);
        }

        /// <summary>
        /// Gets the entity's component list
        /// </summary>
        internal List<Component> Components
        {
            get;
        } = new List<Component>();

        /// <summary>
        /// Gets the entity's level
        /// </summary>
        public Level Level
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the entity's position
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity's origin
        /// </summary>
        public Origin Origin
        {
            get;
            set;
        }

        /// <summary>
        /// True if the entity has been destroyed
        /// </summary>
        public bool IsDestroyed
        {
            get;
            internal set;
        }

        /// <summary>
        /// Adds the given component to the entity
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(Component component)
        {
            Components.Add(component);
            Level?.AddComponent(this, component);
        }

        /// <summary>
        /// Removes the given component from the entity
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(Component component)
        {
            Components.Remove(component);
            Level?.RemoveComponent(component);
        }

        /// <summary>
        /// Gets the first instance of a component of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : class
        {
            var result = default(T);

            foreach (var component in Components)
            {
                if (component is T t)
                {
                    result = t;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true if the entity contains a component of the given type and outputs it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetComponent<T>(out T component) where T : class
        {
            return (component = GetComponent<T>()) != null;
        }

        /// <summary>
        /// Gets the first instance of a component of the given type and throws an exception if the component is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T RequireComponent<T>() where T : class
        {
            return GetComponent<T>() ?? throw new Exception($"Required behavior of type {typeof(T).Name} is missing.");
        }
    }
}
