using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Global;
using wey.Interface;

namespace wey.Host.Provider
{
    class VanillaBuild : IProviderBuild
    {
        public string DownloadURL { get; set; } = string.Empty;

        public VanillaBuild(string version, string downloadURL) : base(version)
        {
            DownloadURL = downloadURL;
        }
    }

    class Vanilla : IProvider<VanillaBuild>
    {
        // versions

        public static class VersionType
        {
            public readonly static string Release = "release";
            public readonly static string Snapshot = "snapshot";

            public static string FromString(string version)
            {
                version = version.ToLower();
                if (version == VersionType.Release || version.StartsWith("last"))
                {
                    return VersionType.Release;
                }
                else if (version == VersionType.Snapshot || version == "alpha" || version == "beta")
                {
                    return VersionType.Snapshot;
                }

                return version;
            }
        }

        public class VersionLatest
        {
            [JsonPropertyName("release")]
            public string Release { get; set; } = string.Empty;

            [JsonPropertyName("snapshot")]
            public string Snapshot { get; set; } = string.Empty;
        }

        public class VersionData
        {
            [JsonPropertyName("id")]
            public string ID { get; set; } = string.Empty;

            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("url")]
            public string URL { get; set; } = string.Empty;

            [JsonPropertyName("releaseTime")]
            public DateTime ReleaseDate { get; set; } = new();
        }

        public class Version
        {
            [JsonPropertyName("latest")]
            public VersionLatest LatestVersion { get; set; } = new();

            [JsonPropertyName("versions")]
            public VersionData[] Versions { get; set; } = Array.Empty<VersionData>();
        }

        public static Version GetVersions()
        {
            return Rest.StaticGet<Version>($"https://launchermeta.mojang.com/mc/game/version_manifest.json");
        }

        // version

        public class VersionMetaDownloadData
        {
            [JsonPropertyName("url")]
            public string URL { get; set; } = string.Empty;
        }

        public class VersionMetaDownload
        {
            [JsonPropertyName("server")]
            public VersionMetaDownloadData Server { get; set; } = new();
        }

        public class VersionMeta
        {
            [JsonPropertyName("downloads")]
            public VersionMetaDownload Downloads { get; set; } = new();
        }

        //#class

        public override bool IsMod()
        {
            return false;
        }

        public override IProviderDownload GetServerJar(string TargetGameVersion)
        {
            //version
            Vanilla.Version GameVersions = Vanilla.GetVersions();

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
            if (string.IsNullOrEmpty(URL)) throw new VersionNotFoundException();

            //download
            Vanilla.VersionMeta VersionMeta = Rest.StaticGet<Vanilla.VersionMeta>(URL);

            return GetServerJar(new VanillaBuild(TargetGameVersion, VersionMeta.Downloads.Server.URL));
        }

        public override IProviderDownload GetServerJar(VanillaBuild build)
        {
            return new()
            {
                Build = JsonEncryption.Encrypt(build),
                ServerJar = Rest.StaticDownload(build.DownloadURL),
            };
        }
    }
}
