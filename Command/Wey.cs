using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Model;

namespace wey.Command
{
    class Wey: SubCommandGroup
    {
        public override string GetName()
        {
            return "wey";
        }

        public override string GetDescription()
        {
            return "wey is server manager for minecraft";
        }

        public override SubCommandBase[] GetSubCommand()
        {
            return new SubCommandBase[]
            {
                new Create(), new Start(), new Stop(), new Delete(), new Tunnel()
            };
        }
    }
}
