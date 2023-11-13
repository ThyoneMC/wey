using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Global;
using wey.Model;
using wey.Tool;

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
                new TunnelPathFlag()
            };
        }

        public static string __ExecuteName = new ExecutablePlatform("hamachi-2.exe", "hamachi").Get();

        public override void Execute(string[] args, ISubCommandFlags flags)
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
            string Hamachi_Path = TunnelPathFlag.GetRequiredPath(flags, __ExecuteName);

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
