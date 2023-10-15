using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void Execute(CommandPromptOptions config)
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
    }
}
