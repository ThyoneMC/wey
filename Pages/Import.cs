using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;

namespace wey.Pages
{
    public class Import : Command
    {
        public Import() : base("import")
        {
            this.Description = "import profile";

            this.Options.Add(new()
            {
                Name = "url",
                Type = CommandOptionsType.String,
                Optional = true
            });
        }

        public override void Execute()
        {
            if (CLI.Options.HasValue("url"))
            {
                // download from internet
            }
        }
    }
}
