using Microsoft.Xna.Framework.Content;
using Reaper.Engine;
using System;

namespace Reaper.Objects.Common
{
    public class OnLoad : Behavior
    {
        private readonly Action _onLoad;

        public OnLoad(WorldObject owner, Action onLoad) : base(owner)
        {
            _onLoad = onLoad;
        }

        public override void Load(ContentManager contentManager)
        {
            _onLoad?.Invoke();
        }
    }
}
