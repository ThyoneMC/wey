using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Forwarding;
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

        [JsonPropertyName("jar_flags")]
        public string ServerJarFlags { get; set; } = "--nogui";

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
            IProviderBuild build = JsonEncryption.Decrypt<IProviderBuild>(Build);

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
        public readonly string DataPath;

        private readonly JsonFileController<int> ProcessID;

        public Executable? Process = null;

        public HostManager(HostData data)
        {
            Data = data;
            DataPath = Path.Join(data.FolderPath, ".wey");

            ProcessID = new(DataPath, "pid");

            int ReadPID = ProcessID.Read();
            if (ReadPID > 0)
            {
                Executable? ImportProcess = Executable.Import(ReadPID);

                if (ImportProcess != null)
                {
                    Process = ImportProcess;
                }
                else
                {
                    ProcessID.Delete();
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

            if (temporary) StaticFolderController.Copy(Data.FolderPath, Path.Combine(StaticFolderController.TemporaryPath, "servers", Data.SpecificName));
            StaticFolderController.Delete(Data.FolderPath);

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

            int port = int.Parse(new HostProperties(Data).Get("server-port") ?? "25565");

            Process = new(new ExecutableOption()
            {
                FileName = javaExecute,
                Arguments = $"-jar server.jar --port={port} {Data.ServerJarFlags}",
                WorkDirectory = Data.FolderPath
            });

            Process.Start();
            ProcessID.Edit(Process.Export());

            new Ngrok().Start(port);
            new Hamachi(Data).Start();
        }

        public void Stop()
        {
            if (Process == null) return;

            Logger.Info($"Stoping Server: {Data.Name}");

            new Ngrok().Stop();
            new Hamachi(Data).Stop();

            Process.Input("stop");
            ProcessID.Delete();

            Process.WaitForExit();
            Executable.RemoveExport(Process.Id);
        }
    }
}
