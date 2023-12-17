using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Interface;

namespace wey.Pages
{
    class Server : IPageGroup
    {
        public override string GetName()
        {
            return "server";
        }

        public override string GetDescription()
        {
            return "server";
        }

        public override IEnumerable<IPageBase> GetPages()
        {
            return new IPageBase[] {
                    new ServerList(),
                    new ServerCreate()
            };
        }
    }
}
