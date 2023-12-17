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
    class SpecificServerDelete : IPage
    {
        public SpecificServerDelete(HostData host) : base(host.Name)
        {

        }

        public override string GetName()
        {
            return "delete";
        }

        public override string GetDescription()
        {
            return "delete the server";
        }

        public override void OnLoad()
        {
            Logger.WriteSingle("START --> Del");
        }

        public override void OnViewing()
        {
            Logger.WriteSingle("VIEW --> Del");
        }
    }
}
