using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
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

        public override void Execute(string[] args, ISubCommandFlags flags)
        {
            //delete flag
            if (SubCommandFlag.GetUsed(flags, "delete"))
            {
                Config.Edit(
                    (data) => {
                        data.Tunnel.PlayIt = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            string PlayIt_Path = TunnelPathFlag.GetRequiredPath(flags, "playit.exe");

            //config
            Config.Edit(
                    (data) => {
                        data.Tunnel.PlayIt = PlayIt_Path;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }
    }
}
