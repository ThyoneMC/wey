using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    public class CommandPromptOptions
    {
        public string FileName { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public string WorkDirectory { get; set; } = Directory.GetCurrentDirectory();
        public ProcessWindowStyle WindowStyle { get; set; } = ProcessWindowStyle.Normal;
    }

    class CommandPrompt
    {
        public static Process? StaticExecute(CommandPromptOptions config)
        {
            ProcessStartInfo StartInfo = new()
            {
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = config.WindowStyle,

                FileName = config.FileName,
                Arguments = config.Arguments,
                WorkingDirectory = config.WorkDirectory
            };

            return Process.Start(StartInfo);
        }

        private readonly Process process = new();

        public CommandPrompt(ProcessStartInfo startInfo)
        {
            process.StartInfo = startInfo;
        }

        public Process Execute(params string[] args)
        {
            process.StartInfo.Arguments = string.Join(" ", args);

            process.Start();
            process.WaitForExit();

            return process;
        }
    }
}
