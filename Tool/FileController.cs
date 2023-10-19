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
    class FileController
    {
        private readonly string filePath;

        private static string Rename(string fileName)
        {
            if (Path.GetExtension(fileName) == "json")
            {
                return fileName;
            }

            return $"{Path.GetFileNameWithoutExtension(fileName)}.json";
        }

        public FileController(string fileName)
        {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "@ThyoneMC", "wey", Rename(fileName));
        }

        public FileController(string filePath, string fileName)
        {
            this.filePath = Path.Combine(filePath, Rename(fileName));
        }

        // class

        public void BuildFile<T>()
        {
            StaticBuildFile(filePath, JsonSerializer.Serialize(CreateTypeInstance<T>()));
        }

        public void EditFile<T>(T data)
        {
            if (!File.Exists(filePath))
            {
                StaticBuildFile(filePath, JsonSerializer.Serialize(data));
            }
            else
            {
                StaticEditFile(filePath, JsonSerializer.Serialize(data));
            }
        }

        public T? ReadFile<T>()
        {
            if (!File.Exists(filePath))
            {
                this.BuildFile<T>();
                return CreateTypeInstance<T>();
            }

            return JsonSerializer.Deserialize<T>(StaticReadFile(filePath));
        }

        // static

        private static T CreateTypeInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        // folder

        public static void StaticBuildFolder(params string[] path)
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

        public static ReadFolderReturn StaticReadFolder(params string[] path)
        {
            string folderPath = Path.Combine(path);

            return new ReadFolderReturn(
                    Directory.GetDirectories(folderPath),
                    Directory.GetFiles(folderPath)
                );
        }

        public static void StaticWaitFolder(params string[] path)
        {
            string folderPath = Path.Combine(path);

            Logger.Info($"Waiting for {folderPath}");

            while (!Directory.Exists(folderPath))
            {
                return;
            }
        }

        // file

        public static void StaticBuildFile(string filePath, string data)
        {
            if (File.Exists(filePath)) return;

            StaticBuildFolder(Path.GetDirectoryName(filePath));

            using StreamWriter writer = File.CreateText(filePath);
            writer.Write(data);
        }

        public static void StaticEditFile(string filePath, string data)
        {
            if (!File.Exists(filePath))
            {
                StaticBuildFile(filePath, data);
                return;
            }

            File.WriteAllText(filePath, data);
        }

        public static string StaticReadFile(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            return File.ReadAllText(filePath);
        }

        public static void StaticWaitFile(params string[] path)
        {
            string filePath = Path.Combine(path);

            Logger.Info($"Waiting for {filePath}");

            while (!File.Exists(filePath))
            {
                return;
            }
        }

        public static void StaticDeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }

        // byte

        public static void StaticBuildByte(string filePath, byte[] data)
        {
            if (File.Exists(filePath)) return;

            StaticBuildFolder(Path.GetDirectoryName(filePath));

            File.WriteAllBytes(filePath, data);
        }
    }
}
