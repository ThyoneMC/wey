using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Game;
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
                Name = "profile",
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
            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            if (CLI.Options.HasValue("gameVersion"))
            {
                string gameVersion = CLI.Options.Get("gameVersion") ?? string.Empty;
                if (!new FabricClientHandler(gameVersion).ContainsGameVersion()) throw new Exception("game version not found");

                profile.GameVersion = gameVersion;
            }

            ModHandlerProvider[] providerName = profile.Mods.Select(x => x.Provider).Distinct().ToArray();
            List<ModHandlerFile> updatedMods = new();

            foreach (ModHandlerProvider provider in providerName)
            {
                ModHandlerFile[] mods = profile.Mods.Where(x => x.Provider == provider).ToArray();

                ModHandler handler = ModHandlerFactory.Get(provider, profile.GameVersion);

                mods = handler.Update(mods);

                updatedMods.AddRange(mods);
            }

            profile.Mods = (ModHandlerFileList)updatedMods;

            ProfileHandler.Update(name, profile);
        }
    }
}
