using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;
using wey.Server;

namespace wey.Command
{
    public static class ServerProvider
    {
        public const string Vanilla = "vanilla";
        public const string PaperMC = "paper";
        public const string FabricMC = "fabric";
    }

    class CreateProviderSyntax : SubCommandSyntax
    {
        public override string GetName()
        {
            return "provider";
        }

        public override string GetDescription()
        {
            return "name of server provider";
        }

        public override bool GetRequired()
        {
            return true;
        }

        public override string[] GetHelp()
        {
            return new string[]
            {
                ServerProvider.Vanilla,
                ServerProvider.PaperMC,
                ServerProvider.FabricMC
            };
        }
    }
}
