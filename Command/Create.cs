using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Client;
using wey.Console;
using wey.Core;
using wey.Tool;
using static wey.Client.FabricMC;

namespace wey.Command
{
    class Create: SubCommand
    {
        public override string GetName()
        {
            return "create";
        }

        public override string GetDescription()
        {
            return "create new server";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[]
            {
                new CreateServerSyntax(),
                new CreateVersionSyntax()
            };
        }

        public override string[] GetFlags()
        {
            return new string[]
            {
                "eula",
                "snapshot?",
                "version"
            };
        }

        public override async Task Execute(string[] args, string[] flags)
        {   
            if (GetFlagContent(flags, "eula").ToLower() != "true" && !Input.ReadBoolean("Do you accept to Minecraft End User License Agreement?")) return;

            bool IsSnapshot = GetFlagUsed(flags, "snapshot");
            bool IsVersionSelected = GetFlagUsed(flags, "version");

            string ServerType;
            if (args.Length > 0)
            {
                ServerType = args[0];
            }
            else
            {
                if (IsSnapshot)
                {
                    ServerType = Choice.Start("vanilla", "fabric");
                } else
                {
                    ServerType = Choice.Start("vanilla", "paper", "fabric");
                }
            }

            switch (ServerType)
            {
                case "vanilla":
                    {
                        //version
                        Vanilla.Version? GameVersions = await Vanilla.GetVersions();
                        if (GameVersions == null) return;

                        string TargetGameVersion = GameVersions.LatestVersion.Release;
                        if (IsSnapshot) TargetGameVersion = GameVersions.LatestVersion.Snapshot;
                        if (IsVersionSelected) TargetGameVersion = GetFlagContent(flags, "version");

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
                        if (string.IsNullOrEmpty(URL)) return;

                        //download
                        Vanilla.VersionMeta? VersionMeta = await Rest.StaticGet<Vanilla.VersionMeta>(URL);
                        if (VersionMeta == null) return;

                        byte[]? ServerFile = await Rest.StaticDownload(VersionMeta.Downloads.Server.URL);
                        if (ServerFile == null) return;

                        FileController.StaticBuildByte(
                                Path.Combine(
                                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                        "Downloads",
                                        "server.jar"
                                    ),
                                ServerFile
                            );

                        break;
                    }
                case "paper":
                    {
                        string TargetProject = "paper";

                        //version
                        PaperMC.Project? GameVersions = await PaperMC.GetProject(TargetProject);
                        if (GameVersions == null) return;

                        string TargetGameVersion = GameVersions.Versions[GameVersions.Versions.Length - 1];
                        if (IsVersionSelected) TargetGameVersion = GetFlagContent(flags, "version");

                        //build
                        PaperMC.Build? ServerBuild = await PaperMC.GetBuilds(TargetProject, TargetGameVersion);
                        if (ServerBuild == null) return;

                        PaperMC.BuildData LastestBuild = ServerBuild.Builds[ServerBuild.Builds.Length - 1];
                        string TargetBuild = LastestBuild.ID.ToString();
                        string TargetDownload = LastestBuild.Download.Application.Name;

                        //download
                        byte[]? ServerFile = await PaperMC.Download(TargetProject, TargetGameVersion, TargetBuild, TargetDownload);
                        if (ServerFile == null) return;

                        FileController.StaticBuildByte(
                                Path.Combine(
                                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                        "Downloads",
                                        TargetDownload
                                    ),
                                ServerFile
                            );

                        break;
                    }
                case "fabric":
                    {
                        //version
                        FabricMC.GameVersions[]? GameVersions = await FabricMC.GetGameVersions();
                        if (GameVersions == null) return;

                        string TargetGameVersion = string.Empty;
                        if (IsVersionSelected)
                        {
                            TargetGameVersion = GetFlagContent(flags, "version");
                        }
                        else
                        {
                            foreach (FabricMC.GameVersions Version in GameVersions)
                            {
                                if (IsSnapshot && !Version.IsStable)
                                {
                                    TargetGameVersion = Version.Version;
                                    break;
                                }

                                if (!IsSnapshot && Version.IsStable)
                                {
                                    TargetGameVersion = Version.Version;
                                    break;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(TargetGameVersion)) return;

                        //server
                        FabricMC.Loaders[]? ServerLoader = await FabricMC.GetLoaders(TargetGameVersion);
                        if (ServerLoader == null) return;

                        string TargetLoader = ServerLoader[0].Loader.Version;

                        FabricMC.Installer[]? ServerInstaller = await FabricMC.GetInstaller();
                        if (ServerInstaller == null) return;

                        string TargetInstaller = ServerInstaller[0].Version;

                        //download
                        byte[]? ServerFile = await FabricMC.Download(TargetGameVersion, TargetLoader, TargetInstaller);
                        if (ServerFile == null) return;

                        FileController.StaticBuildByte(
                                Path.Combine(
                                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                        "Downloads",
                                        $"fabric-server-mc.{TargetGameVersion}-loader.{TargetLoader}-launcher.{TargetInstaller}.jar"
                                    ),
                                ServerFile
                            );

                        break;
                    }
                default:
                    {
                        System.Console.WriteLine("Not Found");
                        return;
                    }
            }
        }
    }
}
