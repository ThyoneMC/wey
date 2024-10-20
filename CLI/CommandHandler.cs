using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.CLI
{
    public static class CommandHandler
    {
        static string NameBuilder(string name, string? description)
        {
            string text = $"\t{name}";

            if (!string.IsNullOrWhiteSpace(description))
            {
                text += $" - {description}";
            }

            return text;
        }

        public static void Execute(Command command)
        {
            string? subname = Arguments.Use();

            if (Options.IsUsed("help") && subname == null)
            {
                string des = command.GetDescription() ?? "no description";

                Console.WriteLine("DESCRIPTION:");
                Console.WriteLine($"\t{des}");

                Command[] thisCmd = command.GetSubCommand();
                if (thisCmd.Length > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("SUB COMMAND:");
                    
                    foreach (Command subcommand in thisCmd)
                    {
                        Console.WriteLine(NameBuilder(subcommand.GetName(), subcommand.GetDescription()));
                    }
                }

                return;
            }

            command.Execute();

            Command[] commands = command.GetSubCommand();
            if (commands.Length > 0)
            {
                if (subname == null) return;

                foreach (Command subcommand in commands)
                {
                    if (subcommand.GetName() == subname)
                    {
                        Execute(subcommand);
                        break;
                    }
                }
            }
        }
    }
}
