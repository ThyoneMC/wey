using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Interface;

namespace wey.Pages
{
    class SpecificServer : IPageGroup
    {
        public SpecificServer(string name) : base(name)
        {

        }

        public override string GetName()
        {
            return arguments[0];
        }

        public override string GetDescription()
        {
            return "server management";
        }

        public override IPageBase[] GetPages()
        {
            return new IPageBase[] {
                    new SpecificServerStart(arguments[0]),
                    new SpecificServerStop(arguments[0]),
                    new SpecificServerDelete(arguments[0]),
            };
        }
    }
}
