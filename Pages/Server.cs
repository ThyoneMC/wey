using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Interface;
using wey.Host;
using wey.Console;
using System.Text.Json;

namespace wey.Pages
{
    class Server : IPageGroup
    {
        private readonly HostData HostData;

        public Server(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return HostData.Name;
        }

        public override string GetDescription()
        {
            return "server management";
        }

        public override IEnumerable<IPage> GetPages()
        {
            if (HostFinder.Find(HostData.Name).Length == 0) return Array.Empty<IPage>(); //deleted server

            return new IPage[] {
                    new ServerInfo(HostData),
                    new ServerConsole(HostData),
                    new ServerStart(HostData),
                    new ServerStop(HostData),
                    new ServerUpdate(HostData),
                    new ServerTunnel(HostData),
                    new ServerDelete(HostData)
            };
        }
    }
}
