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
    class PaperMC : ProviderBase
    {
        private static readonly Rest Client = new("https://api.papermc.io/v2/");

        // project

        private static string TargetProject = "paper";

        public class Project
        {
            [JsonPropertyName("versions")]
            public string[] Versions { get; set; }
        }

        public static Project GetProject()
        {
            return Client.Get<Project>($"projects/{TargetProject}");
        }

        // build

        public class BuildDownloadApplicationData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class BuildDownloadData
        {
            [JsonPropertyName("application")]
            public BuildDownloadApplicationData Application { get; set; }
        }

        public class BuildData
        {
            [JsonPropertyName("build")]
            public int ID { get; set; }

            [JsonPropertyName("time")]
            public DateTime ReleaseDate { get; set; }

            [JsonPropertyName("channel")]
            public string Type { get; set; }

            [JsonPropertyName("downloads")]
            public BuildDownloadData Download { get; set; }
        }

        public class Build
        {
            [JsonPropertyName("builds")]
            public BuildData[] Builds { get; set; }
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

        public override ProviderBaseDownload GetServerJar(string TargetGameVersion)
        {
            //version
            PaperMC.Project GameVersions = PaperMC.GetProject();

            if (TargetGameVersion == Vanilla.VersionType.Release) TargetGameVersion = GameVersions.Versions[GameVersions.Versions.Length - 1];
            else if (TargetGameVersion == Vanilla.VersionType.Snapshot) throw new ArgumentException("paper did not have a snapshot version");

            //build
            PaperMC.Build ServerBuild = PaperMC.GetBuilds(TargetGameVersion);
            if (ServerBuild.Builds == null) throw new ArgumentException("game version not found");

            PaperMC.BuildData LastestBuild = ServerBuild.Builds[ServerBuild.Builds.Length - 1];
            string TargetBuild = LastestBuild.ID.ToString();
            string TargetDownload = LastestBuild.Download.Application.Name;

            //download
            return GetServerJar(new string[] { TargetGameVersion, TargetBuild, TargetDownload });
        }

        public override ProviderBaseDownload GetServerJar(string[] buildInfo)
        {
            // [version, build, download]

            return new ProviderBaseDownload()
            {
                BuildInfo = buildInfo,
                ServerJar = PaperMC.Download(version: buildInfo[0], build: buildInfo[1], download: buildInfo[2])
            };
        }
    }
}
