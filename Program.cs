using System;
using System.Globalization;
using System.Linq;
using System.Text;
using wey.Command;
using wey.Console;
using wey.Core;
using wey.Tool;

namespace wey
{
    class Program
    {
        private static void WriteHelper(SubCommandBase subCommandBase, string[] parent)
        {
            Logger.WriteMultiple(
                    "NAME:",
                    $"{LoggerUtil.Tab}{subCommandBase.GetName()}",
                    "",
                    "DESCRIPTION:",
                    $"{LoggerUtil.Tab}{subCommandBase.GetDescription()}"
                );

            switch (subCommandBase.GetType())
            {
                case SubCommandType.Executable:
                    {
                        SubCommand subCommand = (SubCommand)subCommandBase;

                        Logger.WriteMultiple(
                                "",
                                "USAGE:",
                                $"{LoggerUtil.Tab}wey {string.Join(" ", parent)} {string.Join(" ", subCommand.GetSyntax().Select(syntax => $"<{syntax.GetName()}{(syntax.GetRequired() ? string.Empty : "?")}>"))}"
                            );

                        if (subCommand.GetSyntax().Length > 0)
                        {
                            Logger.WriteMultiple(
                                "",
                                "ARGUMENT:"
                            );

                            Logger.WriteMultiple(
                                    subCommand.GetSyntax().Select(
                                            syntax =>
                                            {
                                                if (syntax.GetHelp().Length > 0)
                                                {
                                                    return $"{LoggerUtil.Tab}{syntax.GetName()} - {syntax.GetDescription()} ({string.Join("|", syntax.GetHelp())})";
                                                }

                                                return $"{LoggerUtil.Tab}{syntax.GetName()} - {syntax.GetDescription()}";
                                            }
                                        ).ToArray()
                                );
                        }

                        if (subCommand.GetFlags().Length > 0)
                        {
                            Logger.WriteMultiple(
                                "",
                                "OPTIONS:"
                            );

                            Logger.WriteMultiple(
                                    subCommand.GetFlags().Select(
                                            flag =>
                                            {
                                                string baseString = $"{LoggerUtil.Tab}{flag.GetName()}";

                                                if (!flag.GetRequiredValue()) return baseString;

                                                switch (flag.GetFlagType())
                                                {
                                                    case SubCommandFlagType.String:
                                                        {
                                                            return $"{baseString} (--{flag.GetName()}=\"Hello World\")";
                                                        }
                                                    case SubCommandFlagType.Integer:
                                                        {
                                                            return $"{baseString} (--{flag.GetName()}=2019)";
                                                        }
                                                    case SubCommandFlagType.Decimal:
                                                        {
                                                            return $"{baseString} (--{flag.GetName()}=16.25)";
                                                        }
                                                    case SubCommandFlagType.Boolean:
                                                        {
                                                            return $"{baseString} (--{flag.GetName()}=true)";
                                                        }
                                                    default:
                                                        {
                                                            return baseString;
                                                        }
                                                }
                                            }
                                        ).ToArray()
                                );
                        }

                        break;
                    }
                case SubCommandType.Group:
                    {
                        SubCommandGroup subCommandGroup = (SubCommandGroup)subCommandBase;

                        Logger.WriteMultiple(
                                "",
                                "USAGE:",
                                $"{LoggerUtil.Tab}wey {string.Join(" ", parent)} [{string.Join("|", subCommandGroup.GetSubCommand().Select(command => command.GetName()))}]",
                                "",
                                "SUB COMMAND:"
                            );

                        Logger.WriteMultiple(
                                subCommandGroup.GetSubCommand().Select(
                                        command => $"{LoggerUtil.Tab}{command.GetName()} - {command.GetDescription()}"
                                    ).ToArray()
                            );

                        break;
                    }
            }
        }

        private static bool handleSyntax(SubCommandSyntax[] subCommandsSyntax, string[] args)
        {
            int index = 0;

            foreach (SubCommandSyntax subCommandSyntax in subCommandsSyntax)
            {
                if (!subCommandSyntax.GetRequired()) continue;

                if (args.Length < (index + 1)) return false;
                if (string.IsNullOrEmpty(args[index])) return false;

                index++;
            }

            return true;
        }

        private static bool handleCommand(SubCommandGroup subCommandBaseGroup, string[] args, string[] flags, int index = 0)
        {
            SubCommandBase[] subCommandsBase = subCommandBaseGroup.GetSubCommand();

            if (args.Length < (index + 1))
            {
                // no args
                WriteHelper(subCommandBaseGroup, args);
                return false;
            }

            foreach (SubCommandBase commandBase in subCommandsBase)
            {
                if (commandBase.GetName() != args[index]) continue;

                switch (commandBase.GetType())
                {
                    case SubCommandType.Executable:
                        {
                            SubCommand subCommand = (SubCommand)commandBase;
                            string[] argsNext = args.Skip(index + 1).ToArray();

                            string[] argsParent = (index == 0) ? args[0..] : args[0..index];

                            //syntax
                            if (!handleSyntax(subCommand.GetSyntax(), argsNext))
                            {
                                WriteHelper(subCommand, argsParent);
                                return false;
                            }

                            //flag
                            foreach (SubCommandFlag flag in subCommand.GetFlags())
                            {
                                if (!flag.GetRequiredValue()) continue;

                                Logger.Info($"\"{SubCommandFlag.GetContent(flags, flag.GetName())}\"");
                                if (SubCommandFlag.GetContent(flags, flag.GetName()) != string.Empty) continue;

                                WriteHelper(subCommand, argsParent);
                                return false;
                            }

                            //help flag
                            if (SubCommandFlag.GetUsed(flags, "help"))
                            {
                                WriteHelper(subCommand, argsParent);
                                return true;
                            }

                            try
                            {
                                subCommand.Execute(argsNext, flags);
                            }
                            catch (Exception)
                            {
                                Logger.Error("Server Error");
                                return false;
                            }

                            return true;
                        }
                    case SubCommandType.Group:
                        {
                            SubCommandGroup subCommandGroup = (SubCommandGroup)commandBase;

                            return handleCommand(subCommandGroup, args, flags, index + 1);
                        }
                }
            }

            // command not found
            WriteHelper(subCommandBaseGroup, args);
            return false;
        }

        public static void Main(string[] args)
        {
            List<string> arguments = new();
            List<string> flags = new();

            foreach (string arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    flags.Add(arg.Remove(0, 2));
                    continue;
                }

                arguments.Add(arg);
            }

            handleCommand(new Wey(), arguments.ToArray(), flags.ToArray());
        }
    }
}