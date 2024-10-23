using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.API;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Load : Command
    {
        public Load() : base("load")
        {
            this.Description = "download mods";

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

            ProfileModHandler.Download(ApplicationDirectoryHelper.Temporary, profile.Mods.ToArray());
            DirectoryHelper.Clone(ApplicationDirectoryHelper.Temporary, Path.Join(ApplicationDirectoryHelper.Appdata, "mods"));

            Launcher.AddProfile(profile);
        }
    }
}
