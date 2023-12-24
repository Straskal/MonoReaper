using Engine.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Engine.Extensions
{
    internal static class EntityListExtensions
    {
        public static IEnumerator Load(this List<Entity> entities, ContentManagerExtended content)
        {
            var currentAssetCount = content.LoadedAssetCount;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Load(content);

                if (currentAssetCount != content.LoadedAssetCount)
                {
                    currentAssetCount = content.LoadedAssetCount;
                    yield return null;
                }
            }
        }

        public static void Spawn(this List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Spawn();
            }
        }

        public static void Start(this List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Start();
            }
        }

        public static void Update(this List<Entity> entities, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(gameTime);
            }
        }

        public static void PostUpdate(this List<Entity> entities, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PostUpdate(gameTime);
            }
        }

        public static void End(this List<Entity> entities)
        {
            foreach (var entity in entities.ToArray())
            {
                entity.End();
            }
        }

        public static void Draw(this List<Entity> entities, Renderer renderer, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Draw(renderer, gameTime);
            }
        }
    }
}
