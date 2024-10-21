using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Config : Command
    {
        public Config() : base("config")
        {
            this.Description = "app configuration";

            this.Options.Add(new()
            {
                Name = "key",
                Type = CommandOptionsType.String,
            });

            this.Options.Add(new()
            {
                Name = "delete",
                Type = CommandOptionsType.Boolean,
                Optional = true
            });

            this.Options.Add(new()
            {
                Name = "value",
                Type = CommandOptionsType.String,
                Optional = true
            });
        }

        public override void Execute()
        {
            string key = ConsoleHelper.ReadString("key");

            if (CLI.Options.GetBool("delete", false))
            {
                Configuration.Delete(key);
            }
            else
            {
                string value = ConsoleHelper.ReadString("value");

                Configuration.Update(key, value);
            }
        }
    }
}
