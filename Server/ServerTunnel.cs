using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;
using wey.Tool;
using System.Security.Cryptography;
using wey.Tunnel;

namespace wey.Server
{
    class ServerTunnel
    {
        //tunnel

        public Ngrok Ngrok;
        public PlayIt PlayIt;
        public Hamachi Hamachi;

        //class

        public class TunnelOpeningData
        {
            public int Ngrok { get; set; } = -1;
            public int PlayIt { get; set; } = -1;
            public bool IsHamachiCreated { get; set; } = false;

            public static TunnelOpeningData Get(string path)
            {
                string read = StaticFileController.Read(path);
                if (string.IsNullOrEmpty(read)) return new();

                TunnelOpeningData? openingData = JsonSerializer.Deserialize<TunnelOpeningData>(read);
                if (openingData == null) return new();

                return openingData;
            }
        }

        private readonly ServerData Data;
        private readonly string DataPath;

        public ServerTunnel(ServerData data)
        {
            Data = data;
            DataPath = Path.Join(data.FolderPath, ".wey", "tunnel.json");

            ConfigData config = Config.Get();
            TunnelOpeningData openingData = TunnelOpeningData.Get(DataPath);

            Ngrok.Import(openingData.Ngrok);
            Ngrok = new Ngrok(config.Tunnel.Ngrok, config.ServerPort);

            PlayIt.Import(openingData.PlayIt);
            PlayIt = new PlayIt(config.Tunnel.PlayIt);

            Hamachi = new Hamachi(config.Tunnel.Hamachi, new HamachiTunnelData()
            {
                Name = data.SpecificName,
                Password = (Math.Max(Data.CreateAt.Millisecond, Data.CreateAt.Second) + 111).ToString(),
                IsCreated = openingData.IsHamachiCreated
            });
        }

        public void Start()
        {
            TunnelOpeningData openingData = TunnelOpeningData.Get(DataPath);

            openingData.Ngrok = Ngrok.Start();

            openingData.PlayIt = PlayIt.Start();

            Hamachi.Start();
            openingData.IsHamachiCreated = Hamachi.Tunnel.IsCreated;

            StaticFileController.Edit(DataPath, JsonSerializer.Serialize(openingData));
        }

        public void Stop()
        {
            Ngrok.Stop();
            
            PlayIt.Stop();

            Hamachi.Stop();
        }
    }
}
