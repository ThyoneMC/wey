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
    class HostCreate : IPageCommand
    {
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

            IProviderDownload download;
            switch (providerName)
            {
                case "vanilla":
                    {
                        Vanilla provider = new();

                        string[] versionList = provider.GetServerVersions(versionFilter);
                        if (!versionList.Any()) throw new VersionNotFoundException(versionFilter);

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
                case "paper":
                    {
                        PaperMC provider = new();

                        string[] versionList = provider.GetServerVersions(versionFilter);
                        if (!versionList.Any()) throw new VersionNotFoundException(versionFilter);

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
                case "fabric":
                    {
                        FabricMC provider = new();

                        string[] versionList = provider.GetServerVersions(versionFilter);
                        if (!versionList.Any()) throw new VersionNotFoundException(versionFilter);

                        string version = Input.SelectionString(versionList, "version").Value;

                        download = provider.GetServerJar(version);

                        break;
                    }
                default:
                    {
                        throw new VersionNotFoundException("provider not found");
                    }
            }

            HostManager host = new(new HostData(name, providerName, path));
            host.Create();
            host.AddServerFile(download);
        }
    }
}
