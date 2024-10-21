using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.CLI
{
    internal static class Options
    {
        static readonly Dictionary<string, string?> data = new();

        public static void Import(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                data.Add(name, null);
            }
            else
            {
                data.Add(name, value);
            }
        }

        public static void Import(string str)
        {
            string txt = str.Substring("--".Length);

            int equalIndex = txt.IndexOf("=");
            int notFound = -1;

            if (equalIndex == notFound)
            {
                Import(txt, null);
            }
            else
            {
                string content = txt.Substring(equalIndex + 1);

                Import(txt[..equalIndex], content);
            }
        }

        public static bool IsArgOptions(string str)
        {
            return str.StartsWith("--");
        }

        public static bool IsUsed(string name)
        {
            return data.ContainsKey(name);
        }

        public static bool HasValue(string name)
        {
            if (!IsUsed(name))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(Get(name));
        }

        public static string? Get(string name)
        {
            return data.GetValueOrDefault(name, null);
        }

        public static bool GetBool(string name, bool nonUsedOrDefaultValue)
        {
            string? value = Get(name);
            if (String.Equals(value, bool.TrueString, StringComparison.OrdinalIgnoreCase)) return true;
            if (String.Equals(value, bool.FalseString, StringComparison.OrdinalIgnoreCase)) return false;

            if (IsUsed(name))
            {
                return !nonUsedOrDefaultValue;
            }
            else
            {
                return nonUsedOrDefaultValue;
            }
        }
    }
}
