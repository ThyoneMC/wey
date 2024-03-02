using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Interface;

namespace wey.Forwarding
{
    class HamachiData
    {
        public bool IsRunning { get; set; } = false;

        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Total { get; set; } = 0;
    }

    class Hamachi : IForwarding
    {
        private readonly HostData HostData;

        public JsonFileController<HamachiData> hamachi;

        private readonly ExecutableInstance Client = new(new ExecutableInstanceOption());

        public Hamachi(HostData host) : base("hamachi")
        {
            HostData = host;

            Client.baseArguments = "--cli";
            Client.startInfo.FileName = FilePath;

            hamachi = new(Path.Join(HostData.FolderPath, ".wey", "forwarding"), "hamachi.json");
            if (!hamachi.Exists())
            {
                hamachi.Build(new HamachiData()
                {
                    Name = HostData.SpecificName,
                    Password = HostData.CreateAt.ToFileTimeUtc().ToString()
                });
            }
        }

        public new void Register(string path)
        {
            base.Register(path);

            Client.startInfo.FileName = path;
        }

        public new void Unregister()
        {
            base.Unregister();

            Client.startInfo.FileName = string.Empty;
        }

        public void Create()
        {
            HamachiData read = hamachi.ReadRequired();

            string name = $"{read.Name}-{read.Total + 1}";

            Logger.Log($"Create Hamachi Tunnel: {name}");

            Client.Execute("create", name, read.Password);
            Client.Execute("go-offline", name);

            read.Total += 1;
            hamachi.Edit(read);
        }

        public void Delete(int count = 1)
        {
            HamachiData read = hamachi.ReadRequired();

            count = Math.Min(count, read.Total);
            for (int i = 0; i < count; i++)
            {
                string name = $"{read.Name}-{read.Total - i}";

                Logger.Log($"Delete Hamachi Tunnel: {name}");

                Client.Execute("delete", name);
            }

            read.Total -= count;
            hamachi.Edit(read);
        }

        public void Start()
        {
            HamachiData read = hamachi.ReadRequired();
            if (read.IsRunning || read.Total == 0) return;

            Logger.Info($"Opening Hamachi Tunnel");

            Client.Execute("login");

            for (int i = 1; i <= read.Total; i++)
            {
                Client.Execute("go-online", $"{read.Name}-{i}");
            }

            read.IsRunning = true;
            hamachi.Edit(read);
        }

        public void Stop()
        {
            HamachiData read = hamachi.ReadRequired();
            if (!read.IsRunning || read.Total == 0) return;

            Logger.Info($"Closing Hamachi Tunnel");

            for (int i = 1; i <= read.Total; i++)
            {
                Client.Execute("go-offline", $"{read.Name}-{i}");
            }

            read.IsRunning = false;
            hamachi.Edit(read);
        }

        public void Exit()
        {
            Client.Execute("logoff", HostData.SpecificName);
        }
    }
}
