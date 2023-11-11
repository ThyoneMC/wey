using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Model;

namespace wey.Command
{
    class TunnelList : SubCommand
    {
        public override string GetName()
        {
            return "list";
        }

        public override string GetDescription()
        {
            return "get all of enable tunnel";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return Array.Empty<SubCommandSyntax>();
        }

        public override SubCommandFlag[] GetFlags()
        {
            return Array.Empty<SubCommandFlag>();
        }

        public override void Execute(string[] args, ISubCommandFlags flags)
        {
            List<string> tunnels = new();

            ConfigData config = Config.Get();

            if (!string.IsNullOrEmpty(config.Tunnel.Ngrok)) tunnels.Add($"ngrok -> {config.Tunnel.Ngrok}");
            if (!string.IsNullOrEmpty(config.Tunnel.PlayIt)) tunnels.Add($"playit.gg -> {config.Tunnel.PlayIt}");
            if (!string.IsNullOrEmpty(config.Tunnel.Hamachi)) tunnels.Add($"hamachi -> {config.Tunnel.Hamachi}");

            Logger.WriteMultiple(tunnels.ToArray());
        }
    }
}
