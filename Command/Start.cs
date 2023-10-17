using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Core;
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
                new StartNameFlag()
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            string TargetServerConfigPath;

            string local = Path.Join(Directory.GetCurrentDirectory(), ".wey", "config.json");
            if (File.Exists(local))
            {
                TargetServerConfigPath = local;
            }
            else if (SubCommandFlag.GetUsed(flags, "name"))
            {
                TargetServerConfigPath = SubCommandFlag.GetContentRequired(flags, "name");

                if (!File.Exists(TargetServerConfigPath))
                {
                    Logger.Error("Server Not Found");
                    return;
                }
            }
            else
            {
                List<string> choices = new();

                foreach (string itemName in FileController.StaticReadFolder(Directory.GetCurrentDirectory()).Folders)
                {
                    if (File.Exists(Path.Join(itemName, ".wey", "config.json")))
                    {
                        choices.Add(Path.GetFileName(itemName));
                    }
                }

                if (choices.Count == 0)
                {
                    Logger.Error("Server Not Found");
                    return;
                }

                string selectedChoice;
                if (choices.Count == 1)
                {
                    selectedChoice = choices.ToArray()[0];
                }
                else
                {
                    selectedChoice = Choice.Start(choices.ToArray());
                }

                TargetServerConfigPath = Path.Join(Directory.GetCurrentDirectory(), selectedChoice, ".wey", "config.json");
            }

            ServerManager server = new(JsonSerializer.Deserialize<ServerData>(FileController.StaticReadFile(TargetServerConfigPath)));
            server.Start();
        }
    }
}
