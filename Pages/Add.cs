using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;

namespace wey.Pages
{
    public class Add : Command
    {
        public override string GetName()
        {
            return "add";
        }

        public override IHelpCommand GetHelp()
        {
            return new()
            {
                Description = "add mods"
            };
        }

        public override Command[] GetSubCommand()
        {
            return new Command[]
            {
                new AddCurseforge(),
                new AddModrinth()
            };
        }

        public override void Execute()
        {
            return;
        }
    }
}
