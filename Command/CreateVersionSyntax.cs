using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class CreateVersionSyntax: SubCommandSyntax
    {
        public override string GetName()
        {
            return "version";
        }

        public override string GetDescription()
        {
            return "version of minecraft";
        }

        public override bool GetRequired()
        {
            return false;
        }
    }
}
