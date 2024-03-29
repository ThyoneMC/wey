﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Interface;

namespace wey.Host.Provider
{
    class PaperMCBuild : IProviderBuild
    {
        public int Build { get; set; } = -1;
        public string Download { get; set; } = string.Empty;

        public PaperMCBuild(string version, int build, string download) : base(version)
        {
            HasPlugin = true;

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
            [JsonPropertyName("versions")]
            public string[] Versions { get; set; } = Array.Empty<string>();
        }

        public static Project GetProject()
        {
            return Client.Get<Project>($"projects/{TargetProject}");
        }

        // build

        public class BuildDownloadApplicationData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }

        public class BuildDownloadData
        {
            [JsonPropertyName("application")]
            public BuildDownloadApplicationData Application { get; set; } = new();
        }

        public class BuildData
        {
            [JsonPropertyName("build")]
            public int ID { get; set; }

            [JsonPropertyName("time")]
            public DateTime ReleaseDate { get; set; } = new();

            [JsonPropertyName("channel")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("downloads")]
            public BuildDownloadData Download { get; set; } = new();
        }

        public class Build
        {
            [JsonPropertyName("builds")]
            public BuildData[] Builds { get; set; } = Array.Empty<BuildData>();
        }

        public static Build GetBuilds(string version)
        {
            return Client.Get<Build>($"projects/{TargetProject}/versions/{version}/builds");
        }

        // download

        public static byte[] Download(string version, int build, string download)
        {
            return Client.Download($"projects/{TargetProject}/versions/{version}/builds/{build}/downloads/{download}");
        }

        //#class

        public override string[] GetServerVersions(string filter = "")
        {
            filter = Vanilla.VersionType.FromString(filter);

            IEnumerable<string> versionList = PaperMC.GetProject().Versions.Reverse();

            if (filter == Vanilla.VersionType.Release)
            {
                return versionList.ToArray();
            }
            else if (filter == Vanilla.VersionType.Snapshot)
            {
                return Array.Empty<string>();
            }
            else
            {
                return versionList.Where(v => v.Contains(filter)).ToArray();
            }
        }

        public override IProviderDownload GetServerJar(string TargetGameVersion)
        {
            //build
            PaperMC.Build ServerBuild = PaperMC.GetBuilds(TargetGameVersion);
            if (ServerBuild.Builds == null) throw new VersionNotFoundException();

            PaperMC.BuildData LastestBuild = ServerBuild.Builds[ServerBuild.Builds.Length - 1];
            int TargetBuild = LastestBuild.ID;
            string TargetDownload = LastestBuild.Download.Application.Name;

            //download
            return GetServerJar(new PaperMCBuild(TargetGameVersion, TargetBuild, TargetDownload));
        }

        public override IProviderDownload GetServerJar(PaperMCBuild build)
        {
            return new()
            {
                Build = JsonEncryption.Encrypt(build),
                ServerJar = PaperMC.Download(build.Version, build.Build,build.Download)
            };
        }
    }
}
