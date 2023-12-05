using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adventure.Content.Pipeline
{
    [ContentImporter(".ldtkl", DisplayName = "LDTK Level", DefaultProcessor = nameof(LdtkLevelProcessor))]
    public class LdtkLevelImporter : ContentImporter<LdtkLevel>
    {
        public override LdtkLevel Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing LDTK Level file: {0}", filename);
            using var sr = new StreamReader(filename);
            return JsonSerializer.Deserialize<LdtkLevel>(sr.ReadToEnd());
        }
    }
}