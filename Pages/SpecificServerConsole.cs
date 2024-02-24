using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Interface;
using static System.Net.Mime.MediaTypeNames;

namespace wey.Pages
{
    class SpecificServerConsole : IPageView
    {
        private readonly HostData HostData;

        public SpecificServerConsole(HostData host) : base(host.Name)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "console";
        }

        public override string GetDescription()
        {
            return "server console";
        }

        private HostManager? Host = null;

        public override void OnLoad()
        {
            Host = new(HostData);

            Panel.Start();
            Panel.SetStatus($"[{Host.Data.Provider}] {Host.Data.Name}");

            if (Host.Process == null) return;

            Panel.AddCanvas(Host.Process.GetOutput());
            Host.Process.GetOnceOutput(); //remove latest print
        }

        public override void OnViewing()
        {
            if (
                (Host == null || Host.Process == null) ||
                (Host.Process.IsStarted && !Host.Process.IsExists())
            ) {
                IsExit = true;

                OnForceExit();

                return;
            }

            string? output = Host.Process.GetOnceOutput();
            if (output != null) Panel.AddCanvas(output);

            string? input = Panel.GetInput();
            if (input != null) Host.Process.Input(input);
        }

        public override void OnForceExit()
        {
            Panel.Stop();

            if (Host == null || Host.Process == null) return;

            Host.Process.Export();
        }
    }
}
