using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Interface;

namespace wey.Pages
{
    class SpecificServerStart : IPage
    {
        public SpecificServerStart(string name) : base(name)
        {

        }

        public override string GetName()
        {
            return "start";
        }

        public override string GetDescription()
        {
            return "start the server";
        }

        public override void OnLoad()
        {
            Logger.WriteSingle("START --> Srt");
        }

        public override void OnViewing()
        {
            Logger.WriteSingle("VIEW --> Srt");
        }
    }
}
