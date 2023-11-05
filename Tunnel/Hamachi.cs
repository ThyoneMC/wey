using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Tunnel
{
    class HamachiTunnelData
    {
        public string Name = string.Empty;
        public string Password = string.Empty;
        public bool IsCreated = false;
    }

    class Hamachi : TunnelBase
    {
        public HamachiTunnelData Tunnel;

        public Hamachi(string path, HamachiTunnelData tunnelData) : base(path)
        {
            this.Tunnel = tunnelData;
        }

        public override string GetName()
        {
            return "hamachi";
        }

        public override int Start()
        {
            if (string.IsNullOrEmpty(Path)) return -1;

            Logger.Info("Starting Hamachi Tunnel");

            CommandPrompt Hamachi_Command = new(
                    new ProcessStartInfo()
                    {
                        FileName = Path,
                    }
                );

            Hamachi_Command.Execute("--cli", "logon");

            if (Tunnel.IsCreated)
            {
                Hamachi_Command.Execute("--cli", "go-online", Tunnel.Name);
            }
            else
            {
                Hamachi_Command.Execute("--cli", "create", Tunnel.Name, Tunnel.Password);

                Tunnel.IsCreated = true;
            }

            Logger.Info($"Opening Hamachi Tunnel with [name: {Tunnel.Name}, password: {Tunnel.Password}]");

            return 0;
        }

        public override void Stop()
        {
            if (string.IsNullOrEmpty(Path)) return;

            Logger.Info("Stopping Hamachi Tunnel");

            CommandPrompt Hamachi_Command = new(
                    new ProcessStartInfo()
                    {
                        FileName = Path
                    }
                );

            Hamachi_Command.Execute("--cli", "go-offline", Tunnel.Name);
        }

        public void Delete()
        {
            if (string.IsNullOrEmpty(Path)) return;

            Logger.Info("Deleting Hamachi Tunnel");

            CommandPrompt Hamachi_Command = new(
                    new ProcessStartInfo()
                    {
                        FileName = Path
                    }
                );

            Hamachi_Command.Execute("--cli", "delete", Tunnel.Name);
        }
    }
}
