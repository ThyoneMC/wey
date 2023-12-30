using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;
using static wey.Host.Provider.PaperMC;
using static wey.Host.Provider.Vanilla;

namespace wey.Pages
{
    class ServerCreate : IPageCommand
    {
        public ServerCreate() : base()
        {

        }

        public override string GetName()
        {
            return "create";
        }

        public override string GetDescription()
        {
            return "create the server";
        }

        public override void OnCommand()
        {
            if(!Input.ReadBoolean("Do you accept to Minecraft End User License Agreement?")) return;

            string name = Input.ReadString("server name", true);
            string path = Path.Join(Directory.GetCurrentDirectory(), name);

            string providerName = Input.SelectionString(new string[] { "vanilla", "paper", "fabric" }, "provider name").Value;

            string versionFilter = Vanilla.VersionType.FromString(Input.ReadString("version (filter)", clear: true));

            IProviderDownload download = new();
            switch (providerName)
            {
                case "vanilla":
                    {
                        Vanilla provider = new();

                        Vanilla.Version versions = Vanilla.GetVersions();

                        if (versionFilter == Vanilla.VersionType.Release) versionFilter = versions.LatestVersion.Release;
                        else if (versionFilter == Vanilla.VersionType.Snapshot) versionFilter = versions.LatestVersion.Snapshot;

                        IEnumerable<string> versionList = versions.Versions.Where(v => v.ID.Contains(versionFilter)).Select(v => v.ID);
                        if (!versionList.Any())
                        {
                            throw new VersionNotFoundException(versionFilter);
                        }

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
                case "paper":
                    {
                        PaperMC provider = new();

                        PaperMC.Project project = PaperMC.GetProject();

                        if (versionFilter == Vanilla.VersionType.Release) versionFilter = project.Versions.Last();
                        else if (versionFilter == Vanilla.VersionType.Snapshot)
                        {
                            throw new VersionNotFoundException("snapshot not support");
                        }

                        IEnumerable<string> versionList = project.Versions.Where(v => v.Contains(versionFilter));
                        if (!versionList.Any())
                        {
                            throw new VersionNotFoundException(versionFilter);
                        }

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
                case "fabric":
                    {
                        FabricMC provider = new();

                        FabricMC.GameVersions[] versions = FabricMC.GetGameVersions();

                        if (versionFilter == Vanilla.VersionType.Release) versionFilter = versions.First(v => v.IsStable).Version;
                        else if (versionFilter == Vanilla.VersionType.Snapshot) versionFilter = versions.First(v => !v.IsStable).Version;

                        IEnumerable<string> versionList = versions.Where(v => v.Version.Contains(versionFilter)).Select(v => v.Version);
                        if (!versionList.Any())
                        {
                            throw new VersionNotFoundException(versionFilter);
                        }

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
            }

            HostManager host = new(new HostData(name, providerName, path));
            host.Create();
            host.AddServerFile(download);
        }
    }
}
