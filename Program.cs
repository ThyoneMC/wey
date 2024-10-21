using Quickenshtein;
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
using wey.Pages;
using static wey.API.Mod.ICurseForge;

namespace wey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
             * TODO:
             * 
             * 4. Better ".minecraft" Path Handling
             * 5. Dynamic ModHandler (nowadays, handle one by one)
             * 8. Minecraft Launcher Profile
             */

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

            CommandHandler.Execute(new Home());
        }
    }
}
