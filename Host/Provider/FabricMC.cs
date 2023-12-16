using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Global;
using wey.Interface;

namespace wey.Host.Provider
{
    class FabricMCBuild : IProviderBuild
    {
        public string Loader { get; set; }
        public string Installer { get; set; }

        public FabricMCBuild(string version, string loader, string installer) : base(version)
        {
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
            public string Version { get; set; } = string.Empty;
            public bool IsStable { get; set; } = true;
        }

        public static GameVersions[] GetGameVersions()
        {
            return Client.Get<GameVersions[]>($"game");
        }

        // loader versions

        public class LoadersData
        {
            public string Separator { get; set; } = string.Empty;
            public int BuildID { get; set; } = -1;
            public string Version { get; set; } = string.Empty;
            public bool IsStable { get; set; } = true;
        }

        public class Loaders
        {
            public LoadersData Loader { get; set; } = new();
        }

        public static Loaders[] GetLoaders(string version)
        {
            return Client.Get<Loaders[]>($"loader/{version}");
        }

        // installer

        public class Installer
        {
            public string URL { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
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

        public override bool IsMod()
        {
            return true;
        }

        public override IProviderDownload<FabricMCBuild> GetServerJar(string TargetGameVersion)
        {
            //version
            FabricMC.GameVersions[] GameVersions = FabricMC.GetGameVersions();

            TargetGameVersion = Vanilla.VersionType.FromString(TargetGameVersion);
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
            if (ServerLoader.Length == 0) throw new VersionNotFoundException();

            string TargetLoader = ServerLoader[0].Loader.Version;

            FabricMC.Installer[] ServerInstaller = FabricMC.GetInstaller();
            string TargetInstaller = ServerInstaller[0].Version;

            //download
            return GetServerJar(new FabricMCBuild(TargetGameVersion, TargetLoader, TargetInstaller));
        }

        public override IProviderDownload<FabricMCBuild> GetServerJar(FabricMCBuild build)
        {
            return new()
            {
                Build = build,
                ServerJar = FabricMC.Download(build.Version, build.Loader, build.Installer)
            };
        }
    }
}
