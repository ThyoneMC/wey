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
using wey.Core;
using wey.Tool;

namespace wey.Server
{
    public class ServerData
    {
        public string Name { get; set; }
        public string SpecificName { get; set; }
        public string Provider { get; set; }
        public string[] BuildInfo { get; set; }
        public string FolderPath { get; set; }
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
            foreach (string itemName in FileController.StaticReadFolder(Directory.GetCurrentDirectory()).Folders)
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
            ServerData? serverData = JsonSerializer.Deserialize<ServerData>(FileController.StaticReadFile(configPath));
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

        public ServerManager(ServerData data)
        {
            Data = data;

            DataPath = Path.Join(data.FolderPath, ".wey");
        }

        public void AddServerFile(byte[] jarData)
        {
            Logger.Info($"Creating: {Data.Name}");

            FileController.StaticBuildFile(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));

            FileController.StaticBuildFile(Path.Join(Data.FolderPath, "eula.txt"), "eula=true");
            FileController.StaticBuildByte(Path.Join(Data.FolderPath, "server.jar"), jarData);
        }

        public int GetServerPID()
        {
            string read = FileController.StaticReadFile(Path.Join(DataPath, "pid"));

            if (!int.TryParse(read, out int pid)) return -1;

            return pid;
        }

        public void Start()
        {
            if (CommandPrompt.IsProcessExists(GetServerPID())) Stop();

            Logger.Info($"Starting Server: {Data.Name}");

            FileController.StaticWaitFile(Data.FolderPath, "server.jar");

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

            FileController.StaticEditFile(Path.Join(DataPath, "pid"), process.Id.ToString());
        }

        public void Stop()
        {
            Logger.Info($"Stoping Server: {Data.Name}");

            int pid = GetServerPID();

            if (!CommandPrompt.IsProcessExists(pid)) CommandPrompt.KillProcess(pid);

            FileController.StaticDeleteFile(Path.Join(DataPath, "pid"));
        }
    }
}
