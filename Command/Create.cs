using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Provider;
using wey.Console;
using wey.Model;
using wey.Server;
using wey.Tool;

namespace wey.Command
{
    class Create: SubCommand
    {
        public override string GetName()
        {
            return "create";
        }

        public override string GetDescription()
        {
            return "create new server";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[]
            {
                new CreateProviderSyntax(),
                new CreateVersionSyntax(),
                new CreateNameSyntax(),
            };
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new CreateEulaFlag()
            };
        }

        public override void Execute(string[] args, ISubCommandFlags flags)
        {   
            if (!SubCommandFlag.GetUsed(flags, "eula") && !Input.ReadBoolean("Do you accept to Minecraft End User License Agreement?")) return;

            string Server_Provider = args[0];
            string Server_Version = args[1];
            string Server_Name = args[2];
            string Server_Path = Path.Join(Directory.GetCurrentDirectory(), Server_Name);

            // download

            ProviderBaseDownload ServerFile = ServerProvider.GetProvider(Server_Provider).GetServerJar(Server_Version);

            ServerManager server = new(
                                new ServerData(
                                        Server_Name,
                                        Server_Provider,
                                        ServerFile.BuildInfo,
                                        Server_Path
                                    )
                            );
            server.Create();
            server.AddServerFile(ServerFile.ServerJar);
        }
    }
}
