using System;
using Microsoft.Xna.Framework.Content;

namespace Engine
{
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Loads content without caching it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static T LoadWithoutCaching<T>(this ContentManager contentManager, string assetName)
        {
            if (contentManager is ContentManagerExtended engineContentManager)
            {
                return engineContentManager.LoadWithoutCaching<T>(assetName);
            }

            throw new NotImplementedException($"The given content manager must be of type {typeof(ContentManagerExtended)}");
        }
    }
}
