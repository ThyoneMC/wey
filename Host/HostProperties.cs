using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;

namespace wey.Host
{
    class HostProperties
    {
        private readonly FileController Properties;

        public HostProperties(HostData data)
        {
            Properties = new(data.FolderPath, "server.properties");
        }

        public Dictionary<string, string>? Read()
        {
            if (!Properties.Exists()) return null;
            string[] read = Properties.ReadRequired().Split("\n");

            Dictionary<string, string> data = new();

            foreach (string line in read)
            {
                if (line.StartsWith('#')) continue;

                string[] content = line.Split('=');

                if (content.Length == 2)
                {
                    data.Add(content[0], content[1].Replace("\r", string.Empty));
                }
            }

            Logger.WriteSingle(data);

            return data;
        }

        public string? Get(string key)
        {
            Dictionary<string, string>? data = Read();
            if (data == null) return null;

            if (data.TryGetValue(key, out string? value))
            {
                return value;
            }

            return null;
        }

        public void Set(string key, string value)
        {
            Dictionary<string, string>? data = Read();
            if (data == null) return;

            data[key] = value;

            Write(data);
        }

        protected void Write(Dictionary<string, string> data)
        {
            StringBuilder content = new();

            content.AppendLine("#Minecraft server properties");
            content.AppendLine($"#{DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzz yyyy", new CultureInfo("en-US"))}");

            foreach (string key in data.Keys)
            {
                content.AppendLine($"{key}={data.GetValueOrDefault(key, string.Empty)}");
            }

            if (Properties.Exists())
            {
                Properties.Edit(content.ToString());
            }
            else
            {
                Properties.Build(content.ToString());
            }
        }
    }
}
