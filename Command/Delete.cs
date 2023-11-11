using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Server;
using wey.Tool;

namespace wey.Command
{
    class Delete : SubCommand
    {
        public override string GetName()
        {
            return "delete";
        }

        public override string GetDescription()
        {
            return "delete the server";
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

        public override void Execute(string[] args, ISubCommandFlags flags)
        {
            ServerData TargetServer = Start.GetTargetServer(flags);
            if (!SubCommandFlag.GetUsed(flags, "force") && !Input.ReadBoolean($"Are you sure to delete {TargetServer.Name}?")) return;

            //tunnel
            ServerTunnel tunnel = new(TargetServer);

            tunnel.Hamachi.Delete();

            //server
            ServerManager server = new(TargetServer);

            server.Delete(!SubCommandFlag.GetUsed(flags, "force"));
        }
    }
}
