using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;

namespace wey.Pages
{
    class ServerUpdateVersion : IPageCommand
    {
        private readonly HostData HostData;

        public ServerUpdateVersion(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "version";
        }

        public override string GetDescription()
        {
            return "update server version";
        }

        public override void OnCommand()
        {
            string versionFilter = Vanilla.VersionType.FromString(Input.ReadString("version (filter)", clear: true));

            IProviderDownload download;
            switch (HostData.Provider)
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

            HostManager host = new(HostData);
            host.AddServerFile(download);
        }
    }
}
