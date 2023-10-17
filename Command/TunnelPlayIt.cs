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
    class TunnelPlayIt: SubCommand
    {
        public override string GetName()
        {
            return "playit";
        }

        public override string GetDescription()
        {
            return "playit.gg";
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
                        data.PlayIt = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            string? PlayIt_Path = CommandPrompt.Where("playit.exe");
            if (PlayIt_Path == null)
            {
                string InputPath = Path.GetFullPath(Input.ReadString("playit.exe path: "));
                if (!File.Exists(InputPath))
                {
                    Logger.Error($"{InputPath} not found");
                    return;
                }

                PlayIt_Path = InputPath;
            }

            //config
            Config.Edit(
                    (data) => {
                        data.PlayIt = PlayIt_Path;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }

        public static void StartTunnel()
        {
            string PlayIt_Path = Config.Get().PlayIt;

            CommandPrompt.StaticExecute(
                    new CommandPrompt.CommandPromptOptions()
                    {
                        FileName = Path.GetFileName(PlayIt_Path),
                        WorkDirectory = Path.GetDirectoryName(PlayIt_Path)
                    }
                );
        }
    }
}
