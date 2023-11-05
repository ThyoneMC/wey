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
    class Start: SubCommand
    {
        public override string GetName()
        {
            return "start";
        }

        public override string GetDescription()
        {
            return "start the server";
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
                new StartForceFlag(),
                new StartServerOnlyFlag(),
                new StartRestartFlag()
            };
        }

        public override void Execute(string[] args, ISubCommandFlags flags)
        {
            ServerData TargetServer = GetTargetServer(flags);
            if (!SubCommandFlag.GetUsed(flags, "force") && !Input.ReadBoolean($"Are you sure to start {TargetServer.Name}?")) return;

            //tunnel
            if (!SubCommandFlag.GetUsed(flags, "server-only"))
            {
                ServerTunnel tunnel = new(TargetServer);
                tunnel.Start();
            }

            //server
            ServerManager server = new(TargetServer);
            server.Start(SubCommandFlag.GetUsed(flags, "auto-restart"));
        }

        // static

        public static ServerData GetTargetServer(ISubCommandFlags flags)
        {
            if (SubCommandFlag.GetUsed(flags, "name"))
            {
                string TargetServerConfigPath = ServerManager.FindServer(SubCommandFlag.GetContentRequired(flags, "name"));

                return ServerManager.GetServer(TargetServerConfigPath);
            }

            ServerData[] TargetServerData = ServerManager.GetServer(ServerManager.FindServer());

            return TargetServerData[Choice.StartGetIndex(TargetServerData.Select(data => data.FolderPath).ToArray())];
        }
    }
}
