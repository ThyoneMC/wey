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

namespace wey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
             * TODO:
             * 
             * 1. Config Command (Get, Set)
             * 2. Duplicate Mod Detector (By Name, Hash)
             * 3. Profile Importer (Local in Appdata, Local in Download, URL Download)
             * 4. Better ".minecraft" Path Handling
             * 5. Dynamic ModHandler (nowadays, handle one by one)
             * 6. Dependency Mod Downloader
             * 7. Incompatible Mod Detector
             * 8. Launcher Profile Update Within Update Command
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
