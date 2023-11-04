using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Server;
using wey.Tool;

namespace wey.Command
{
    class Stop : SubCommand
    {
        public override string GetName()
        {
            return "stop";
        }

        public override string GetDescription()
        {
            return "stop the server";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return Array.Empty<SubCommandSyntax>();
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new StartNameFlag(),
                new StartForceFlag()
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            ServerData TargetServer = Start.GetTargetServer(flags);
            if (!SubCommandFlag.GetUsed(flags, "force") && !Input.ReadBoolean($"Are you sure to stop {TargetServer.Name}?")) return;

            //tunnel
            ServerTunnel tunnel = new(TargetServer);
            tunnel.Stop();

            //server
            ServerManager server = new(TargetServer);
            server.Stop();
        }
    }
}
