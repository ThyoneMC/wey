using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using wey.Command;
using wey.Console;
using wey.Model;
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
                                                    return $"{LoggerUtil.Tab}{syntax.GetName()} -> {syntax.GetDescription()} ({string.Join("|", syntax.GetHelp())})";
                                                }

                                                return $"{LoggerUtil.Tab}{syntax.GetName()} -> {syntax.GetDescription()}";
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
                                                string baseString = $"{LoggerUtil.Tab}--{flag.GetName()} -> {flag.GetDescription()}";

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

        private static bool HandleSyntax(SubCommandSyntax[] subCommandsSyntax, string[] args)
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

        private static bool HandleCommand(SubCommandGroup subCommandBaseGroup, string[] args, string[] flags, int index = 0)
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

                            string[] argsParent = (index == 0) ? (new string[] { args[0] }) : args[0..(index + 1)];

                            //syntax
                            if (!HandleSyntax(subCommand.GetSyntax(), argsNext))
                            {
                                WriteHelper(subCommand, argsParent);
                                return false;
                            }

                            //flag
                            Dictionary<string, string?> flagDictionary = new();

                            foreach (string flagString in flags)
                            {
                                int indexOfEqualSign = flagString.IndexOf("=");
                                if (indexOfEqualSign == -1)
                                {
                                    // no value
                                    flagDictionary.Add(flagString, string.Empty);
                                    continue;
                                }

                                // value
                                string content = flagString[(indexOfEqualSign + "=".Length)..];
                                if (string.IsNullOrWhiteSpace(content))
                                {
                                    flagDictionary.Add(flagString[..indexOfEqualSign], string.Empty);
                                    continue;
                                }

                                flagDictionary.Add(flagString[..indexOfEqualSign], content);
                            }

                            //flag help 
                            if (SubCommandFlag.GetUsed(flagDictionary, "help"))
                            {
                                WriteHelper(subCommand, argsParent);
                                return true;
                            }

                            // flag check
                            foreach (SubCommandFlag subCommandFlag in subCommand.GetFlags())
                            {
                                if (!subCommandFlag.GetRequiredValue()) continue;

                                foreach (string flagString in flagDictionary.Keys)
                                {
                                    if (subCommandFlag.GetName() == flagString)
                                    {
                                        if (flagDictionary[flagString] == string.Empty)
                                        {
                                            WriteHelper(subCommand, argsParent);
                                            return false;
                                        }
                                    }
                                }
                            }

                            try
                            {
                                subCommand.Execute(argsNext, flagDictionary);
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(exception);
                                return false;
                            }

                            return true;
                        }
                    case SubCommandType.Group:
                        {
                            SubCommandGroup subCommandGroup = (SubCommandGroup)commandBase;

                            return HandleCommand(subCommandGroup, args, flags, index + 1);
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

            HandleCommand(new Wey(), arguments.ToArray(), flags.ToArray());
        }
    }
}