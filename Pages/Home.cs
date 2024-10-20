using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;

namespace wey.Pages
{
    public class Home : Command
    {
        public override string GetName()
        {
            return "wey";
        }

        public override IHelpCommand GetHelp()
        {
            return new()
            {
                Description = "wey is the way to share minecraft mods"
            };
        }

        public override Command[] GetSubCommand()
        {
            return new Command[]
            {
                new Create(),
                new Add()
            };
        }

        public override void Execute()
        {
            return;
        }
    }
}
