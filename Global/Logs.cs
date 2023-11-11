using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Tool;

namespace wey.Global
{
    class Logs
    {
        private static readonly string FolderPath = Path.Join(StaticFolderController.AppdataPath, "logs");
        private static readonly string FilePath = Path.Join(FolderPath, $"{DateTime.UtcNow.ToLocalTime().ToFileTime()}.log");

        public static readonly StreamWriter File;

        static Logs()
        {
            StaticFolderController.Build(FolderPath);
            StaticFileController.Build(FilePath, string.Empty);

            File = new(FilePath);
        }

        public static string Get()
        {
            string? read = StaticFileController.Read(FilePath);

            if (read == null) return string.Empty;

            return read;
        }

        public static void Add(string data)
        {
            File.WriteLine(data);
            File.Flush();
        }

        public static void Delete()
        {
            File.Close();

            StaticFileController.Delete(FilePath);
        }
    }
}
