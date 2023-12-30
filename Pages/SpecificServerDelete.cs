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
    class SpecificServerDelete : IPageCommand
    {
        private readonly HostData HostData;

        public SpecificServerDelete(HostData host) : base(host.Name)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "delete";
        }

        public override string GetDescription()
        {
            return "delete server";
        }

        public override void OnCommand()
        {
            if (!Input.ReadBoolean($"Are you sure to delete {HostData.Name}?")) return;

            HostManager Host = new(HostData);

            if (Host.Process != null) Host.Process.Kill();
            Host.Delete();
        }
    }
}
