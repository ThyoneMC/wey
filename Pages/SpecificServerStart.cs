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

            Panel.Start();
            Panel.SetStatus($"[{Host.Data.Provider}] {Host.Data.Name}");
        }

        public override void OnViewing()
        {
            if (Host == null || Host.Process == null) return;

            if (Host.Process.IsStarted && !Host.Process.IsExists())
            {
                IsExit = true;

                Panel.Stop();

                return;
            }

            string? output = Host.Process.GetOnceOutput();
            if (output != null) Panel.AddCanvas(output);

            string? input = Panel.GetInput();
            if (input != null) Host.Process.Input(input);
        }
    }
}
