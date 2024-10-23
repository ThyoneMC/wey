using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API.Mod;
using wey.API;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Append : Command
    {
        public Append() : base("append")
        {
            this.Description = "add mod manually";

            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "fileName",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "url",
                Type = CommandOptionsType.String
            });

            this.Options.Add(new()
            {
                Name = "replaceModId",
                Type = CommandOptionsType.String,
                Optional = true
            });

            this.Options.Add(new()
            {
                Name = "nonClientSide",
                Type = CommandOptionsType.Boolean,
                Optional = true
            });

            this.Options.Add(new()
            {
                Name = "nonServerSide",
                Type = CommandOptionsType.Boolean,
                Optional = true
            });
        }

        public override void Execute()
        {
            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            ModHandlerFileExternal file = new()
            {
                FileName = Path.ChangeExtension(ConsoleHelper.ReadString("fileName"), "jar"),
                URL = ConsoleHelper.ReadString("url"),
                ReplacementModID = CLI.Options.Get("replaceModId") ?? null,
                ClientSide = CLI.Options.GetBool("nonClientSide", true),
                ServerSide = CLI.Options.GetBool("nonServerSide", true)
            };

            bool contain = false;

            for (int i = 0; i < profile.ExternalMods.Count; i++)
            {
                ModHandlerFileExternal mod = profile.ExternalMods[i];
                if (string.IsNullOrWhiteSpace(file.ReplacementModID)) continue;

                if (file.ReplacementModID == mod.ReplacementModID)
                {
                    contain = true;

                    profile.ExternalMods[i] = mod;

                    break;
                }
            }

            if (!contain)
            {
                profile.ExternalMods.Add(file);
            }

            ProfileHandler.Update(name, profile);
        }
    }
}
