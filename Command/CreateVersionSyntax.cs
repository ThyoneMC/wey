using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Provider;
using wey.Model;

namespace wey.Command
{
    class CreateVersionSyntax : SubCommandSyntax
    {
        public override string GetName()
        {
            return "version";
        }

        public override string GetDescription()
        {
            return "game version";
        }

        public override bool GetRequired()
        {
            return true;
        }

        public override string[] GetHelp()
        {
            return new string[]
            {
                Vanilla.VersionType.Release,
                Vanilla.VersionType.Snapshot,
                "1.8.9",
                "..."
            };
        }
    }
}
