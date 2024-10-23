using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class List : Command
    {
        public List() : base("list")
        {
            this.Options.Add(new()
            {
                Name = "profileList",
                Type = CommandOptionsType.Boolean,
                Optional = true
            });

            this.Options.Add(new()
            {
                Name = "profileName",
                Type = CommandOptionsType.String,
                Optional = true
            });
        }

        public override void Execute()
        {
            string[] displayList;

            if (CLI.Options.GetBool("profileList", false))
            {
                displayList = ProfileHandler.ReadAll().Select(x => x.Name).ToArray();
            }
            else
            {
                string name = ConsoleHelper.ReadString("profileName");

                ISharedProfile? profile = ProfileHandler.Read(name);
                if (profile == null) throw new Exception("profile not found");

                displayList = profile.Mods.Select(x => x.Name).ToArray();
            }

            foreach (string content in displayList)
            {
                Console.WriteLine($"\t- {content}");
            }
        }
    }
}
