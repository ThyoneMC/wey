using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Global;

namespace wey.Console
{
    class Logs
    {
        protected static readonly List<string> Data = new();

        protected static readonly StreamWriter File;

        static Logs()
        {
            string FilePath = Path.Join(StaticFolderController.AppdataPath, "logs", $"{DateTime.UtcNow.ToLocalTime().ToFileTime()}.log");

            StaticFileController.Build(FilePath, string.Empty);

            File = new(FilePath)
            {
                AutoFlush = true
            };
        }

        public static void Add(string data)
        {
            File.WriteLine(data);
        }

        public static void Stop()
        {
            File.Close();
        }
    }

    class Logger
    {
        public static void ClearLine(int line)
        {
            System.Console.SetCursorPosition(0, line - 1);
            System.Console.Write(new String(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, line - 1);
        }

        public static void ClearFromLine(int line)
        {
            int numClear = (System.Console.CursorTop + 1) - line;

            System.Console.SetCursorPosition(0, line - 1);
            while (numClear != 0)
            {
                System.Console.Write(new String(' ', System.Console.WindowWidth));

                numClear--;
            }
            System.Console.SetCursorPosition(0, line - 1);
        }

        private static string Bracket(params string[] message)
        {
            return $"[{string.Join("] [", message)}]";
        }

        private static string Combine(params string[] message)
        {
            return string.Join(' ', message);
        }

        public static void CreateWriteLine(string mode, ConsoleColor color, string[] message)
        {
            CreateWriteLine(
                    color: color,
                    message: Combine(
                            Bracket(
                                DateTime.UtcNow.ToLocalTime().ToLongTimeString(),
                                mode
                            ),
                            Log(message)
                        )
                );
        }

        public static void CreateWriteLine(ConsoleColor color, string message)
        {
            System.Console.ForegroundColor = color;

            WriteSingle(message);

            System.Console.ResetColor();
        }

        public static void WriteSingle(object? message)
        {
            if (message == null)
            {
                WriteSingle("null");
                return;
            };

            WriteSingle(message.ToString() ?? $"{message}");
        }

        public static void WriteSingle(params string[] message)
        {
            System.Console.WriteLine(Combine(message));
        }

        public static void WriteMultiple(params string[] message)
        {
            foreach (string text in message)
            {
                System.Console.WriteLine(text);
            }
        }

        public static void WriteLine()
        {
            System.Console.WriteLine(new String('-', System.Console.WindowWidth));
        }

        public static string Log(params string[] message)
        {
            string text = Combine(message);

            Logs.Add($"{DateTime.UtcNow.ToFileTime()} : {text}");

            return text;
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

        public static void Warn(Exception exception)
        {
            Warn(exception.Message);
            if (exception.StackTrace != null) CreateWriteLine(ConsoleColor.Yellow, exception.StackTrace);
        }

        public static void Error(params string[] message)
        {

            CreateWriteLine("ERROR", ConsoleColor.Red, message);
        }

        public static void Error(Exception exception)
        {
            Error(exception.Message);
            if (exception.StackTrace != null) CreateWriteLine(ConsoleColor.Red, exception.StackTrace);
        }
    }
}
