using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Game;
using wey.CLI;
using wey.IO;
using static wey.ILauncher;

namespace wey.Pages
{
    public class Import : Command
    {
        public Import() : base("import")
        {
            this.Description = "import profile";

            this.Options.Add(new()
            {
                Name = "path",
                Type = CommandOptionsType.String
            });
        }

        public override void Execute()
        {
            string filePath = ConsoleHelper.ReadFilePath("path");

            ISharedProfile? profile = FileHelper.ReadJSON<ISharedProfile>(filePath);
            if (profile == null) throw new Exception("can not read profile");
            if (!new FabricClientHandler(profile.GameVersion).ContainsGameVersion()) throw new Exception("game version not found");

            ProfileHandler.Update(profile.Name, profile);
        }
    }
}
