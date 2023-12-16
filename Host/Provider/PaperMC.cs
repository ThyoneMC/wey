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
    class PaperMCBuild : IProviderBuild
    {
        public string Build { get; set; }
        public string Download { get; set; }

        public PaperMCBuild(string version, string build, string download) : base(version)
        {
            Build = build;
            Download = download;
        }
    }

    class PaperMC : IProvider<PaperMCBuild>
    {
        private static readonly Rest Client = new("https://api.papermc.io/v2/");

        // project

        private static readonly string TargetProject = "paper";

        public class Project
        {
            public string[] Versions { get; set; } = Array.Empty<string>();
        }

        public static Project GetProject()
        {
            return Client.Get<Project>($"projects/{TargetProject}");
        }

        // build

        public class BuildDownloadApplicationData
        {
            public string Name { get; set; } = string.Empty;
        }

        public class BuildDownloadData
        {
            public BuildDownloadApplicationData Application { get; set; } = new();
        }

        public class BuildData
        {
            public int ID { get; set; } = -1;
            public DateTime ReleaseDate { get; set; } = new();
            public string Type { get; set; } = string.Empty;
            public BuildDownloadData Download { get; set; } = new();
        }

        public class Build
        {
            public BuildData[] Builds { get; set; } = Array.Empty<BuildData>();
        }

        public static Build GetBuilds(string version)
        {
            return Client.Get<Build>($"projects/{TargetProject}/versions/{version}/builds");
        }

        // download

        public static byte[] Download(string version, string build, string download)
        {
            return Client.Download($"projects/{TargetProject}/versions/{version}/builds/{build}/downloads/{download}");
        }

        //#class

        public override bool IsMod()
        {
            return false;
        }

        public override IProviderDownload<PaperMCBuild> GetServerJar(string TargetGameVersion)
        {
            //version
            PaperMC.Project GameVersions = PaperMC.GetProject();

            TargetGameVersion = Vanilla.VersionType.FromString(TargetGameVersion);
            if (TargetGameVersion == Vanilla.VersionType.Release)
            {
                TargetGameVersion = GameVersions.Versions[GameVersions.Versions.Length - 1];
            }
            else if (TargetGameVersion == Vanilla.VersionType.Snapshot)
            {
                throw new VersionNotFoundException("paper did not have a snapshot version");
            }

            //build
            PaperMC.Build ServerBuild = PaperMC.GetBuilds(TargetGameVersion);
            if (ServerBuild.Builds == null) throw new VersionNotFoundException();

            PaperMC.BuildData LastestBuild = ServerBuild.Builds[ServerBuild.Builds.Length - 1];
            string TargetBuild = LastestBuild.ID.ToString();
            string TargetDownload = LastestBuild.Download.Application.Name;

            //download
            return GetServerJar(new PaperMCBuild(TargetGameVersion, TargetBuild, TargetDownload));
        }

        public override IProviderDownload<PaperMCBuild> GetServerJar(PaperMCBuild build)
        {
            return new()
            {
                Build = build,
                ServerJar = PaperMC.Download(build.Version, build.Build,build.Download)
            };
        }
    }
}
