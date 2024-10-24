using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Mod;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class Add : Command
    {
        readonly string[] providers = Enum.GetNames(typeof(ModHandlerProvider));

        public Add() : base("add")
        {
            this.Description = "add mods";

            this.Options.Add(new()
            {
                Name = "curseforgeApi",
                Type = CommandOptionsType.String,
                Optional = true,
                InConfigFile = true
            });

            this.Options.Add(new()
            {
                Name = "ids",
                Type = CommandOptionsType.StringArray
            });

            this.Options.Add(new()
            {
                Name = "profile",
                Type = CommandOptionsType.String
            });

            foreach (string name in providers)
            {
                this.Subcommand.Add(new CommandByName(name.ToLower()));
            }
        }

        public override void Execute()
        {
            string? providerUse = Arguments.LastUse();

            ModHandlerProvider? provider = null;

            foreach (string providerName in providers)
            {
                if (String.Equals(providerUse, providerName, StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse(providerName, out ModHandlerProvider parseProvider))
                    {
                        provider = parseProvider;
                    }
                }
            }

            string name = ConsoleHelper.ReadString("profile");

            ISharedProfile? profile = ProfileHandler.Read(name);
            if (profile == null) throw new Exception("profile not found");

            ModHandler handler = ModHandlerFactory.Get(provider, profile.GameVersion);

            string[] ids = ConsoleHelper.ReadDynamicStringArray("ids");

            foreach (string modId in ids)
            {
                ModHandlerFile file = handler.Get(modId);

                profile.Mods.Add(file);

                ProfileHandler.Update(name, profile);
            }
        }
    }
}
