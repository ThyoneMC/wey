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
        public Home() : base("wey")
        {
            this.Description = "wey is the way to share minecraft mods";

            this.Subcommand.Add(new Create());
            this.Subcommand.Add(new Import());
            this.Subcommand.Add(new Add());
            this.Subcommand.Add(new Update());
            this.Subcommand.Add(new Load());
            this.Subcommand.Add(new Config());
        }

        public override void Execute()
        {
            
        }
    }
}
