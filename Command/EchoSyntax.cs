using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class EchoSyntax: SubCommandSyntax
    {
        public override string GetName()
        {
            return "name";
        }

        public override string GetDescription()
        {
            return "return pong";
        }

        public override bool GetRequired()
        {
            return false;
        }
    }
}
