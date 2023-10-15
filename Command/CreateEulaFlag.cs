using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class CreateEulaFlag: SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.Boolean;
        }

        public override string GetName()
        {
            return "eula";
        }

        public override string GetDescription()
        {
            return "accept to Minecraft EULA";
        }

        public override bool GetRequiredValue()
        {
            return false;
        }
    }
}
