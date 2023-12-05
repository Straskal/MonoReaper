using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adventure.Content.Pipeline
{
    [ContentProcessor(DisplayName = "LDTK Level")]
    public class LdtkLevelProcessor : ContentProcessor<LdtkLevel, LdtkLevel>
    {
        public override LdtkLevel Process(LdtkLevel input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing LDTK Level: {0}", input.Identifier);
            return input;
        }
    }
}