using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;

namespace wey.Host
{
    class HostFinder
    {
        protected static string[] FindConfig()
        {
            List<string> path = new();

            string local = Path.Join(Directory.GetCurrentDirectory(), ".wey", "config.json"); //current folder

            string[] localList = StaticFolderController.Read(Directory.GetCurrentDirectory()).Folders; //in folder (depth=1)
            string[] serverLists = HostList.Get().ToArray(); //server list

            foreach (string itemPath in localList.Concat(serverLists).Append(local))
            {
                string configPath = Path.Join(itemPath, ".wey", "config.json");

                if (File.Exists(configPath))
                {
                    path.Add(configPath);
                }
            }

            return path.Distinct().ToArray();
        }

        public static HostData[] Find(string? name = null)
        {
            HostData[] hostList = FindConfig().Select(Get).ToArray();

            if (name == null)
            {
                return hostList;
            }
            else
            {
                List<HostData> servers = new();

                foreach (HostData? host in hostList)
                {
                    if (host == null) continue;

                    if (host.Name.Equals(name) || host.SpecificName.Equals(name))
                    {
                        servers.Add(host);
                    }
                }

                return servers.ToArray();
            }
        }

        public static HostData Get(string configPath)
        {
            HostData? host = JsonSerializer.Deserialize<HostData>(StaticFileController.Read(configPath));

            if (host == null) throw new HostNotFoundException();

            return host;
        }
    }

    class HostList
    {
        private static readonly JsonFileController<List<string>> File = new("servers.json");

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
