using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API.Game;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Create : Command
    {
        public override string GetName()
        {
            return "create";
        }

        public override string? GetDescription()
        {
            return "create new profile";
        }

        public override IHelpCommand GetHelp()
        {
            return new()
            {
                Description = "wey is the way to share minecraft mods"
            };
        }

        public override Command[] GetSubCommand()
        {
            return Array.Empty<Command>();
        }

        public override void Execute()
        {
            string gameVersion = ConsoleHelper.ReadString("gameVersion");

            IFabric.IVersion[]? versions = Fabric.GetGames();
            if (versions == null) throw new Exception("rest error - Fabric.GetGames");

            if (!versions.Select(x => x.Version).Contains(gameVersion))
            {
                throw new Exception("game version not found");
            }

            string name = ConsoleHelper.ReadString("name");

            ProfileHandler.Create(new()
            {
                Name = name,
                GameVersion = gameVersion
            });

            return;
        }
    }
}
