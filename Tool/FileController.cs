using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    class StaticFileController
    {
        public static void Build(string filePath, string data)
        {
            if (File.Exists(filePath)) return;

            StaticFolderController.Build(Path.GetDirectoryName(filePath));

            using StreamWriter writer = File.CreateText(filePath);
            writer.Write(data);
        }

        public static void Build(string filePath, byte[] data)
        {
            if (File.Exists(filePath)) return;

            StaticFolderController.Build(Path.GetDirectoryName(filePath));

            File.WriteAllBytes(filePath, data);
        }

        public static void Edit(string filePath, string data)
        {
            if (!File.Exists(filePath))
            {
                Build(filePath, data);
                return;
            }

            File.WriteAllText(filePath, data);
        }

        public static string Read(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            return File.ReadAllText(filePath);
        }

        public static void Wait(params string[] path)
        {
            string filePath = Path.Combine(path);

            Logger.Info($"Waiting for {filePath}");

            while (!File.Exists(filePath))
            {
                return;
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }
    }

    class StaticFolderController
    {
        public static void Build(params string[] path)
        {
            string folderPath = Path.Combine(path);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public class ReadFolderReturn
        {
            public string[] Folders;
            public string[] Files;

            public ReadFolderReturn(string[] folders, string[] files)
            {
                this.Folders = folders;
                this.Files = files;
            }
        }

        public static ReadFolderReturn Read(params string[] path)
        {
            string folderPath = Path.Combine(path);

            return new ReadFolderReturn(
                    Directory.GetDirectories(folderPath),
                    Directory.GetFiles(folderPath)
                );
        }

        public static void Wait(params string[] path)
        {
            string folderPath = Path.Combine(path);

            Logger.Info($"Waiting for {folderPath}");

            while (!Directory.Exists(folderPath))
            {
                return;
            }
        }
    }

    class FileController<T>
    {
        private static T CreateTypeInstance()
        {
            return Activator.CreateInstance<T>();
        }

        private static string Rename(string fileName)
        {
            if (Path.GetExtension(fileName) == "json")
            {
                return fileName;
            }

            return $"{Path.GetFileNameWithoutExtension(fileName)}.json";
        }

        // class

        private readonly string filePath;

        public FileController(string fileName)
        {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "@ThyoneMC", "wey", Rename(fileName));
        }

        public FileController(string filePath, string fileName)
        {
            this.filePath = Path.Combine(filePath, Rename(fileName));
        }

        public void Build()
        {
            StaticFileController.Build(filePath, JsonSerializer.Serialize(CreateTypeInstance()));
        }

        public void Edit(T data)
        {
            if (!File.Exists(filePath))
            {
                StaticFileController.Build(filePath, JsonSerializer.Serialize(data));
            }
            else
            {
                StaticFileController.Edit(filePath, JsonSerializer.Serialize(data));
            }
        }

        public void Edit(Func<T, T> callback)
        {
            Edit(callback(ReadRequired()));
        }

        public T? Read()
        {
            if (!File.Exists(filePath))
            {
                Build();
                return CreateTypeInstance();
            }

            return JsonSerializer.Deserialize<T>(StaticFileController.Read(filePath));
        }

        public T ReadRequired()
        {
            T? read = Read();
            if (read == null) throw new NullReferenceException();

            return read;
        }
        
        public void Delete()
        {
            StaticFileController.DeleteFile(filePath);
        }
    }
}
