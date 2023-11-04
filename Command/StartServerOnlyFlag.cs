using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Model;

namespace wey.Command
{
    class StartServerOnlyFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.Boolean;
        }

        public override string GetName()
        {
            return "server-only";
        }

        public override string GetDescription()
        {
            return "start server without starting tunnel";
        }

        public override bool GetRequiredValue()
        {
            return false;
        }
    }
}
