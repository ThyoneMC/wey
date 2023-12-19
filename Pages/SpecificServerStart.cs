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
    class SpecificServerStart : IPage
    {
        private HostData HostData;

        public SpecificServerStart(HostData host) : base(host.Name)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "start";
        }

        public override string GetDescription()
        {
            return "start the server";
        }

        private HostManager? Host = null;

        public override void OnLoad()
        {
            Host = new(HostData);

            Host.Start();

            //string read = KeyReader.ReadLine();
        }

        public override void OnViewing()
        {
            
        }
    }
}
