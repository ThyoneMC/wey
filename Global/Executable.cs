using Microsoft.VisualBasic.FileIO;
using SharpHook.Native;
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
    public class ExecutableArgument
    {
        public static List<string> Arguments { get; protected set; } = new();

        public static Dictionary<string, string?> Flags { get; protected set; } = new();

        public static void Import(params string[] args)
        {
            foreach (string text in args)
            {
                // args
                if (!text.StartsWith("--"))
                {
                    Arguments.Add(text);
                    continue;
                }

                // flag
                int indexOfEqualSign = text.IndexOf("=");
                if (indexOfEqualSign == -1)
                {
                    Flags.Add(text, string.Empty);
                    continue;
                }

                // flag=
                string content = text[(indexOfEqualSign + "=".Length)..];
                if (string.IsNullOrWhiteSpace(content))
                {
                    Flags.Add(text[..indexOfEqualSign], string.Empty);
                    continue;
                }

                // flag=content
                Flags.Add(text[..indexOfEqualSign], content);
            }
        }
    }

    public class ExecutablePlatformNotFoundException : Exception
    {
        public ExecutablePlatformNotFoundException(string? message = null) : base(message)
        {

        }
    }

    public class ExecutablePlatform
    {
        public static readonly bool IsOsWindows;
        public static readonly bool IsOsLinux;

        static ExecutablePlatform()
        {
            IsOsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsOsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if(!IsOsWindows && !IsOsLinux) throw new ExecutablePlatformNotFoundException();
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

    public class ExecutableNotFoundException : Exception
    {
        public ExecutableNotFoundException(string? message = null) : base(message)
        {

        }
    }

    class Executable : Process
    {
        public readonly Queue<string> Output = new();

        public bool IsStarted = false;

        private string? OnceOutput = null;

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

                Array.ForEach(thisEvent.Data.Split('\n'), output => Output.Enqueue(output));
                while (Output.Count > option.OutputSaved) Output.Dequeue();

                OnceOutput = Output.Last();
            });

            IsStarted = IsExists();
        }

        protected static Dictionary<int, Executable> ExecutableList = new();

        public static int Count => ExecutableList.Count;

        public int Export()
        {
            ExecutableList.Add(Id, this);

            return Id;
        }

        public static Executable? Import(int pid)
        {
            return ExecutableList[pid];
        }

        private static readonly string PathSplitter = ExecutablePlatform.Get(windows: ";", linux: ":");
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
            IsStarted = true;

            BeginOutputReadLine();
            StandardInput.AutoFlush = true;
        }

        public new void Kill()
        {
            if (!IsExists()) return;

            Kill(true);
            WaitForExit();

            ExecutableList.Remove(Id);
        }

        public string[] GetOutput()
        {
            return Output.ToArray();
        }

        public string? GetOnceOutput()
        {
            if (OnceOutput != null)
            {
                string tempOnceOutput = OnceOutput;
                OnceOutput = null;

                return tempOnceOutput;
            }

            return null;
        }

        public void Input(string input)
        {
            StandardInput.WriteLine(input);
        }
    }
}
