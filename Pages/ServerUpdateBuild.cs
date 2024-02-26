using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;

namespace wey.Pages
{
    class ServerUpdateBuild : IPageCommand
    {
        private readonly HostData HostData;

        public ServerUpdateBuild(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "build";
        }

        public override string GetDescription()
        {
            return "update server build";
        }

        public override void OnCommand()
        {
            string version = HostData.GetVersion();

            IProviderDownload download = HostData.Provider switch
            {
                "vanilla" => new Vanilla().GetServerJar(version),
                "paper" => new PaperMC().GetServerJar(version),
                "fabric" => new FabricMC().GetServerJar(version),

                _ => throw new VersionNotFoundException("provider not found"),
            };

            HostManager host = new(HostData);
            host.AddServerFile(download);
        }
    }
}
