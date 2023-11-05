using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Tunnel
{
    class PlayIt : TunnelBase
    {
        public static Process? process = null;

        public static void Import(int pid)
        {
            if (!CommandPrompt.IsProcessExists(pid)) return;

            process = Process.GetProcessById(pid);
        }

        public PlayIt(string path) : base(path)
        {
            
        }

        public override string GetName()
        {
            return "playit.gg";
        }

        public override int Start()
        {
            if (string.IsNullOrEmpty(Path)) return -1;

            Logger.Info("Starting PlayIt Tunnel");

            process = CommandPrompt.StaticExecute(
                    new CommandPromptOptions()
                    {
                        FileName = Path,
                        WindowStyle = ProcessWindowStyle.Minimized
                    }
                );
            if (process == null) return -1;

            return process.Id;
        }

        public override void Stop()
        {
            if (process == null) return;

            Logger.Info("Stopping PlayIt Tunnel");

            CommandPrompt.KillProcess(process);
        }
    }
}
