using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Model
{
    public abstract class SubCommandSyntax: SubCommandBase
    {
        public override SubCommandType GetType()
        {
            return SubCommandType.Syntax;
        }

        public abstract bool GetRequired();

        public abstract string[] GetHelp();
    }
}
