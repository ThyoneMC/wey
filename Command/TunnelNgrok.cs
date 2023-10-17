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
    class TunnelNgrok : SubCommand
    {
        public override string GetName()
        {
            return "ngrok";
        }

        public override string GetDescription()
        {
            return "ngrok";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return Array.Empty<SubCommandSyntax>();
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new TunnelDeleteFlag()
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            //delete flag
            if (SubCommandFlag.GetUsed(flags, "delete"))
            {
                Config.Edit(
                    (data) => {
                        data.Ngrok = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            string? Ngrok_Path = CommandPrompt.Where("ngrok.exe");
            if (Ngrok_Path == null)
            {
                string InputPath = Path.GetFullPath(Input.ReadString("ngrok.exe path: "));
                if (!File.Exists(InputPath))
                {
                    Logger.Error($"{InputPath} not found");
                    return;
                }

                Ngrok_Path = InputPath;
            }

            //config
            Config.Edit(
                    (data) => { 
                        data.Ngrok = Ngrok_Path;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }

        public static string StartTunnel(int port = 25565)
        {
            string PlayIt_Path = Config.Get().PlayIt;

            CommandPrompt.StaticExecute(
                    new CommandPrompt.CommandPromptOptions()
                    {
                        FileName = Path.GetFileName(PlayIt_Path),
                        Arguments = $"tcp {port}",
                        WorkDirectory = Path.GetDirectoryName(PlayIt_Path)
                    }
                );

            return $"{port}";
        }
    }
}
