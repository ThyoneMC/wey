using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API
{
    public class ModHandlerFileHash
    {
        [JsonPropertyName("algorithm")]
        public required string Algorithm { get; set; }

        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }

    public class ModHandlerFile
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("provider")]
        public required ModHandlerProvider Provider { get; set; }

        [JsonPropertyName("id")]
        public required string ID { get; set; }

        [JsonPropertyName("fileName")]
        public required string FileName { get; set; }

        [JsonPropertyName("fileId")]
        public string? FileID { get; set; } = null;

        [JsonPropertyName("hash")]
        public required ModHandlerFileHash Hash { get; set; }

        [JsonPropertyName("url")]
        public required string URL { get; set; }

        [JsonPropertyName("clientSide")]
        public required bool ClientSide { get; set; }

        [JsonPropertyName("serverSide")]
        public required bool ServerSide { get; set; }

        [JsonPropertyName("dependencies")]
        public ModHandlerFile[] Dependencies { get; set; } = Array.Empty<ModHandlerFile>();

        [JsonPropertyName("incompatible")]
        public ModHandlerFile[] Incompatibles { get; set; } = Array.Empty<ModHandlerFile>();
    }

    public abstract class ModHandler
    {
        protected string gameVersion;

        protected ModHandler(string gameVersion)
        {
            this.gameVersion = gameVersion;
        }

        public abstract ModHandlerFile Get(string id);

        public abstract ModHandlerFile[] Update(ModHandlerFile[] ids);
    }
}
