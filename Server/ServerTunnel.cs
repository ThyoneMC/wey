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

namespace wey.Server
{
    class ServerTunnel
    {
        //ngrok

        public static int StartNgrok(string path, int port = 25565)
        {
            Logger.Info("Starting Ngrok Tunnel");

            Process? Ngrok_Process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = path,
                        Arguments = $"tcp {port}"
                    }
                );

            if (Ngrok_Process != null)
            {
                Logger.Info($"Opening Ngrok Tunnel with [port: {port}]");
                return Ngrok_Process.Id;
            }

            return -1;
        }

        //playit

        public static int StartPlayIt(string path)
        {
            Logger.Info("Starting PlayIt Tunnel");

            Process? PlayIt_Process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = path
                    }
                );

            if (PlayIt_Process != null) return PlayIt_Process.Id;

            return -1;
        }

        //hamachi

        public static string StartHamachi(string path, string name, string password, bool isCreated = false)
        {
            Logger.Info("Starting Hamachi Tunnel");

            CommandPrompt Hamachi_Command = new(
                    new ProcessStartInfo()
                    {
                        FileName = path
                    }
                );

            Hamachi_Command.Execute("--cli", "logon");

            if (isCreated)
            {
                Hamachi_Command.Execute("--cli", "go-online", name);
            }
            else
            {
                Hamachi_Command.Execute("--cli", "create", name, password);
            }

            Logger.Info($"Opening Hamachi Tunnel with [name: {name}, password: {password}]");
            return name;
        }

        public static void StopHamachi(string name)
        {
            string Hamachi_Path = Config.Get().Tunnel.Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            Hamachi_Command.Execute("--cli", "go-offline", name);
        }

        // class

        public class TunnelOpeningData
        {
            public int Ngrok { get; set; } = -1;
            public int PlayIt { get; set; } = -1;
            public string Hamachi { get; set; } = string.Empty;
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
        }

        public void Start()
        {
            ConfigData config = Config.Get();
            TunnelOpeningData openingData = TunnelOpeningData.Get(DataPath);

            if (!string.IsNullOrEmpty(config.Tunnel.Ngrok) && !CommandPrompt.IsProcessExists(openingData.Ngrok)) openingData.Ngrok = StartNgrok(config.Tunnel.Ngrok);

            if (!string.IsNullOrEmpty(config.Tunnel.PlayIt) && !CommandPrompt.IsProcessExists(openingData.PlayIt)) openingData.PlayIt = StartPlayIt(config.Tunnel.PlayIt);

            int password = Math.Max(Data.CreateAt.Millisecond, Data.CreateAt.Second) + 111;
            if (!string.IsNullOrEmpty(config.Tunnel.Hamachi))
            {
                openingData.Hamachi = StartHamachi(config.Tunnel.Hamachi, Data.SpecificName, password.ToString(), openingData.IsHamachiCreated);
                openingData.IsHamachiCreated = true;
            }

            StaticFileController.Edit(DataPath, JsonSerializer.Serialize(openingData));
        }

        public void Stop()
        {
            TunnelOpeningData openingData = TunnelOpeningData.Get(DataPath);

            if (CommandPrompt.IsProcessExists(openingData.Ngrok)) CommandPrompt.KillProcess(openingData.Ngrok);
            if (CommandPrompt.IsProcessExists(openingData.PlayIt)) CommandPrompt.KillProcess(openingData.PlayIt);
            if (!string.IsNullOrEmpty(openingData.Hamachi)) StopHamachi(openingData.Hamachi);
        }
    }
}
