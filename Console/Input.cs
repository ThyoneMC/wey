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
            if (!string.IsNullOrEmpty(message)) System.Console.Write(message);

            while (true)
            {
                string? read = System.Console.ReadLine();

                if (read != null) return read;
            }
        }

        public static bool ReadBoolean(string? message = null, bool? defaultOption = true)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message} (default: {defaultOption}) ");

            while (true)
            {
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

                return true;
            }
        }

        public static int ReadInteger(string? message = null, int? min = int.MinValue, int? max = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write(message);

            while (true)
            {
                string read = ReadString();

                int number;
                if (int.TryParse(read, out number))
                {
                    if (number < min || number > max) continue;

                    return number;
                }
            }
        }

        public static float ReadFloat(string? message = null, float? min = float.MinValue, float? max = float.MaxValue)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write(message);

            while (true)
            {
                string read = ReadString();

                float number;
                if (float.TryParse(read, out number))
                {
                    if (number < min || number > max) continue;

                    return number;
                }
            }
        }
    }
}
