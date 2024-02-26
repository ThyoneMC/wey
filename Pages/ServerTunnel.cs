using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class ServerTunnel : IPageGroup
    {
        private readonly HostData HostData;

        public ServerTunnel(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "tunnel";
        }

        public override string GetDescription()
        {
            return "tunnel";
        }

        public override IEnumerable<IPage> GetPages()
        {
            return new IPage[] {
                    new ServerTunnelNgrok(HostData),
                    new ServerTunnelHamachi(HostData)
            };
        }
    }
}
