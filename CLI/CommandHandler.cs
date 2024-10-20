using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace wey.CLI
{
    public static class CommandHandler
    {
        static void HandleHelpSubcommand(List<Command> commandList)
        {
            if (commandList.Count == 0) return;

            Console.WriteLine();
            Console.WriteLine("SUB COMMAND:");

            foreach (Command command in commandList)
            {
                string text = $"\t{command.Name}";

                if (!string.IsNullOrWhiteSpace(command.Description))
                {
                    text += $" - {command.Description}";
                }

                Console.WriteLine(text);
            }
        }

        static void HandleHelpOption(List<CommandOptions> optionList)
        {
            if (optionList.Count == 0) return;

            Console.WriteLine();
            Console.WriteLine("OPTION:");

            foreach (CommandOptions option in optionList)
            {
                string text = $"\t--{option.Name}={option.Type.ToString()}";

                List<string> tags = new();
                if (option.InConfigFile) tags.Add("config");
                if (option.Optional) tags.Add("optional");

                if (tags.Count > 0)
                {
                    text += $" ({string.Join(", ", tags)})";
                }

                Console.WriteLine(text);
            }
        }

        static void HandleHelp(Command command)
        {
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine($"\t{command.Description ?? "no description"}");

            HandleHelpSubcommand(command.Subcommand);
            HandleHelpOption(command.Options);
        }

        public static void Execute(Command command)
        {
            string? subCommandUse = Arguments.Use();

            if (Options.IsUsed("help") && subCommandUse == null)
            {
                HandleHelp(command);
                return;
            }

            command.Execute();

            if (command.Subcommand.Count > 0)
            {
                foreach (Command subcommand in command.Subcommand)
                {
                    if (subcommand.Name == subCommandUse)
                    {
                        Execute(subcommand);
                        break;
                    }
                }
            }
        }
    }
}
