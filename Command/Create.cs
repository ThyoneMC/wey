using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Client;
using wey.Console;
using wey.Core;
using wey.Server;
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
                new CreateProviderSyntax(),
                new CreateVersionSyntax(),
                new CreateNameSyntax(),
            };
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new CreateEulaFlag()
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {   
            if (!SubCommandFlag.GetUsed(flags, "eula") && !Input.ReadBoolean("Do you accept to Minecraft End User License Agreement?")) return;

            string Server_Provider = args[0];
            string Server_Version = args[1];
            string Server_Name = args[2];
            string Server_Path = Path.Join(Directory.GetCurrentDirectory(), Server_Name);

            if (Server_Version.StartsWith("last")) Server_Version = Vanilla.VersionType.Release;

            switch (Server_Provider)
            {
                case ServerProvider.Vanilla:
                    {
                        //version
                        Vanilla.Version GameVersions = Vanilla.GetVersions();

                        string TargetGameVersion = Server_Version;
                        if (Server_Version == Vanilla.VersionType.Release) TargetGameVersion = GameVersions.LatestVersion.Release;
                        else if (Server_Version == Vanilla.VersionType.Snapshot) TargetGameVersion = GameVersions.LatestVersion.Snapshot;

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
                        if (string.IsNullOrEmpty(URL))
                        {
                            Logger.Error("game version not found");
                            return;
                        }

                        //download
                        Vanilla.VersionMeta VersionMeta = Rest.StaticGet<Vanilla.VersionMeta>(URL);

                        byte[] ServerFile = Rest.StaticDownload(VersionMeta.Downloads.Server.URL);

                        ServerManager server = new(
                                new ServerData(
                                        Server_Name, 
                                        ServerProvider.Vanilla, 
                                        new string[] { TargetGameVersion },
                                        Server_Path
                                    )
                            );
                        server.AddServerFile(ServerFile);

                        break;
                    }
                case ServerProvider.PaperMC:
                    {
                        string TargetProject = "paper";

                        //version
                        PaperMC.Project GameVersions = PaperMC.GetProject(TargetProject);

                        string TargetGameVersion = Server_Version;
                        if (Server_Version == Vanilla.VersionType.Release) TargetGameVersion = GameVersions.Versions[^1];
                        else if (Server_Version == Vanilla.VersionType.Snapshot)
                        {
                            Logger.Error("paper did not have a snapshot version");
                            return;
                        }

                        //build
                        PaperMC.Build ServerBuild = PaperMC.GetBuilds(TargetProject, TargetGameVersion);
                        if (ServerBuild.Builds == null)
                        {
                            Logger.Error("game version not found");
                            return;
                        }
                        
                        PaperMC.BuildData LastestBuild = ServerBuild.Builds[^1];
                        string TargetBuild = LastestBuild.ID.ToString();
                        string TargetDownload = LastestBuild.Download.Application.Name;

                        //download
                        byte[] ServerFile = PaperMC.Download(TargetProject, TargetGameVersion, TargetBuild, TargetDownload);

                        ServerManager server = new(
                                new ServerData(
                                        Server_Name,
                                        ServerProvider.PaperMC,
                                        new string[] { Server_Version, TargetGameVersion, TargetBuild, TargetDownload },
                                        Server_Path
                                    )
                            );
                        server.AddServerFile(ServerFile);

                        break;
                    }
                case ServerProvider.FabricMC:
                    {
                        //version
                        FabricMC.GameVersions[] GameVersions = FabricMC.GetGameVersions();

                        string TargetGameVersion = Server_Version;
                        if (Server_Version == Vanilla.VersionType.Release)
                        {
                            foreach (FabricMC.GameVersions Version in GameVersions)
                            {
                                if (Version.IsStable)
                                {
                                    TargetGameVersion = Version.Version;
                                    break;
                                }
                            }
                        }
                        else if (Server_Version == Vanilla.VersionType.Snapshot)
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
                        if (ServerLoader.Length == 0)
                        {
                            Logger.Error("game version not found");
                            return;
                        }

                        string TargetLoader = ServerLoader[0].Loader.Version;

                        FabricMC.Installer[] ServerInstaller = FabricMC.GetInstaller();
                        string TargetInstaller = ServerInstaller[0].Version;

                        //download
                        byte[] ServerFile = FabricMC.Download(TargetGameVersion, TargetLoader, TargetInstaller);

                        ServerManager server = new(
                                new ServerData(
                                        Server_Name,
                                        ServerProvider.FabricMC,
                                        new string[] { Server_Version, TargetLoader, TargetInstaller },
                                        Server_Path
                                    )
                            );
                        server.AddServerFile(ServerFile);

                        break;
                    }
                default:
                    {
                        Logger.Error("Provider Not Found");
                        return;
                    }
            }
        }
    }
}
