using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Total { get; set; } = 0;
    }

    class Hamachi : IForwarding
    {
        private readonly HostData HostData;

        public static ForwardingState State { get; private set; } = ForwardingState.Stopped;

        public JsonFileController<HamachiData> hamachi;

        public Hamachi(HostData host) : base("hamachi")
        {
            HostData = host;

            Client.baseArguments = "--cli";

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

        public void Create()
        {
            HamachiData read = hamachi.ReadRequired();

            string name = $"{read.Name}-{read.Total + 1}";

            Logger.Info($"Create Hamachi Tunnel: {name}");

            Client.Execute("create", name, read.Password);

            hamachi.Edit(data =>
            {
                data.Total += 1;

                return data;
            });
        }

        public void Delete()
        {
            HamachiData read = hamachi.ReadRequired();

            for (int i = 1; i <= read.Total; i++)
            {
                Client.Execute("go-offline", $"{read.Name}-{i}");
            }
        }

        public void Start()
        {
            if (State == ForwardingState.Running) return;

            Logger.Info($"Opening Hamachi Tunnel");

            HamachiData read = hamachi.ReadRequired();

            Client.Execute("login");

            if (read.Total == 0)
            {
                Create();
                read.Total += 1;
            }

            for (int i = 1; i <= read.Total; i++)
            {
                Client.Execute("go-online", $"{read.Name}-{i}");
            }

            State = ForwardingState.Running;
        }

        public void Stop()
        {
            if (State == ForwardingState.Stopped) return;

            HamachiData read = hamachi.ReadRequired();

            for (int i = 1; i <= read.Total; i++)
            {
                Client.Execute("go-offline", $"{read.Name}-{i}");
            }

            State = ForwardingState.Stopped;
        }

        public void Exit()
        {
            Client.Execute("logoff", HostData.SpecificName);
        }
    }
}
