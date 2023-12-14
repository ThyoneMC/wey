using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Interface;

namespace wey.Pages
{
    class Main : IPageGroup
    {
        public override string GetName()
        {
            return "wey";
        }

        public override string GetDescription()
        {
            return "wey";
        }

        public override IPageBase[] GetPages()
        {
            return new IPageBase[] {
                    new Server()
            };
        }
    }
}
