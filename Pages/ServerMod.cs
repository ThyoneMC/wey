using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class ServerMod : IPageGroup
    {
        private readonly HostData HostData;

        public ServerMod(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "mod";
        }

        public override string GetDescription()
        {
            return "server modification";
        }

        public override IEnumerable<IPage> GetPages()
        {
            return new IPage[] {
                    new ServerModModrinth(HostData)
            };
        }
    }
}
