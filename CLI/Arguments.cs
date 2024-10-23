using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.CLI
{
    internal static class Arguments
    {
        static readonly Queue<string> data = new();

        public static void Import(string name)
        {
            data.Enqueue(name);
        }

        public static bool IsUsed(string name)
        {
            return data.Contains(name);
        }

        public static string? Use()
        {
            if (data.Count == 0)
            {
                return null;
            }

            return data.Dequeue();
        }
    }
}
