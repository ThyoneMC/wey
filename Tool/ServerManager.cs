using System;
using System.Collections.Generic;
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

        public void AddServer(byte[] jarData)
        {
            Logger.Info($"Creating: {Data.Name}");

            FileController.StaticBuildFile(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));

            FileController.StaticBuildFile(Path.Join(Data.FolderPath, "eula.txt"), "eula=true");
            FileController.StaticBuildByte(Path.Join(Data.FolderPath, "server.jar"), jarData);
        }

        public void Start()
        {
            Logger.Info($"Starting: {Data.Name}");

            FileController.StaticWaitFile(Data.FolderPath, "server.jar");

            CommandPrompt.Execute(
                    new CommandPrompt.CommandPromptOptions()
                    {
                        FileName = $"java.exe",
                        Arguments = $"-jar server.jar",
                        WorkDirectory = Data.FolderPath
                    }
                );
        }
    }
}
