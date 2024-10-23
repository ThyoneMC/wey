using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Mod;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class AddModrinth : Command
    {
        public AddModrinth() : base("modrinth")
        {
            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "ids",
                Type = CommandOptionsType.StringArray
            });
        }

        public override void Execute()
        {
            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            string[] ids = ConsoleHelper.ReadDynamicStringArray("ids");

            ModrinthHandler handler = new(profile.GameVersion);

            foreach (string modId in ids)
            {
                ModHandlerFile file = handler.Get(modId);

                profile.Mods.Add(file);

                ProfileHandler.Update(name, profile);
            }
        }
    }
}
