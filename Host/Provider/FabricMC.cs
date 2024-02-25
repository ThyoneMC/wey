using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Interface;

namespace wey.Host.Provider
{
    class FabricMCBuild : IProviderBuild
    {
        public string Loader { get; set; } = string.Empty;
        public string Installer { get; set; } = string.Empty;

        public FabricMCBuild(string version, string loader, string installer) : base(version)
        {
            HasMod = true;

            Loader = loader;
            Installer = installer;
        }
    }

    class FabricMC : IProvider<FabricMCBuild>
    {
        private static readonly Rest Client = new("https://meta.fabricmc.net/v2/versions/");

        // game versions

        public class GameVersions
        {
            [JsonPropertyName("version")]
            public string Version { get; set; } = string.Empty;

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; } = true;
        }

        public static GameVersions[] GetGameVersions()
        {
            return Client.Get<GameVersions[]>($"game");
        }

        // loader versions

        public class LoadersData
        {
            [JsonPropertyName("separator")]
            public string Separator { get; set; } = string.Empty;

            [JsonPropertyName("build")]
            public int BuildID { get; set; } = -1;

            [JsonPropertyName("version")]
            public string Version { get; set; } = string.Empty;

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; } = true;
        }

        public class Loaders
        {
            [JsonPropertyName("loader")]
            public LoadersData Loader { get; set; } = new();
        }

        public static Loaders[] GetLoaders(string version)
        {
            return Client.Get<Loaders[]>($"loader/{version}");
        }

        // installer

        public class Installer
        {
            [JsonPropertyName("url")]
            public string URL { get; set; } = string.Empty;

            [JsonPropertyName("version")]
            public string Version { get; set; } = string.Empty;

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; } = true;
        }

        public static Installer[] GetInstaller()
        {
            return Client.Get<Installer[]>($"installer");
        }

        // download

        public static byte[] Download(string version, string loader, string installer)
        {
            return Client.Download($"loader/{version}/{loader}/{installer}/server/jar");
        }

        //#class

        public override string[] GetServerVersions(string filter = "")
        {
            filter = Vanilla.VersionType.FromString(filter);

            FabricMC.GameVersions[] versionList = FabricMC.GetGameVersions();

            if (filter == Vanilla.VersionType.Release)
            {
                versionList = Array.FindAll(versionList, v => v.IsStable);
            }
            else if (filter == Vanilla.VersionType.Snapshot)
            {
                versionList = Array.FindAll(versionList, v => !v.IsStable);
            }
            else
            {
                versionList = Array.FindAll(versionList, v => v.Version.Contains(filter));
            }

            return versionList.Select(v => v.Version).ToArray();
        }

        public override IProviderDownload GetServerJar(string TargetGameVersion)
        {
            //server
            FabricMC.Loaders[] ServerLoader = FabricMC.GetLoaders(TargetGameVersion);
            if (ServerLoader.Length == 0) throw new VersionNotFoundException();

            string TargetLoader = ServerLoader[0].Loader.Version;

            FabricMC.Installer[] ServerInstaller = FabricMC.GetInstaller();
            string TargetInstaller = ServerInstaller[0].Version;

            //download
            return GetServerJar(new FabricMCBuild(TargetGameVersion, TargetLoader, TargetInstaller));
        }

        public override IProviderDownload GetServerJar(FabricMCBuild build)
        {
            return new()
            {
                Build = JsonEncryption.Encrypt(build),
                ServerJar = FabricMC.Download(build.Version, build.Loader, build.Installer)
            };
        }
    }
}
