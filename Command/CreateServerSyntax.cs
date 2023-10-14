using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class CreateServerSyntax: SubCommandSyntax
    {
        public override string GetName()
        {
            return "server";
        }

        public override string GetDescription()
        {
            return "name of server provider";
        }

        public override bool GetRequired()
        {
            return false;
        }
    }
}
