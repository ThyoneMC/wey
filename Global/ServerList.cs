using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Tool;

namespace wey.Global
{
    class ServerList
    {
        private static readonly FileController<List<string>> File = new("servers");

        public static List<string> Get()
        {
            List<string>? read = File.Read();

            if (read == null) return new();

            return read;
        }

        public static void Add(string path)
        {
            File.Edit(list =>
            {
                list.Add(path);

                return list;
            });
        }

        public static void Remove(string path)
        {
            File.Edit(list =>
            {
                list.RemoveAll(list => list.Equals(path));

                return list;
            });
        }

        public static void Edit(Func<List<string>, List<string>> callback)
        {
            Set(callback(Get()));
        }

        public static void Set(List<string> data)
        {
            File.Edit(data);

            Logger.Log("servers saved");
        }
    }
}
