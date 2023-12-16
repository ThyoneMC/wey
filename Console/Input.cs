using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    class Input
    {
        public static string ReadString(string? message = null)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");

            while (true)
            {
                string? read = KeyReader.ReadLine();

                if (read != null) return read;
            }
        }

        public static string SelectionString(string[] choices, string? message = null)
        {
            int StartingCursorTop = System.Console.CursorTop + 1;

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

            if (!string.IsNullOrEmpty(message)) System.Console.WriteLine($"{message}: {Selector.Result.Value}");

            return Selector.Result.Value;
        }

        public static bool ReadBoolean(string? message = null, bool defaultOption = true)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message} (default: {defaultOption}): ");

            string read = ReadString().ToLower();

            // contains "no"
            if (defaultOption == true && read.Contains('n'))
            {
                return false;
            }

            // contains "yes"
            if (defaultOption == false && read.Contains('y'))
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
                string read = ReadString();

                if (int.TryParse(read, out int number))
                {
                    if (number < min || number > max) continue;

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
                string read = ReadString();

                if (float.TryParse(read, out float number))
                {
                    if (number < min || number > max) continue;

                    return number;
                }

                Logger.ClearLine(System.Console.CursorTop);
            }
        }
    }
}
