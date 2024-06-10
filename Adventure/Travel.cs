using Microsoft.Xna.Framework;

namespace Adventure
{
    public sealed class Travel : AdventureStateBase
    {
        public const int TimeTickMilliseconds = 1000;

        public Trip Trip { get; }
        public int Timer { get; set; }

        public override AdventureState Type => AdventureState.Travel;

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
