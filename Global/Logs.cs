using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace wey.Global
{
    class Logs
    {
        protected static readonly List<string> Data = new();

        protected static readonly StreamWriter File;

        static Logs()
        {
            string FilePath = Path.Join(StaticFolderController.AppdataPath, "logs", $"{DateTime.UtcNow.ToLocalTime().ToFileTime()}.log");

            StaticFileController.Build(FilePath, string.Empty);

            File = new(FilePath)
            {
                AutoFlush = true
            };
        }

        public static void Add(string data)
        {
            File.WriteLine(data);
        }

        public static void Stop()
        {
            File.Close();
        }
    }
}
