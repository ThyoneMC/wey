using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API
{
    public class ModHelperFileHash
    {
        [JsonPropertyName("algorithm")]
        public required string Algorithm { get; set; }

        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }

    public enum ModHelperProvider
    {
        Modrinth,
        CurseForge
    }

    public class ModHelperFile
    {
        [JsonPropertyName("provider")]
        public required ModHelperProvider Provider { get; set; }

        [JsonPropertyName("id")]
        public required string ID { get; set; }

        [JsonPropertyName("fileName")]
        public required string FileName { get; set; }

        [JsonPropertyName("fileId")]
        public string? FileID { get; set; } = null;

        [JsonPropertyName("hash")]
        public required ModHelperFileHash Hash { get; set; }

        [JsonPropertyName("url")]
        public required string URL { get; set; }

        [JsonPropertyName("clientSide")]
        public required bool ClientSide { get; set; }

        [JsonPropertyName("serverSide")]
        public required bool ServerSide { get; set; }

        [JsonPropertyName("dependencies")]
        public string[] DependencyProjectIDs { get; set; } = Array.Empty<string>();

        [JsonPropertyName("incompatible")]
        public string[] IncompatibleProjectIDs { get; set; } = Array.Empty<string>();
    }

    public abstract class ModHelper
    {
        protected string gameVersion;

        protected ModHelper(string gameVersion)
        {
            this.gameVersion = gameVersion;
        }

        public abstract ModHelperFile Get(string id);

        public abstract ModHelperFile[] Update(ModHelperFile[] ids);
    }
}
