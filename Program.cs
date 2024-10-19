using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using wey.API;
using wey.API.Game;
using wey.CLI;
using wey.IO;

namespace wey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (string text in args)
            {
                if (string.IsNullOrWhiteSpace(text)) continue;

                if (Options.IsArgOptions(text))
                {
                    Options.Import(text);
                }
                else
                {
                    Arguments.Import(text);
                }
            }

            string gameVersionID = new FabricClientHelper("1.20.6").Download();

            Launcher.AddProfile(new Launcher.ProfileOptions()
            {
                Name = "KiewCraft",
                MinecraftPath = Launcher.MinecraftPath,
                GameVersionID = gameVersionID,
                IconPath = ""
            });
        }
    }
}
