using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;

namespace wey.Pages
{
    class HostList : IPageGroup
    {
        public override string GetName()
        {
            return "list";
        }

        public override string GetDescription()
        {
            return "servers list";
        }

        public override IEnumerable<IPage> GetPages()
        {
            HostData[] hostList = HostFinder.Find();

            return hostList.Select(host => new Server(host));
        }
    }
}
