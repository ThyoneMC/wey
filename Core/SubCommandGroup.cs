using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public abstract class SubCommandGroup: SubCommandBase
    {
        public abstract SubCommandBase[] SubCommands {  get; set; }
    }
}
