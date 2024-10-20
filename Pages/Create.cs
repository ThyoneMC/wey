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
    public class Create : Command
    {
        public Create() : base("create")
        {
            this.Description = "add mods";

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

            IFabric.IVersion[]? versions = Fabric.GetGames();
            if (versions == null) throw new Exception("rest error - Fabric.GetGames");
            if (!versions.Where(x => x.IsStable).Select(x => x.Version).Contains(gameVersion)) throw new Exception("game version not found");

            string name = ConsoleHelper.ReadString("name");

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
