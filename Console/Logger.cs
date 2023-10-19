using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    static class LoggerUtil
    {
        public static string NewLine = "\n";
        public static string Tab = "\t";
    }

    class Logger
    {
        private static string Bracket(params string[] message)
        {
            StringBuilder stringBuilder = new();

            foreach (string text in message)
            {
                stringBuilder.Append($"[{text}] ");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }

        public static void WriteSingle(params string[] message)
        {
            System.Console.WriteLine(string.Join(" ", message));
        }

        public static void WriteMultiple(params string[] message)
        {
            foreach (string text in message)
            {
                System.Console.WriteLine(text);
            }
        }

        public static void CreateWriteLine(string mode, params string[] message)
        {
            WriteSingle(
                    Bracket(
                            DateTime.UtcNow.ToLocalTime().ToLongTimeString(),
                            mode
                        ),
                    string.Join(" ", message)
                );
        }

        public static void CreateWriteLine(string mode, ConsoleColor color, string[] message)
        {
            System.Console.ForegroundColor = color;

            CreateWriteLine(mode, message);

            System.Console.ResetColor();
        }

        public static void CreateWriteLine(string message, ConsoleColor color)
        {
            System.Console.ForegroundColor = color;

            WriteSingle(message);

            System.Console.ResetColor();
        }

        public static void Info(params string[] message)
        {
            CreateWriteLine("INFO", ConsoleColor.White, message);
        }

        public static void Help(params string[] message)
        {
            CreateWriteLine("HELP", ConsoleColor.Cyan, message);
        }

        public static void Warn(params string[] message)
        {
            CreateWriteLine("WARN", ConsoleColor.Yellow, message);
        }

        public static void Error(params string[] message)
        {
            CreateWriteLine("ERROR", ConsoleColor.Red, message);
        }

        public static void Error(Exception exception)
        {
            Error(exception.Message);
            if (exception.StackTrace != null) CreateWriteLine(exception.StackTrace, ConsoleColor.Red);
        }
    }
}
