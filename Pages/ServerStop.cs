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
    class ServerStop : IPageCommand
    {
        private readonly HostData HostData;

        public ServerStop(HostData host)
        {
            HostData = host;
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
            new HostManager(HostData).Stop();
        }
    }
}
