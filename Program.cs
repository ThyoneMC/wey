using System.Globalization;
using System.Text;
using wey.Command;
using wey.Core;
using wey.Tool;

namespace wey
{
    class Program
    {
        private static string GetSyntaxHelper(SubCommandSyntax[] subCommandsSyntax)
        {
            string[] syntaxNextName = subCommandsSyntax.Select(syntax =>
            {
                StringBuilder stringBuilder = new();
                stringBuilder.Append($"<{syntax.GetName()}");

                if (!syntax.GetRequired()) stringBuilder.Append('?');

                stringBuilder.Append(">");
                return stringBuilder.ToString();
            }).ToArray();

            return string.Join(" ", syntaxNextName);
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

        private static string GetCommandHelper(SubCommandBase[] subCommandsBase, string[] args, int index)
        {
            string[] argsNextName = subCommandsBase.Select(subCommand => subCommand.GetName()).ToArray();

            StringBuilder stringBuilder = new();
            stringBuilder.Append("wey ");

            string[] argsTrue = args[0..index];
            if (argsTrue.Length > 0)
            {
                stringBuilder.AppendJoin(' ', argsTrue);
                stringBuilder.Append(' ');
            }

            string argFalse = string.Join(" | ", argsNextName);
            if (argsNextName.Length > 0) stringBuilder.Append($"[ {argFalse} ]");

            return stringBuilder.ToString();
        }

        private static bool handleCommand(SubCommandBase[] subCommandsBase, string[] args, string[] flags, int index = 0)
        {
            if (args.Length < (index + 1))
            {
                // no args
                System.Console.WriteLine(GetCommandHelper(subCommandsBase, args, index));
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

                            //syntax
                            if (!handleSyntax(subCommand.GetSyntax(), argsNext))
                            {
                                System.Console.WriteLine($"wey {string.Join(" ", args[0..index])} {GetSyntaxHelper(subCommand.GetSyntax())}");
                                return false;
                            }

                            //flag
                            foreach (string flag in subCommand.GetFlags())
                            {
                                if (flag.EndsWith("?")) continue;

                                if (SubCommand.GetFlagContent(flags, flag) != "NO_INPUT") continue;

                                System.Console.WriteLine($"Please insert value of {flag}");
                                System.Console.WriteLine($"Example: --description=WeyIsTheBest");
                                return false;
                            }

                            Task
                                .Run(async () =>
                                {
                                    await subCommand.Execute(argsNext, flags);
                                })
                                .Wait();

                            return true;
                        }
                    case SubCommandType.Group:
                        {
                            SubCommandGroup subCommandGroup = (SubCommandGroup)commandBase;

                            return handleCommand(subCommandGroup.GetSubCommand(), args, flags, index + 1);
                        }
                }
            }

            // command not found
            System.Console.WriteLine(GetCommandHelper(subCommandsBase, args, index));
            return false;
        }

        public static SubCommandBase[] SubCommands = new SubCommandBase[]
        {
            new Create()
        };

        public static void Main(string[] args)
        {
            List<string> arguments = new List<string>();
            List<string> flags = new List<string>();

            foreach (string arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    flags.Add(arg.Remove(0, 2));
                    continue;
                }

                arguments.Add(arg);
            }

            handleCommand(SubCommands, arguments.ToArray(), flags.ToArray());
        }
    }
}