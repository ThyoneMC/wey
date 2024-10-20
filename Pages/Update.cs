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
    public class Update : Command
    {
        public Update() : base("update")
        {
            this.Description = "update mods";

            this.Options.Add(new()
            {
                Name = "name",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "gameVersion",
                Type = CommandOptionsType.String,
                Optional = true
            });
        }

        public override void Execute()
        {
            string name = ConsoleHelper.ReadString("name");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            if (CLI.Options.HasValue("gameVersion"))
            {
                profile.GameVersion = CLI.Options.Get("gameVersion") ?? string.Empty;
            }

            ModHandlerFile[] modrinthMods = profile.Mods.Where(x => x.Provider == ModHandlerProvider.Modrinth).ToArray();
            ModHandlerFile[] curseforgeMods = profile.Mods.Where(x => x.Provider == ModHandlerProvider.CurseForge).ToArray();

            ModrinthHandler modrinthHandler = new(profile.GameVersion);
            CurseForgeHandler curseForgeHandler = new(profile.GameVersion);

            modrinthMods = modrinthHandler.Update(modrinthMods);
            curseforgeMods = curseForgeHandler.Update(curseforgeMods);

            profile.Mods = modrinthMods.Union(curseforgeMods).ToList();

            ProfileHandler.Update(name, profile);
        }
    }
}
