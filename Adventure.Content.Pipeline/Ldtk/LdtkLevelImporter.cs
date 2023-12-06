using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adventure.Content.Pipeline.Ldtk
{
    /// <summary>
    /// Loads a .ldtkl file from json format.
    /// </summary>
    [ContentImporter(".ldtkl", DisplayName = "LDTK Level Importer", DefaultProcessor = nameof(LdtkLevelProcessor))]
    public class LdtkLevelImporter : ContentImporter<LdtkLevel>
    {
        public override LdtkLevel Import(string filename, ContentImporterContext context)
        {
            using var sr = new StreamReader(filename);
            return JsonSerializer.Deserialize<LdtkLevel>(sr.ReadToEnd());
        }
    }
}