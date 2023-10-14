using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public abstract class SubCommand: SubCommandBase
    {
        public override SubCommandType GetType()
        {
            return SubCommandType.Executable;
        }

        public abstract SubCommandSyntax[] GetSyntax();

        public abstract string[] GetFlag();

        public abstract void Execute(string[] args, string[] flags);
    }
}
