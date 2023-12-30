using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Interface;
using wey.Pages;

namespace wey.Host
{
    public class HostData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "default";

        [JsonPropertyName("specific_name")]
        public string SpecificName { get; set; } = $"default{DateTime.UtcNow.ToFileTime()}";

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = string.Empty;

        [JsonPropertyName("build")]
        public string Build { get; set; } = string.Empty;

        [JsonPropertyName("folder")]
        public string FolderPath { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("pid")]
        public int ProcessID { get; set; } = -1;

        public HostData(string name, string provider, string folderPath)
        {
            Name = name;
            SpecificName = $"{name.Replace(" ", "")}{new Random().Next(9999999)}"; //random from 0 - 9,999,999
            Provider = provider;
            FolderPath = Path.GetFullPath(folderPath);
        }

        public HostData()
        {

        }

        public string GetVersion()
        {
            IProviderBuild build = JsonEncryption<IProviderBuild>.Decrypt(Build);

            return build.Version;
        }
    }

    class HostNotFoundException : Exception
    {
        public HostNotFoundException(string? message = null) : base($"host not found: {message}")
        {

        }
    }

    class HostManager
    {
        public HostData Data;
        public string DataPath;

        public Executable? Process = null;

        public HostManager(HostData data)
        {
            Data = data;
            DataPath = Path.Join(data.FolderPath, ".wey");

            if (Data.ProcessID != -1)
            {
                Executable? ImportProcess = Executable.Import(Data.ProcessID);

                if (ImportProcess != null)
                {
                    Process = ImportProcess;
                    return;
                }
            }
        }

        public void Create()
        {
            Logger.Info($"Creating: {Data.Name}");

            StaticFileController.Build(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));
            StaticFileController.Build(Path.Join(Data.FolderPath, "eula.txt"), "eula=true");

            HostList.Add(Data.FolderPath);
        }

        public void Delete(bool temporary = true)
        {
            Logger.Info($"Deleting: {Data.Name}");

            if (temporary)
            {
                StaticFolderController.Temporary(Data.FolderPath);
            }
            else
            {
                StaticFolderController.Delete(Data.FolderPath);
            }

            HostList.Remove(Data.FolderPath);
        }

        public void AddServerFile(byte[] jarData)
        {
            StaticFileController.Delete(Path.Join(Data.FolderPath, "server.jar"));
            StaticFileController.Build(Path.Join(Data.FolderPath, "server.jar"), jarData);
        }

        public void AddServerFile(IProviderDownload downloadData)
        {
            this.Data.Build = downloadData.Build;
            StaticFileController.Edit(Path.Join(DataPath, "config.json"), JsonSerializer.Serialize(Data));

            AddServerFile(downloadData.ServerJar);
        }

        public void Start()
        {
            if (Process != null && Process.IsExists()) return;

            Logger.Info($"Starting Server: {Data.Name} ({Data.Provider} {Data.GetVersion()})");

            StaticFileController.Wait(Path.Join(Data.FolderPath, "server.jar"));

            string javaExecuteName = ExecutablePlatform.Get(windows: "java.exe", linux: "java");

            string? javaExecute = Executable.Where(javaExecuteName);
            if (javaExecute == null) throw new FileNotFoundException($"{javaExecuteName} not found");

            Process = new(new ExecutableOption()
            {
                FileName = javaExecute,
                Arguments = "-jar server.jar --nogui",
                WorkDirectory = Data.FolderPath
            });

            Process.Start();
            Data.ProcessID = Process.Export();
        }

        public void Stop()
        {
            if (Process == null) return;

            Logger.Info($"Stoping Server: {Data.Name}");

            Process.Input("stop");
        }
    }
}
