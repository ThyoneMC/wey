using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.Client
{
    class FabricMC
    {
        private static readonly Rest Client = new("https://meta.fabricmc.net/v2/versions/");

        // game versions

        public class GameVersions
        {
            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; }
        }

        public static GameVersions[] GetGameVersions()
        {
            return Client.Get<GameVersions[]>($"game");
        }

        // loader versions

        public class LoadersData
        {
            [JsonPropertyName("separator")]
            public string Separator { get; set; }

            [JsonPropertyName("build")]
            public int BuildID { get; set; }

            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; }
        }

        public class Loaders
        {
            [JsonPropertyName("loader")]
            public LoadersData Loader { get; set; }
        }

        public static Loaders[] GetLoaders(string gameVersion)
        {
            return Client.Get<Loaders[]>($"loader/{gameVersion}");
        }

        // installer

        public class Installer
        {
            [JsonPropertyName("url")]
            public string URL { get; set; }

            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; }
        }

        public static Installer[] GetInstaller()
        {
            return Client.Get<Installer[]>($"installer");
        }

        // download

        public static byte[] Download(string gameVersion, string loaderVersion, string installerVersion)
        {
            return Client.Download($"loader/{gameVersion}/{loaderVersion}/{installerVersion}/server/jar");
        }
    }
}
