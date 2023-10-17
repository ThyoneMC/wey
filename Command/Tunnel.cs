using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Core;

namespace wey.Command
{
    class Tunnel : SubCommandGroup
    {
        public override string GetName()
        {
            return "tunnel";
        }

        public override string GetDescription()
        {
            return "auto port forwarding";
        }

        public override SubCommandBase[] GetSubCommand()
        {
            return new SubCommandBase[]
            {
                new TunnelNgrok(),
                new TunnelPlayIt(),
                new TunnelHamachi()
            };
        }
    }
}
