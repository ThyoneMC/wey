using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Global
{
    class StaticFileController
    {
        public static void Build(string path, string data)
        {
            if (File.Exists(path)) return;

            StaticFolderController.Build(Path.GetDirectoryName(path));

            using StreamWriter writer = File.CreateText(path);
            writer.Write(data);
        }

        public static void Build(string path, byte[] data)
        {
            if (File.Exists(path)) return;

            StaticFolderController.Build(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, data);
        }

        public static void Edit(string path, string data)
        {
            if (!File.Exists(path))
            {
                Build(path, data);
                return;
            }

            File.WriteAllText(path, data);
        }

        public static string Read(string path)
        {
            if (!File.Exists(path)) return string.Empty;

            return File.ReadAllText(path);
        }

        public static void Wait(string path)
        {
            Logger.Info($"Waiting for {path}");

            while (true)
            {
                if (File.Exists(path)) break;
            }
        }

        public static void Delete(string path)
        {
            if (!File.Exists(path)) return;

            File.Delete(path);
        }
    }

    class StaticFolderController
    {
        public static string AppdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".thyonemc", "wey");

        public static string TemporaryPath = Path.Combine(Path.GetTempPath(), ".wey");

        static StaticFolderController()
        {
            if (!Directory.Exists(AppdataPath)) Build(AppdataPath);

            if (!Directory.Exists(TemporaryPath)) Build(TemporaryPath);
        }

        //#functions

        public static void Build(string path)
        {
            if (Directory.Exists(path)) return;

            Directory.CreateDirectory(path);
        }

        public class StaticFolderControllerRead
        {
            public string[] Folders { get; set; } = Array.Empty<string>();
            public string[] Files { get; set; } = Array.Empty<string>();
        }

        public static StaticFolderControllerRead Read(string path)
        {
            return new StaticFolderControllerRead()
            {
                Folders = Directory.GetDirectories(path),
                Files = Directory.GetFiles(path)
            };
        }

        public static void Wait(string path)
        {
            Logger.Info($"Waiting for {path}");

            while (true)
            {
                if (Directory.Exists(path)) break;
            }
        }

        public static void Copy(string sourcePath, string destinationPath)
        {
            StaticFolderControllerRead read = Read(sourcePath);

            Build(destinationPath);

            foreach (string file in read.Files)
            {
                File.Copy(file, Path.Join(destinationPath, Path.GetFileName(file)));
            }

            foreach (string folder in read.Folders)
            {
                string folderName = Path.GetFileName(folder);

                StaticFolderController.Copy(Path.Join(sourcePath, folderName), Path.Join(destinationPath, folderName));
            }
        }

        public static void Temporary(string path)
        {
            if (!Directory.Exists(path)) return;

            StaticFolderController.Copy(path, Path.Combine(TemporaryPath, $"{Path.GetFileName(path)}_{DateTime.UtcNow.ToFileTime()}".ToLower()));
            Delete(path);
        }

        public static void Delete(string path)
        {
            if (!Directory.Exists(path)) return;

            Directory.Delete(path, true);
        }
    }

    class FileControllerReadException : Exception
    {
        public FileControllerReadException(string? message = null) : base(message)
        {

        }
    }

    class FileController
    {
        protected readonly string FilePath;

        public FileController(string name)
        {
            FilePath = Path.Combine(StaticFolderController.AppdataPath, name);
        }

        public FileController(string path, string name)
        {
            this.FilePath = Path.Combine(path, name);
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public void Build(string data)
        {
            StaticFileController.Build(FilePath, data);
        }

        public void Edit(string data)
        {
            if (!Exists())
            {
                Build(data);
            }
            else
            {
                StaticFileController.Edit(FilePath, data);
            }
        }

        public void Edit(Func<string, string> callback)
        {
            Edit(callback(ReadRequired()));
        }

        public string? Read()
        {
            if (!Exists())
            {
                Build(string.Empty);
                return string.Empty;
            }

            return StaticFileController.Read(FilePath);
        }

        public string ReadRequired()
        {
            string? read = Read();
            if (read == null) throw new FileControllerReadException();

            return read;
        }

        public void Delete()
        {
            StaticFileController.Delete(FilePath);
        }
    }

    class JsonFileController<T> : FileController
    {
        public static T CreateTypeInstance()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)string.Empty;
            }

            return Activator.CreateInstance<T>();
        }

        public static string Rename(string name)
        {
            if (Path.GetExtension(name) == "json")
            {
                return name;
            }

            return $"{Path.GetFileNameWithoutExtension(name)}.json";
        }

        //class

        public JsonFileController(string name) : base(Rename(name))
        {

        }

        public JsonFileController(string path, string name) : base(path, Rename(name))
        {

        }

        public void Build()
        {
            Build(CreateTypeInstance());
        }

        public void Build(T data)
        {
            StaticFileController.Build(FilePath, JsonSerializer.Serialize(data));
        }

        public void Edit(T data)
        {
            if (!Exists())
            {
                Build();
            }
            else
            {
                StaticFileController.Edit(FilePath, JsonSerializer.Serialize(data));
            }
        }

        public void Edit(Func<T, T> callback)
        {
            Edit(callback(ReadRequired()));
        }

        public new T? Read()
        {
            if (!Exists())
            {
                Build();

                return CreateTypeInstance();
            }

            return JsonSerializer.Deserialize<T>(StaticFileController.Read(FilePath));
        }

        public new T ReadRequired()
        {
            T? read = Read();
            if (read == null) throw new FileControllerReadException();

            return read;
        }
    }
}
