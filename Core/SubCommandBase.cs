using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public abstract class SubCommandBase
    {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
    }
}
