using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Command;
using wey.Console;

namespace wey.Tool
{
    public static class ServerProvider
    {
        public const string Vanilla = "vanilla";
        public const string PaperMC = "paper";
        public const string FabricMC = "fabric";
    }

    public class ServerData
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string[] BuildInfo { get; set; }
        public string FolderPath { get; set; }

        public ServerData(string name, string provider, string[] buildInfo, string folderPath)
        {
            this.Name = name;
            this.Provider = provider;
            this.BuildInfo = buildInfo;
            this.FolderPath = Path.GetFullPath(folderPath);
        }
    }

    public class ServerManager
    {
        public ServerData Data { get; set; } 
        public string DataPath { get; set; }

        public ServerManager(ServerData data)
        {
            this.Data = data;

            this.DataPath = Path.Join(data.FolderPath, ".wey");
        }

        //server

        public void AddServerFile(byte[] jarData)
        {
            Logger.Info($"Creating: {Data.Name}");

            FileController.StaticBuildFile(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));

            FileController.StaticBuildFile(Path.Join(Data.FolderPath, "eula.txt"), "eula=true");
            FileController.StaticBuildByte(Path.Join(Data.FolderPath, "server.jar"), jarData);
        }

        public void StartServer()
        {
            Logger.Info($"Starting: {Data.Name}");

            FileController.StaticWaitFile(Data.FolderPath, "server.jar");

            string? javaExecute = CommandPrompt.Where("java.exe");
            if (javaExecute == null) throw new FileNotFoundException("java.exe not found");

            CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = javaExecute,
                        Arguments = "-jar server.jar --nogui",
                        WorkDirectory = Data.FolderPath
                    }
                );
        }

        //tunnel ngrok

        public List<int> NgrokOpening = new();

        public void StartNgrokTunnel(int port = 25565)
        {
            Logger.Info("Starting Ngrok Tunnel");

            string Ngrok_Path = Config.Get().Ngrok;

            Process? Ngrok_Process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = Ngrok_Path,
                        Arguments = $"tcp {port}"
                    }
                );
            if (Ngrok_Process != null) NgrokOpening.Add(Ngrok_Process.Id);

            Logger.Info($"Opening Ngrok Tunnel with [port: {port}]");
        }

        public void StopNgrokTunnel()
        {
            foreach (int pid in NgrokOpening)
            {
                Process process = Process.GetProcessById(pid);

                if (!process.HasExited) process.Kill();
            }
        }

        //tunnel playit

        public List<int> PlayItOpening = new();

        public void StartPlayItTunnel()
        {
            Logger.Info("Starting PlayIt Tunnel");

            string PlayIt_Path = Config.Get().PlayIt;

            Process? PlayIt_Process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = PlayIt_Path
                    }
                );
            if (PlayIt_Process != null) PlayItOpening.Add(PlayIt_Process.Id);
        }

        public void StopPlayItTunnel()
        {
            foreach (int pid in PlayItOpening)
            {
                Process process = Process.GetProcessById(pid);

                if (!process.HasExited) process.Kill();
            }
        }

        //tunnel hamachi

        public IDictionary<string, string> HamachiOpening = new Dictionary<string, string>();

        public void StartHamachiTunnel()
        {
            Logger.Info("Starting Hamachi Tunnel");
            Random random = new();
            string name = $"{Data.Name.Replace(" ", "")}-{random.Next(9999999)}"; //random from 0 - 9,999,999

            string Hamachi_Path = Config.Get().Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            Hamachi_Command.Execute("--cli", "logon");

            // random from 0 - 999,999
            int password = random.Next(999999);

            Hamachi_Command.Execute("--cli", "create", name, password.ToString());
            HamachiOpening.Add(name, "create");

            Logger.Info($"Opening Hamachi Tunnel with [name: {name}, password: {password}]");
        }

        public void StopHamachiTunnel(string name)
        {
            string Hamachi_Path = Config.Get().Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            Hamachi_Command.Execute("--cli", "delete", name);
            HamachiOpening.Remove(name);
        }

        // tunnel

        public void StartTunnel()
        {
            ConfigData config = Config.Get();

            if (!string.IsNullOrEmpty(config.Ngrok)) StartNgrokTunnel();
            if (!string.IsNullOrEmpty(config.PlayIt)) StartPlayItTunnel();
            if (!string.IsNullOrEmpty(config.Hamachi)) StartHamachiTunnel();
        }
    }
}
