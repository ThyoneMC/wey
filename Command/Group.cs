using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class Group : SubCommandGroup
    {
        public override string GetName()
        {
            return "group";
        }

        public override string GetDescription()
        {
            return "group";
        }

        public override SubCommandBase[] GetSubCommand()
        {
            return new SubCommandBase[]
            {
                new Group(), new Ping(), new Echo()
            };
        }
    }
}
