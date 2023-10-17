using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class TunnelHamachiJoinFlag : SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.String;
        }

        public override string GetName()
        {
            return "join";
        }

        public override string GetDescription()
        {
            return "join info";
        }

        public override bool GetRequiredValue()
        {
            return true;
        }
    }
}
