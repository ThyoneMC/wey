using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Game;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Import : Command
    {
        public Import() : base("import")
        {
            this.Description = "import profile";

            this.Options.Add(new()
            {
                Name = "url",
                Type = CommandOptionsType.String,
                Optional = true
            });

            this.Options.Add(new()
            {
                Name = "path",
                Type = CommandOptionsType.String,
                Optional = true
            });
        }

        public override void Execute()
        {
            string importPath;

            if (CLI.Options.HasValue("url"))
            {
                string url = CLI.Options.Get("url") ?? string.Empty;

                importPath = Downloader.Download(url, $"profile-{DateTime.Now.ToFileTimeUtc()}");
            }
            else
            {
                importPath = Path.GetFullPath(ConsoleHelper.ReadString("path"));
            }

            ISharedProfile? profile = FileHelper.ReadJSON<ISharedProfile>(importPath);
            if (profile == null) throw new Exception("can not read profile");
            if (!new FabricClientHandler(profile.GameVersion).ContainsGameVersion()) throw new Exception("game version not found");

            ProfileHandler.Update(profile.Name, profile);
        }
    }
}
