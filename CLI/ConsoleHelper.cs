using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.IO;

namespace wey.CLI
{
    public static class ConsoleHelper
    {
        static string FormatKeyString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            StringBuilder builder = new();

            builder.Append(char.ToUpper(str[0]));

            for (int i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    builder.Append(' ');
                }

                builder.Append(str[i]);
            }

            return builder.ToString();
        }

        public static string ReadString(string key)
        {
            if (Options.HasValue(key))
            {
                return Options.Get(key) ?? string.Empty;
            }

            Console.Write($"{FormatKeyString(key)}: ");
            string? content = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("read error");
                content = ReadString(key);
            }

            return content;
        }

        public static string[] ReadStringArray(string key)
        {
            string content = ReadString(key);

            string[]? arr = JsonSerializer.Deserialize<string[]>(content);
            if (arr == null) throw new Exception("console error - ConsoleHelper.ReadStringArray");

            return arr;
        }

        public static string[] ReadDynamicStringArray(string key)
        {
            if (Options.HasValue(key))
            {
                return ReadStringArray(key);
            }

            List<string> data = new();

            Console.WriteLine($"{FormatKeyString(key)}: ");

            string? str = null;
            while (true)
            {
                Console.Write("\t");
                str = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(str))
                {
                    break;
                }

                data.Add(str);
            }

            return data.ToArray();
        }

        public static int ReadInt(string key)
        {
            string content = ReadString(key);

            if (!int.TryParse(content, out int value))
            {
                Console.WriteLine("read error - int");
                content = ReadString(key);
            }

            return value;
        }

        public static int[] ReadIntArray(string key)
        {
            string content = ReadString(key);

            int[]? arr = JsonSerializer.Deserialize<int[]>(content);
            if (arr == null) throw new Exception("console error - ConsoleHelper.ReadIntArray");

            return arr;
        }

        public static int[] ReadDynamicIntArray(string key)
        {
            if (Options.HasValue(key))
            {
                return ReadIntArray(key);
            }

            List<int> data = new();

            Console.WriteLine($"{FormatKeyString(key)}: ");

            string? id = null;
            while (true)
            {
                Console.Write("\t");
                id = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(id))
                {
                    break;
                }

                if (int.TryParse(id, out int value))
                {
                    data.Add(value);
                }
            }

            return data.ToArray();
        }
    }
}
