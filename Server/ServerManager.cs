using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Command;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Server
{
    public class ServerData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("specific_name")]
        public string SpecificName { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("build")]
        public string[] BuildInfo { get; set; }

        [JsonPropertyName("folder")]
        public string FolderPath { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime CreateAt { get; set; }

        public ServerData(string name, string provider, string[] buildInfo, string folderPath)
        {
            Name = name;
            SpecificName = $"{name.Replace(" ", "")}-{new Random().Next(9999999)}"; //random from 0 - 9,999,999
            Provider = provider;
            BuildInfo = buildInfo;
            FolderPath = Path.GetFullPath(folderPath);
            CreateAt = DateTime.UtcNow;
        }
    }

    public class ServerProcessData
    {
        [JsonPropertyName("pid")]
        public int ProcessId { get; set; } = -1;

        [JsonPropertyName("auto_restart")]
        public bool AutoRestart { get; set; } = false;
    }

    public class ServerManager
    {
        //staic

        public static string[] FindServer()
        {
            List<string> path = new();

            //current folder
            string local = Path.Join(Directory.GetCurrentDirectory(), ".wey", "config.json");
            if (File.Exists(local))
            {
                path.Add(local);
            }

            //in folder (depth=1)
            foreach (string itemName in StaticFolderController.Read(Directory.GetCurrentDirectory()).Folders)
            {
                string itemPath = Path.Join(itemName, ".wey", "config.json");

                if (File.Exists(itemPath))
                {
                    path.Add(itemPath);
                }
            }

            return path.ToArray();
        }

        public static string FindServer(string name)
        {
            foreach (string itemPath in FindServer())
            {
                if (GetServer(itemPath).Name == name)
                {
                    return itemPath;
                }
            }

            throw new DirectoryNotFoundException($"{Path.Join(Directory.GetCurrentDirectory(), name)} not found");
        }

        public static ServerData GetServer(string configPath)
        {
            ServerData? serverData = JsonSerializer.Deserialize<ServerData>(StaticFileController.Read(configPath));
            if (serverData == null) throw new NullReferenceException();

            return serverData;
        }

        public static ServerData[] GetServer(params string[] configPath)
        {
            return configPath.Select(GetServer).ToArray();
        }

        //class

        public ServerData Data { get; set; }
        public string DataPath { get; set; }

        private readonly FileController<ServerProcessData> ProcessData;

        public ServerManager(ServerData data)
        {
            Data = data;
            DataPath = Path.Join(data.FolderPath, ".wey");

            ProcessData = new(DataPath, "process_info");
        }

        public void AddServerFile(byte[] jarData)
        {
            Logger.Info($"Creating: {Data.Name}");

            StaticFileController.Build(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));

            StaticFileController.Build(Path.Join(Data.FolderPath, "eula.txt"), "eula=true");
            StaticFileController.Build(Path.Join(Data.FolderPath, "server.jar"), jarData);
        }

        public void Start(bool autoRestart = false)
        {
            if (CommandPrompt.IsProcessExists(ProcessData.ReadRequired().ProcessId)) Stop();

            Logger.Info($"Starting Server: {Data.Name} ({Data.Provider} {Data.BuildInfo[0]})");

            StaticFileController.Wait(Data.FolderPath, "server.jar");

            string? javaExecute = CommandPrompt.Where("java.exe");
            if (javaExecute == null) throw new FileNotFoundException("java.exe not found");

            Process? process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = javaExecute,
                        Arguments = "-jar server.jar --nogui",
                        WorkDirectory = Data.FolderPath
                    }
                );
            if (process == null) return;

            ProcessData.Edit((data) =>
            {
                data.ProcessId = process.Id;
                data.AutoRestart = autoRestart;
                return data;
            });

            if (autoRestart)
            {
                int delay = Config.Get().AutoRestartDelay;

                Logger.Info($"Auto Restarting is Enable: Every {delay}-{delay * 2} seconds");
                StartAutoRestart(delay);
            }
        }

        private void StartAutoRestart(int delay)
        {
            delay *= 1000;

            string? read = "Any";
            CancellationTokenSource cancellationSource = new();

            bool IsStop = false;
            while (!IsStop)
            {
                Thread.Sleep(delay);

                //input
                if (read != null)
                {
                    switch (read)
                    {
                        case "stop":
                            {
                                IsStop = true;
                                break;
                            }
                    }

                    cancellationSource.Cancel();
                }

                //script
                if (!cancellationSource.IsCancellationRequested)
                {
                    Logger.Info($"Auto Restart Checking: {Data.Name}");

                    if (!CommandPrompt.IsProcessExists(ProcessData.ReadRequired().ProcessId))
                    {
                        Logger.Info($"Auto Restart: {Data.Name}");
                        Start(false);
                    }
                }
                else
                {
                    read = null;
                    cancellationSource = new();
                    Task.Run(() => { read = System.Console.ReadLine(); }, cancellationSource.Token);
                }
            }

            Logger.Warn("Please use the stop command to ensure that everything is stopped");
            Stop();
        }

        public void Stop()
        {
            Logger.Info($"Stoping Server: {Data.Name}");

            int pid = ProcessData.ReadRequired().ProcessId;

            if (CommandPrompt.IsProcessExists(pid)) CommandPrompt.KillProcess(pid);

            ProcessData.Delete();
        }
    }
}
