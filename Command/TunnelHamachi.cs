using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Model;

namespace wey.Command
{
    class TunnelHamachi: SubCommand
    {
        public override string GetName()
        {
            return "hamachi";
        }

        public override string GetDescription()
        {
            return "hamachi";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return Array.Empty<SubCommandSyntax>();
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new TunnelDeleteFlag(),
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            //delete flag
            if (SubCommandFlag.GetUsed(flags, "delete"))
            {
                Config.Edit(
                    (data) => {
                        data.Tunnel.Hamachi = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            Logger.Warn("make sure it not shortcut or ui");
            string Hamachi_Path = Path.GetFullPath(Input.ReadString("hamachi path: "));

            if (!File.Exists(Hamachi_Path))
            {
                Logger.Error($"{Hamachi_Path} not found");
                return;
            }

            //config
            Config.Edit(
                    (data) => {
                        data.Tunnel.Hamachi = Hamachi_Path;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }
    }
}
