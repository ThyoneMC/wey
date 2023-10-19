using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class StartForceFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.Boolean;
        }

        public override string GetName()
        {
            return "force";
        }

        public override string GetDescription()
        {
            return "no sure asking";
        }

        public override bool GetRequiredValue()
        {
            return false;
        }
    }
}
