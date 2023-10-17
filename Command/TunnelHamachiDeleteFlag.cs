using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class TunnelHamachiDeleteFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.String;
        }

        public override string GetName()
        {
            return "delete";
        }

        public override string GetDescription()
        {
            return "delete auto port forwarding of server name";
        }

        public override bool GetRequiredValue()
        {
            return true;
        }
    }
}
