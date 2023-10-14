using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public abstract class SubCommandSyntax: SubCommandBase
    {
        public abstract bool IsRequired { get; set; }
    }
}
