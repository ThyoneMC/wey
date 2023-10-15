using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public enum SubCommandType
    {
        Group,
        Executable,
        Syntax,
        Flag
    }

    public abstract class SubCommandBase
    {
        public new abstract SubCommandType GetType();
        public abstract string GetName();
        public abstract string GetDescription();
    }
}
