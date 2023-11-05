using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Tunnel
{
    class Ngrok : TunnelBase
    {
        public static Process? process = null;

        public static void Import(int pid)
        {
            if (!CommandPrompt.IsProcessExists(pid)) return;

            process = Process.GetProcessById(pid);
        }

        public int Port;

        public Ngrok(string path, int port) : base(path)
        {
            this.Port = port;
        }

        public override string GetName()
        {
            return "ngrok";
        }

        public override int Start()
        {
            if (string.IsNullOrEmpty(Path)) return -1;

            Logger.Info("Starting Ngrok Tunnel");

            process = CommandPrompt.StaticExecute(new CommandPromptOptions()
            {
                FileName = Path,
                Arguments = $"tcp {Port}",
                WindowStyle = ProcessWindowStyle.Minimized
            });
            if (process == null) return -1;

            Logger.Info($"Opening Ngrok Tunnel with [port: {Port}]");
            return process.Id;
        }

        public override void Stop()
        {
            if (process == null) return;

            Logger.Info("Stopping Ngrok Tunnel");

            CommandPrompt.KillProcess(process);
        }
    }
}
