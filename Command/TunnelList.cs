using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Core;
using wey.Tool;

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

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            List<string> tunnels = new();

            ConfigData config = Config.Get();

            if (!string.IsNullOrEmpty(config.Ngrok)) tunnels.Add($"ngrok -> {config.Ngrok}");
            if (!string.IsNullOrEmpty(config.PlayIt)) tunnels.Add($"playit.gg -> {config.PlayIt}");
            if (!string.IsNullOrEmpty(config.Hamachi)) tunnels.Add($"hamachi -> {config.Hamachi}");

            Logger.WriteMultiple(tunnels.ToArray());
        }
    }
}
