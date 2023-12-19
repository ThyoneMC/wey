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
        private static (int Left, int Top)? CursorReturnPosition = null;

        public static void SetCursorReturnPoint()
        {
            CursorReturnPosition = System.Console.GetCursorPosition();
        }

        public static void CursorReturnPoint(bool reset = true)
        {
            if (CursorReturnPosition == null) return;

            System.Console.SetCursorPosition(CursorReturnPosition.Value.Left, CursorReturnPosition.Value.Top);

            if (reset) CursorReturnPosition = null;
        }

        public static void ClearLine(int line)
        {
            SetCursorReturnPoint();

            System.Console.SetCursorPosition(0, line);
            System.Console.Write(new String(' ', System.Console.WindowWidth));

            CursorReturnPoint();
        }

        public static void ClearFromLine(int line, int end = -1)
        {
            if (end < 0) end = System.Console.CursorTop;

            SetCursorReturnPoint();

            int clearLines = (end + 1) - line;
            System.Console.SetCursorPosition(0, line);
            while (clearLines != 0)
            {
                System.Console.Write(new String(' ', System.Console.WindowWidth));

                clearLines--;
            }

            CursorReturnPoint();
        }

        private static string Bracket(params string[] message)
        {
            return $"[{string.Join("] [", message)}]";
        }

        private static string Combine(params string[] message)
        {
            return string.Join(' ', message);
        }

        public static int CreateWriteLine(ConsoleColor color, string message)
        {
            System.Console.ForegroundColor = color;

            int CursorTop = WriteSingle(message);

            System.Console.ResetColor();

            return CursorTop;
        }

        public static int CreateWriteLine(string mode, ConsoleColor color, string[] message)
        {
            return CreateWriteLine(
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

        public static int WriteSingle(object? message)
        {
            if (message == null)
            {
                System.Console.WriteLine("null");
            }
            else
            {
                if (typeof(object) == typeof(IEnumerable<string>))
                {
                    message = Combine((string[])message);
                }

                System.Console.WriteLine(message.ToString() ?? $"{message}");
            }

            return System.Console.CursorTop - 1;
        }

        public static (int StartLine, int EndLine) WriteMultiple(params string[] message)
        {
            List<int> CursorTop = new();

            foreach (string text in message)
            {
                CursorTop.Add(WriteSingle(text));
            }

            return (CursorTop.Min(), CursorTop.Max());
        }

        public static int WriteLine()
        {
            return WriteSingle(new String('-', System.Console.WindowWidth));
        }

        public static string Log(params string[] message)
        {
            string text = Combine(message);

            Logs.Add($"{DateTime.UtcNow.ToFileTime()} : {text}");

            return text;
        }

        public static int Info(params string[] message)
        {
            return CreateWriteLine("INFO", ConsoleColor.White, message);
        }

        public static int Help(params string[] message)
        {
            return CreateWriteLine("HELP", ConsoleColor.Cyan, message);
        }

        public static int Warn(params string[] message)
        {
            return CreateWriteLine("WARN", ConsoleColor.Yellow, message);
        }

        public static (int StartLine, int EndLine) Warn(Exception exception)
        {
            Warn(exception.Message);
            if (exception.StackTrace != null) CreateWriteLine(ConsoleColor.Yellow, exception.StackTrace);

            return (System.Console.CursorTop - 1, System.Console.CursorTop);
        }

        public static int Error(params string[] message)
        {

            return CreateWriteLine("ERROR", ConsoleColor.Red, message);
        }

        public static (int StartLine, int EndLine) Error(Exception exception)
        {
            Error(exception.Message);
            if (exception.StackTrace != null) CreateWriteLine(ConsoleColor.Red, exception.StackTrace);

            return (System.Console.CursorTop - 1, System.Console.CursorTop);
        }
    }
}
