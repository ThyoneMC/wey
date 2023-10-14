using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    class ConsoleSystem
    {
        private static string Bracket(params string[] message)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string text in message)
            {
                stringBuilder.Append($"[{text}] ");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }

        public static void WriteLine(params string[] message)
        {
            System.Console.WriteLine(string.Join(" ", message));
        }

        private static void MakeWriteLine(string mode, string[] message)
        {
            WriteLine(
                    Bracket(
                            DateTime.UtcNow.ToLocalTime().ToString(),
                            mode
                        ), 
                    string.Join(" ", message)
                );
        }

        public static void Log(params string[] message)
        {
            MakeWriteLine("LOG", message);
        }

        public static void Warn(params string[] message)
        {
            MakeWriteLine("WARN", message);
        }

        public static void Error(params string[] message)
        {
            MakeWriteLine("ERROR", message);
        }
    }
}
