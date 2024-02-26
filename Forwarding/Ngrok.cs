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

        public void Start(int port)
        {
            Logger.Info($"Opening Ngrok Tunnel [port: {port}]");

            Process = new(new ExecutableOption()
            {
                FileName = FilePath,
                Arguments = $"tcp {port}"
            });

            Process.Start();
        }

        public void Stop()
        {
            if (Process == null) return;

            Process.Kill();
        }
    }
}
