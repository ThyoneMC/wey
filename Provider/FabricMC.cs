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
    class FabricMC : ProviderBase
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

        //#class

        public override ProviderBaseDownload GetServerJar(string TargetGameVersion)
        {
            //version
            FabricMC.GameVersions[] GameVersions = FabricMC.GetGameVersions();

            if (TargetGameVersion == Vanilla.VersionType.Release)
            {
                foreach (FabricMC.GameVersions Version in GameVersions)
                {
                    // first that are stable
                    if (Version.IsStable)
                    {
                        TargetGameVersion = Version.Version;
                        break;
                    }
                }
            }
            else if (TargetGameVersion == Vanilla.VersionType.Snapshot)
            {
                foreach (FabricMC.GameVersions Version in GameVersions)
                {
                    if (!Version.IsStable)
                    {
                        TargetGameVersion = Version.Version;
                        break;
                    }
                }
            }

            //server
            FabricMC.Loaders[] ServerLoader = FabricMC.GetLoaders(TargetGameVersion);
            if (ServerLoader.Length == 0) throw new ArgumentException("game version not found");

            string TargetLoader = ServerLoader[0].Loader.Version;

            FabricMC.Installer[] ServerInstaller = FabricMC.GetInstaller();
            string TargetInstaller = ServerInstaller[0].Version;

            //download
            return GetServerJar(new string[] { TargetGameVersion, TargetLoader, TargetInstaller });
        }

        public override ProviderBaseDownload GetServerJar(string[] buildInfo)
        {
            // [version, loader, installer]

            return new ProviderBaseDownload()
            {
                BuildInfo = buildInfo,
                ServerJar = FabricMC.Download(gameVersion: buildInfo[0], loaderVersion: buildInfo[1], installerVersion: buildInfo[2])
            };
        }
    }
}
