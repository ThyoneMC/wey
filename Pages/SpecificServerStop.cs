using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Interface;

namespace wey.Pages
{
    class SpecificServerStop : IPage
    {
        public SpecificServerStop(string name) : base(name)
        {

        }

        public override string GetName()
        {
            return "stop";
        }

        public override string GetDescription()
        {
            return "stop the server";
        }

        public override void OnLoad()
        {
            Logger.WriteSingle("START --> Stp");
        }

        public override void OnViewing()
        {
            Logger.WriteSingle("VIEW --> Stp");
        }
    }
}
