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
    class SpecificServerUpdate : IPageGroup
    {
        private readonly HostData HostData;

        public SpecificServerUpdate(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "update";
        }

        public override string GetDescription()
        {
            return "server update";
        }

        public override IEnumerable<IPage> GetPages()
        {
            if (!Input.ReadBoolean($"Are you sure to update {HostData.Name}?", clear: true)) return Array.Empty<IPage>();

            return new IPage[] {
                    new SpecificServerUpdateBuild(HostData),
                    new SpecificServerUpdateVersion(HostData),
            };
        }
    }
}
