using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;

namespace wey.Pages
{
    public class AddModrinth : Command
    {
        public override string GetName()
        {
            return "modrinth";
        }

        public override string? GetDescription()
        {
            return null;
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
            return Array.Empty<Command>();
        }

        public override void Execute()
        {
            return;
        }
    }
}
