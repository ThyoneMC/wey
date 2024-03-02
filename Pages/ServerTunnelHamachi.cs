using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextCopy;
using wey.Console;
using wey.Forwarding;
using wey.Global;
using wey.Host;
using wey.Interface;

namespace wey.Pages
{
    class ServerTunnelHamachi : IPageCommand
    {
        private readonly HostData HostData;

        public ServerTunnelHamachi(HostData host)
        {
            HostData = host;

            ExitDelay = 0;
        }

        public override string GetName()
        {
            return "hamachi";
        }

        public override string GetDescription()
        {
            return "hamachi";
        }

        public override void OnCommand()
        {
            Hamachi host = new(HostData);

            //register
            if (!host.IsRegistered())
            {
                Logger.WriteMultiple("[https://vpn.net/]", string.Empty);

                if (ExecutablePlatform.IsOsWindows)
                {
                    string path = Path.GetFullPath(Input.ReadString("Path to your hamachi executable", true));

                    if (!File.Exists(path))
                    {
                        Logger.Info($"Not Found: {path}");
                        return;
                    }

                    string fileName = Path.GetFileName(path);

                    if (fileName == "hamachi-2-ui.exe")
                    {
                        string folder = StaticFileController.GetDirectory(path);

                        //x64
                        host.Register(Path.Join(folder, "x64", "hamachi-2.exe"));
                    }
                    else if (fileName == "hamachi-2.exe")
                    {
                        host.Register(path);
                    }
                    else
                    {
                        Logger.Info($"Executable Not Found");
                        return;
                    }
                }
                else
                {
                    string? path = Executable.Where("hamachi");
                    if (path == null)
                    {
                        Logger.Info($"Path Environment Not Found");
                        return;
                    }
                }

                System.Console.Clear();
            }

            //command

            HamachiData read = host.hamachi.ReadRequired();

            if (read.IsRunning)
            {
                Logger.WriteSingle($"Current State: Running");
            }
            else
            {
                Logger.WriteSingle($"Current State: Stop");
            }

            string[] commands = new string[] { "exit", "create" };

            StringBuilder info = new();
            if (read.Total > 0)
            {
                commands = new string[] { "exit", "copy", "create", "delete" };

                info.AppendLine($"Name:");
                for (int i = 1; i <= read.Total; i++)
                {
                    info.AppendLine($"\t> {read.Name}-{i}");
                }

                info.Append($"Password: {read.Password}");

                Logger.WriteSingle(info.ToString());
            }

            Logger.WriteSingle(string.Empty);
            switch (Selection<string>.Create(commands).Get().Value)
            {
                case "copy":
                    {
                        new Clipboard().SetText(info.ToString());
                        break;
                    }
                case "create":
                    {
                        host.Create();
                        break;
                    }
                case "delete":
                    {
                        host.Delete();
                        break;
                    }
            }
        }
    }
}
