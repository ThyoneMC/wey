using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class StartNameFlag: SubCommandFlag
    {
        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.String;
        }

        public override string GetName()
        {
            return "name";
        }

        public override string GetDescription()
        {
            return "name of the server";
        }

        public override bool GetRequiredValue()
        {
            return true;
        }
    }
}
