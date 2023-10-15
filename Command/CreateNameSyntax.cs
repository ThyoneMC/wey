using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class CreateNameSyntax : SubCommandSyntax
    {
        public override string GetName()
        {
            return "name";
        }

        public override string GetDescription()
        {
            return "name of the server";
        }

        public override bool GetRequired()
        {
            return true;
        }

        public override string[] GetHelp()
        {
            return new string[0];
        }
    }
}
