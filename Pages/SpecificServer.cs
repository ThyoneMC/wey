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
    class SpecificServer : IPageGroup
    {
        private readonly HostData Host;

        public SpecificServer(HostData host) : base(host.Name)
        {
            Host = host;
        }

        public override string GetName()
        {
            return Host.Name;
        }

        public override string GetDescription()
        {
            return "server management";
        }

        public override IEnumerable<IPageBase> GetPages()
        {
            return new IPageBase[] {
                    new SpecificServerStart(Host),
                    new SpecificServerStop(Host),
                    new SpecificServerDelete(Host),
            };
        }
    }
}
