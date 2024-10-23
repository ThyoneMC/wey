using Quickenshtein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
             * 1. Better dependency handling
             * 2. Better ".minecraft" Path Handling
             * 3. ModHandlerFactory (nowadays, handle one by one)
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
