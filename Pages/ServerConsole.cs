using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class ServerConsole : IPageView
    {
        private readonly HostData HostData;

        public ServerConsole(HostData host)
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

            Host.Process.ListenOutput(output =>
            {
                Panel.AddCanvas(output);
            });
        }

        public override void OnViewing()
        {
            if (
                Host == null || Host.Process == null ||
                Host.Process.IsStarted && !Host.Process.IsExists()
            )
            {
                IsExit = true;

                OnExit();

                return;
            }

            string? input = Panel.GetInput();
            if (input != null) Host.Process.Input(input);
        }

        public override void OnExit()
        {
            Panel.Stop();

            if (Host == null || Host.Process == null) return;

            Host.Process.Export();
        }
    }
}
