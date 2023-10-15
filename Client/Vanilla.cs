using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.Client
{
    class Vanilla
    {
        // versions

        public static class VersionType
        {
            public static string Release = "release";
            public static string Snapshot = "snapshot";
        }

        public class VersionLatest
        {
            [JsonPropertyName("release")]
            public string Release { get; set; }

            [JsonPropertyName("snapshot")]
            public string Snapshot { get; set; }
        }

        public class VersionData
        {
            [JsonPropertyName("id")]
            public string ID { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("url")]
            public string URL { get; set; }

            [JsonPropertyName("releaseTime")]
            public DateTime ReleaseDate { get; set; }
        }

        public class Version
        {
            [JsonPropertyName("latest")]
            public VersionLatest LatestVersion { get; set; }

            [JsonPropertyName("versions")]
            public VersionData[] Versions { get; set; }
        }

        public static Version GetVersions()
        {
            return Rest.StaticGet<Version>($"https://launchermeta.mojang.com/mc/game/version_manifest.json");
        }

        // version

        public class VersionMetaDownloadData
        {
            [JsonPropertyName("url")]
            public string URL { get; set; }
        }

        public class VersionMetaDownload
        {
            [JsonPropertyName("server")]
            public VersionMetaDownloadData Server { get; set; }
        }

        public class VersionMeta
        {
            [JsonPropertyName("downloads")]
            public VersionMetaDownload Downloads { get; set; }
        }
    }
}
