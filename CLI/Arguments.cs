using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.CLI
{
    internal static class Arguments
    {
        static readonly List<string> data = new();

        public static void Import(string name)
        {
            data.Add(name);
        }

        public static bool IsUsed(string name)
        {
            return data.Contains(name);
        }

        // return -1 when not found otherwise return position of args
        public static int Get(string name)
        {
            if (!IsUsed(name))
            {
                return -1;
            }

            return data.IndexOf(name) + 1;
        }
    }
}
