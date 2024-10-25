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
    public class Server : Command
    {
        public Server() : base("server")
        {
            this.Description = "download mods for server";

            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
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
            string path = Environment.CurrentDirectory;
            if (CLI.Options.HasValue("path"))
            {
                path = CLI.Options.Get("path") ?? string.Empty;
            }

            if (!Directory.Exists(path) || Directory.GetFileSystemEntries(path).Length > 0)
            {
                Console.WriteLine($"{Path.GetFullPath(path)} is not empty directory");
                return;
            }

            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            ModHandlerFile[] serverMods = profile.Mods.Where(x => x.ServerSide).ToArray();
            ModHandlerFileExternal[] serverExternal = profile.ExternalMods.Where(x => x.ServerSide).ToArray();

            ProfileModHandler.Download(serverMods, serverExternal);

            ProfileModHandler.Load(Path.Join(path, "mods"), serverMods, serverExternal);

            new FabricServerHandler(profile.GameVersion).DownloadAndReturnServerFile(path);
        }
    }
}
