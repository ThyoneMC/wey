using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class CreateNameSyntax: SubCommandSyntax
    {
        public override string GetName()
        {
            return "name";
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
