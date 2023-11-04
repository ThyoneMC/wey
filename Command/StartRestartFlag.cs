using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Model;

namespace wey.Command
{
    class StartRestartFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.Boolean;
        }

        public override string GetName()
        {
            return "auto-restart";
        }

        public override string GetDescription()
        {
            return "auto restart when server crash";
        }

        public override bool GetRequiredValue()
        {
            return false;
        }
    }
}
