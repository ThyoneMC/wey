using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class SpecificServerStop : IPageCommand
    {
        private HostData Host;

        public SpecificServerStop(HostData host) : base(host.Name)
        {
            Host = host;
        }

        public override string GetName()
        {
            return "stop";
        }

        public override string GetDescription()
        {
            return "stop the server";
        }

        public override void OnCommand()
        {
            HostManager host = new(Host);

            host.Start();
        }
    }
}
