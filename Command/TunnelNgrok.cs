using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Model;
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

        public override void Execute(string[] args, ISubCommandFlags flags)
        {
            //delete flag
            if (SubCommandFlag.GetUsed(flags, "delete"))
            {
                Config.Edit(
                    (data) => {
                        data.Tunnel.Ngrok = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            string Ngrok_Path = TunnelPathFlag.GetRequiredPath(flags, "ngrok.exe");

            //config
            Config.Edit(
                    (data) => { 
                        data.Tunnel.Ngrok = Ngrok_Path;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }
    }
}
