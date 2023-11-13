using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    public class ExecutablePlatform
    {
        private static readonly bool IsOsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public string Windows { get; set; } = "WINDOWS";
        public string Linux { get; set; } = "LINUX";

        public ExecutablePlatform(string windows, string linux)
        {
            this.Windows = windows;
            this.Linux = linux;
        }

        public string Get()
        {
            if (IsOsWindows)
            {
                return Windows;
            }

            return Linux;
        }
    }

    class Executable
    {
        public static string PathSplitter = new ExecutablePlatform(";", ":").Get();

        public static string? Where(string fileName, string variable = "PATH")
        {
            string? AllPath = Environment.GetEnvironmentVariable(variable);

            if (AllPath == null) return null;

            foreach (string folderPath in AllPath.Split(PathSplitter))
            {
                if (!Directory.Exists(folderPath)) continue;
                string[] folderFiles = StaticFolderController.Read(folderPath).Files;

                foreach (string filePath in folderFiles)
                {
                    if (Path.GetFileName(filePath).StartsWith(fileName))
                    {
                        return Path.GetFullPath(filePath);
                    }
                }
            }

            return null;
        }

        public static bool IsExists(int pid)
        {
            return Process.GetProcesses().Any(process => process.Id == pid);
        }

        public static void Kill(int? pid = -1)
        {
            if (pid == null || pid == -1) return;
            if (!IsExists((int)pid)) return;

            Kill(Process.GetProcessById((int)pid));
        }

        public static void Kill(Process? process)
        {
            if (process == null) return;

            try
            {
                if (!process.HasExited) process.Kill(true);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
