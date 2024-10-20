using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.CLI;
using wey.IO;

namespace wey.Pages
{
    public class AddCurseforge : Command
    {
        public override string GetName()
        {
            return "curseforge";
        }

        public override IHelpCommand GetHelp()
        {
            return new()
            {
                Options = new IHelpOptions[]
                {
                    new()
                    {
                        Name = "ids",
                        Type = IHelpOptionsType.IntArray,
                    },
                    new ()
                    {
                        Name = "curseforgeApi",
                        Type = IHelpOptionsType.String,
                        IsConfig = true
                    }
                }
            };
        }

        public override Command[] GetSubCommand()
        {
            return Array.Empty<Command>();
        }

        public override void Execute()
        {
            if (Configuration.Read("curseforgeApi") == null)
            {
                string apiKey = ConsoleHelper.ReadString("curseforgeApi");

                Configuration.Update("curseforgeApi", apiKey);
            }

            int[] ids = ConsoleHelper.ReadDynamicIntArray("ids");

            Console.WriteLine(JsonSerializer.Serialize(ids));

            return;
        }
    }
}
