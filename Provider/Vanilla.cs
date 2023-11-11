using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Provider
{
    class Vanilla : ProviderBase
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

        //#class

        public override ProviderBaseDownload GetServerJar(string TargetGameVersion)
        {
            //version
            Vanilla.Version GameVersions = Vanilla.GetVersions();

            if (TargetGameVersion == Vanilla.VersionType.Release) TargetGameVersion = GameVersions.LatestVersion.Release;
            else if (TargetGameVersion == Vanilla.VersionType.Snapshot) TargetGameVersion = GameVersions.LatestVersion.Snapshot;

            //server
            string URL = string.Empty;
            foreach (Vanilla.VersionData Version in GameVersions.Versions)
            {
                if (Version.ID == TargetGameVersion)
                {
                    URL = Version.URL;
                    break;
                }
            }
            if (string.IsNullOrEmpty(URL)) throw new ArgumentException("game version not found");

            //download
            Vanilla.VersionMeta VersionMeta = Rest.StaticGet<Vanilla.VersionMeta>(URL);

            return GetServerJar(new string[] { TargetGameVersion, VersionMeta.Downloads.Server.URL });
        }

        public override ProviderBaseDownload GetServerJar(string[] buildInfo)
        {
            // [version, downloadURL]

            return new ProviderBaseDownload()
            {
                BuildInfo = buildInfo,
                ServerJar = Rest.StaticDownload(buildInfo[1]),
            };
        }
    }
}
