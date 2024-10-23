using Quickenshtein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.API;
using wey.API.Mod;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class AddCurseforge : Command
    {
        public AddCurseforge() : base("curseforge")
        {
            this.Options.Add(new()
            {
                Name = "curseforgeApi",
                Type = CommandOptionsType.String,
                InConfigFile = true
            });

            this.Options.Add(new()
            {
                Name = "ids",
                Type = CommandOptionsType.IntegerArray
            });

            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
            });
        }

        public override void Execute()
        {
            if (Configuration.Read("curseforgeApi") == null)
            {
                string apiKey = ConsoleHelper.ReadString("curseforgeApi");

                Configuration.Update("curseforgeApi", apiKey);
            }

            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            int[] ids = ConsoleHelper.ReadDynamicIntArray("ids");

            CurseForgeHandler handler = new(profile.GameVersion);

            foreach (int modId in ids)
            {
                ModHandlerFile file = handler.Get(modId.ToString());

                profile.Mods.Add(file);
            }

            ProfileHandler.Update(name, profile);
        }
    }
}
