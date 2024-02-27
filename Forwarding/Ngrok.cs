using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Interface;

namespace wey.Forwarding
{
    class Ngrok : IForwarding
    {
        public Ngrok() : base("ngrok")
        {

        }

        protected static Executable? Process = null;

        public static int Port { get; private set; } = -1;

        public static bool IsRunning()
        {
            return Process != null && Process.IsExists();
        }

        public bool Start(int port)
        {
            if (IsRunning()) return false;

            if (string.IsNullOrWhiteSpace(FilePath)) return false;

            Port = port;

            Logger.Info($"Opening Ngrok Tunnel: {Port}");

            Process = new(new ExecutableOption()
            {
                FileName = FilePath,
                Arguments = $"tcp {Port}"
            });

            Process.Start();

            return true;
        }

        public void Stop()
        {
            if (Process == null) return;

            Logger.Info($"Closing Ngrok Tunnel");

            Process.Kill();
        }
    }
}
