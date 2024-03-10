using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Forwarding;
using wey.Global;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class ServerTunnelNgrok : IPageCommand
    {
        private readonly HostData HostData;

        public ServerTunnelNgrok(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "ngrok";
        }

        public override string GetDescription()
        {
            return "ngrok";
        }

        public override void OnCommand()
        {
            Ngrok host = new();

            //register
            if (!host.IsRegistered())
            {
                Logger.WriteMultiple("[https://ngrok.com/download]", string.Empty);

                if (ExecutablePlatform.IsOsWindows)
                {
                    string read = Path.GetFullPath(Input.ReadString("Path to your ngrok executable", true));

                    if (!File.Exists(read))
                    {
                        Logger.Info($"Not Found: {read}");
                        return;
                    }

                    string fileName = Path.GetFileName(read);

                    if (fileName == "ngrok.exe")
                    {
                        host.Register(read);
                    }
                    else
                    {
                        Logger.Info($"Executable Not Found");
                        return;
                    }
                }
                else
                {
                    string? path = Executable.Where("ngrok");
                    if (path == null)
                    {
                        Logger.Info($"Path Environment Not Found");
                        return;
                    }
                }

                Logger.ClearFromLine(2);
            }

            //command

            string? port = new HostProperties(HostData).Get("server-port");

            if (Ngrok.IsRunning())
            {
                if (!string.IsNullOrWhiteSpace(port) && Ngrok.Port == int.Parse(port))
                {
                    Logger.WriteSingle($"Current State: Running");
                }
                else
                {
                    Logger.WriteSingle($"Current State: Running (busy)");
                }
            }
            else
            {
                Logger.WriteSingle($"Current State: Stop");
            }
        }
    }
}
