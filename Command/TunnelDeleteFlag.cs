using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Model;

namespace wey.Command
{
    class TunnelDeleteFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.Boolean;
        }

        public override string GetName()
        {
            return "delete";
        }

        public override string GetDescription()
        {
            return "delete auto port forwarding";
        }

        public override bool GetRequiredValue()
        {
            return false;
        }
    }
}
