using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Interface;

namespace wey.Pages
{
    class ServerList : IPageGroup
    {
        public override string GetName()
        {
            return "list";
        }

        public override string GetDescription()
        {
            return "servers list";
        }

        public override IPageBase[] GetPages()
        {
            List<IPageBase> list = new();

            string[] fakeServerList = new string[] { "pete", "is", "my", "friends" };

            foreach (string fakeServer in fakeServerList)
            {
                list.Add(new SpecificServer(fakeServer));
            }

            return list.ToArray();
        }
    }
}
