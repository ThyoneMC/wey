using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Model
{
    public abstract class SubCommand: SubCommandBase
    {
        public override SubCommandType GetType()
        {
            return SubCommandType.Executable;
        }

        public abstract SubCommandSyntax[] GetSyntax();

        public abstract SubCommandFlag[] GetFlags();

        public abstract void Execute(string[] args, Dictionary<string, string?> flags);
    }
}
