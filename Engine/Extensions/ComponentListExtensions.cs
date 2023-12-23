using Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Extensions
{
    internal static class ComponentListExtensions
    {
        public static void Spawn(this List<Component> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnSpawn();
            }
        }

        public static void Start(this List<Component> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnStart();
            }
        }

        public static void Update(this List<Component> components, GameTime gameTime) 
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnUpdate(gameTime);
            }
        }

        public static void PostUpdate(this List<Component> components, GameTime gameTime)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnPostUpdate(gameTime);
            }
        }

        public static void Draw(this List<Component> components, Renderer renderer, GameTime gameTime)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnDraw(renderer, gameTime);
            }
        }

        public static void End(this List<Component> components)
        {
            foreach (var component in components.ToArray())
            {
                component.OnEnd();
            }
        }
    }
}
