using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Extensions
{
    internal static class EntityListExtensions
    {
        public static IEnumerator Load(this List<Entity> entities, ContentManagerExtended content)
        {
            var currentAssetCount = content.LoadedAssetCount;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnLoad(content);

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
                entities[i].OnSpawn();
            }
        }

        public static void Start(this List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnStart();
            }
        }

        public static void Update(this List<Entity> entities, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnUpdate(gameTime);
            }
        }

        public static void PostUpdate(this List<Entity> entities, GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].OnPostUpdate(gameTime);
            }
        }

        public static void End(this List<Entity> entities)
        {
            foreach (var entity in entities.ToArray())
            {
                entity.OnEnd();
            }
        }
    }
}
