using System;
using Microsoft.Xna.Framework.Content;

namespace Engine
{
    /// <summary>
    /// Custom content manager with extended functions.
    /// </summary>
    public sealed class ContentManagerExtended : ContentManager
    {
        public ContentManagerExtended(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
        }

        /// <summary>
        /// Loads content without caching it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadWithoutCaching<T>(string assetName) 
        {
            return ReadAsset<T>(assetName, null);
        }
    }
}
