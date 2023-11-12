using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Provider;
using wey.Server;

namespace wey.Command
{
    class Update: SubCommand
    {
        public override string GetName()
        {
            return "update";
        }

        public override string GetDescription()
        {
            return "update server version";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[]
            {
                new CreateVersionSyntax()
            };
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
            if (!SubCommandFlag.GetUsed(flags, "force") && !Input.ReadBoolean($"Are you sure to update {TargetServer.Name}?")) return;

            string Server_Version = args[0];

            ServerManager server = new(TargetServer);

            ProviderBaseDownload ServerFile = ServerProvider.GetProvider(server.Data.Provider).GetServerJar(Server_Version);
            server.AddServerFile(ServerFile);
        }
    }
}
