using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.API;
using wey.API.Game;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Load : Command
    {
        public Load() : base("load")
        {
            this.Description = "download mods for client";

            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
            });
        }

        public override void Execute()
        {
            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            ModHandlerFile[] clientMods = profile.Mods.Where(x => x.ClientSide).ToArray();
            ModHandlerFileExternal[] clientExternal = profile.ExternalMods.Where(x => x.ClientSide).ToArray();

            ProfileModHandler.Download(clientMods, clientExternal);

            string gameModsPath = Path.Join(Launcher.GameDirectoryPath, "mods");

            string[] oldModFiles = Directory.GetFiles(gameModsPath);
            if (oldModFiles.Length > 0)
            {
                string newOldModsDir = Path.Join(gameModsPath, $"mods-{DateTime.Now.ToFileTimeUtc()}");
                DirectoryHelper.CloneFiles(gameModsPath, newOldModsDir);
                FileHelper.Delete(oldModFiles);

                Console.WriteLine($"old mods has been moved to {newOldModsDir}");
            }

            ProfileModHandler.Load(gameModsPath, clientMods, clientExternal);

            string gameVersionDir = new FabricClientHandler(profile.GameVersion).DownloadAndReturnVersionDir();

            Launcher.AddProfile(new()
            {
                Name = profile.Name,
                GameVersionID = DirectoryHelper.GetRootDirectoryName(gameVersionDir),
                IconString = profile.LauncherIconString
            });
        }
    }
}
