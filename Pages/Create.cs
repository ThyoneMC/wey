using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.API;
using wey.API.Game;
using wey.API.Mod;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Create : Command
    {
        public Create() : base("create")
        {
            this.Description = "new profile";

            this.Options.Add(new()
            {
                Name = "gameVersion",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "name",
                Type = CommandOptionsType.String
            });
        }

        public override void Execute()
        {
            string gameVersion = ConsoleHelper.ReadString("gameVersion");
            if (!new FabricClientHandler(gameVersion).ContainsGameVersion()) throw new Exception("game version not found");

            string name = ConsoleHelper.ReadString("name");
            if (ProfileHandler.Exists(name)) throw new Exception("profile already exists");

            ModrinthHandler handler = new(gameVersion);
            string fabricApiModId = "P7dR8mSH";

            ProfileHandler.Create(new()
            {
                Name = name,
                GameVersion = gameVersion,
                Mods = new()
                {
                    handler.Get(fabricApiModId)
                },
            });

            return;
        }
    }
}
