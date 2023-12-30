using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    class Input
    {
        public static string ReadString(string? message = null, bool required = false, bool clear = false)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");

            (int StartingCursorLeft, int StartingCursorTop) = System.Console.GetCursorPosition();

            while (true)
            {
                System.Console.SetCursorPosition(StartingCursorLeft, StartingCursorTop);

                string? read = ConsoleReader.ReadLine();

                if (read != null)
                {
                    if (required && string.IsNullOrWhiteSpace(read)) continue;

                    if (clear)
                    {
                        Logger.ClearLine(StartingCursorTop);
                        System.Console.CursorTop = StartingCursorTop;
                    }

                    return read;
                }
            }
        }

        public static SelectionChoice<string> SelectionString(IEnumerable<string> choices, string? message = null)
        {
            int StartingCursorTop = System.Console.CursorTop;

            if (!string.IsNullOrEmpty(message)) System.Console.WriteLine($"{message}: ");

            Selection<string> Selector = Selection<string>.Create(choices);
            while (true)
            {
                Selector.RenderNext();

                if (Selector.Result != null)
                {
                    break;
                }
            }

            Logger.ClearFromLine(StartingCursorTop);

            if (!string.IsNullOrEmpty(message))
            {
                System.Console.CursorTop = StartingCursorTop;
                System.Console.WriteLine($"{message}: {Selector.Result.Value}");
            }

            return Selector.Result;
        }

        public static bool ReadBoolean(string? message = null, bool defaultOption = true)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message} (default: {defaultOption}): ");

            string read = ReadString(required: false).ToLower();

            // contains "no"
            if (defaultOption == true && (read.Contains('n') || read.ToLower() == "false"))
            {
                return false;
            }

            // contains "yes"
            if (defaultOption == false && (read.Contains('y') || read.ToLower() == "true"))
            {
                return true;
            }

            return defaultOption;
        }

        public static int ReadInteger(string? message = null, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");
                string read = ReadString(clear: true);

                if (int.TryParse(read, out int number))
                {
                    if (number < min || number > max) continue;

                    System.Console.WriteLine($"{message}: {number}");
                    return number;
                }

                Logger.ClearLine(System.Console.CursorTop);
            }
        }

        public static float ReadFloat(string? message = null, float min = float.MinValue, float max = float.MaxValue)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");
                string read = ReadString(clear: true);

                if (float.TryParse(read, out float number))
                {
                    if (number < min || number > max) continue;

                    System.Console.WriteLine($"{message}: {number}");
                    return number;
                }

                Logger.ClearLine(System.Console.CursorTop);
            }
        }
    }
}
