using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    class CommandPrompt
    {
        public class CommandPromptOptions
        {
            public string FileName { get; set; } = string.Empty;
            public string Arguments { get; set; } = string.Empty;
            public string WorkDirectory { get; set; } = Directory.GetCurrentDirectory();
        }

        public static string? Where(string fileName, string variable = "PATH")
        {
            string? AllPath = Environment.GetEnvironmentVariable(variable);
            if (AllPath == null) return null;

            foreach (string folderPath in AllPath.Split(";"))
            {
                if (!Directory.Exists(folderPath)) continue;
                string[] folderFiles = FileController.StaticReadFolder(folderPath).Files;

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

        public static void StaticExecute(CommandPromptOptions config)
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = config.FileName,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = config.Arguments,
                    WorkingDirectory = config.WorkDirectory
                }
            };

            process.Start();
            process.WaitForExit();
        }

        private Process process = new();

        public CommandPrompt(ProcessStartInfo startInfo)
        {
            process.StartInfo = startInfo;
        }

        public void Execute(params string[] args)
        {
            process.StartInfo.Arguments = string.Join(" ", args);

            process.Start();
            process.WaitForExit();
        }
    }
}
