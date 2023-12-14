using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Global
{
    public class ExecutablePlatformNotFoundException : Exception
    {
    }

    public class ExecutablePlatform
    {
        public static readonly bool IsOsWindows;
        public static readonly bool IsOsLinux;

        static ExecutablePlatform()
        {
            IsOsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsOsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            throw new ExecutablePlatformNotFoundException();
        }

        public static string Get(string windows, string linux)
        {
            if (IsOsWindows) return windows;
            if (IsOsLinux) return linux;

            return string.Empty;
        }
    }

    class ExecutableOption
    {
        public string FileName { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public string WorkDirectory { get; set; } = Directory.GetCurrentDirectory();
        public int OutputSaved { get; set; } = 25;
    }

    class Executable : Process
    {
        public readonly Queue<string> Output = new();

        public readonly bool IsStarted = false;

        public Executable(ExecutableOption option)
        {
            StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,

                FileName = option.FileName,
                WorkingDirectory = option.WorkDirectory,
                Arguments = option.Arguments,
            };

            OutputDataReceived += new DataReceivedEventHandler((_sender, thisEvent) =>
            {
                if (string.IsNullOrEmpty(thisEvent.Data)) return;

                Output.Enqueue(thisEvent.Data);
                while (Output.Count > option.OutputSaved) Output.Dequeue();
            });

            IsStarted = IsExists();
        }

        public bool IsExists()
        {
            if (!IsStarted) return false;

            if (HasExited) return false;

            return GetProcesses().Any(process => process.Id == Id);
        }

        public new void Start()
        {
            if (IsStarted) return;

            base.Start();

            Logger.WriteSingle($"STARTING -->");

            BeginOutputReadLine();
            StandardInput.AutoFlush = true;
        }

        public new void Kill()
        {
            if (!IsExists()) return;

            Kill(true);
            WaitForExit();
        }

        public string[] GetOutput()
        {
            return Output.ToArray();
        }
    }
}
