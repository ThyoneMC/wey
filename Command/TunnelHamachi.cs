using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Core;
using wey.Tool;

namespace wey.Command
{
    class TunnelHamachi: SubCommand
    {
        public override string GetName()
        {
            return "hamachi";
        }

        public override string GetDescription()
        {
            return "hamachi";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return Array.Empty<SubCommandSyntax>();
        }

        public override SubCommandFlag[] GetFlags()
        {
            return new SubCommandFlag[]
            {
                new TunnelHamachiJoinFlag(),
                new TunnelHamachiDeleteFlag(),
            };
        }

        public override void Execute(string[] args, Dictionary<string, string?> flags)
        {
            //join flag
            if (SubCommandFlag.GetUsed(flags, "join"))
            {
                JoinTunnel(SubCommandFlag.GetContentRequired(flags, "join"));

                Logger.Info("joined");
                return;
            }

            //delete flag
            if (SubCommandFlag.GetUsed(flags, "delete"))
            {
                DeleteTunnel(SubCommandFlag.GetContentRequired(flags, "delete"));

                Config.Edit(
                    (data) => {
                        data.Hamachi = string.Empty;
                        return data;
                    }
                );

                Logger.Info("deleted");
                return;
            }

            //path
            string Hamachi_InputPath = Path.GetFullPath(Input.ReadString("hamachi path: "));
            if (!File.Exists(Hamachi_InputPath))
            {
                Logger.Warn("make sure it not shortcut");
                Logger.Error($"{Hamachi_InputPath} not found");
                return;
            }

            string Hamachi_Folder = Path.GetDirectoryName(Hamachi_InputPath);
            string Hamachi_ExecuteFolder = Path.Join(Hamachi_Folder, "x64");
            if (!Directory.Exists(Hamachi_ExecuteFolder))
            {
                Logger.Error($"{Hamachi_ExecuteFolder} not found");
                return;
            }

            string Hamachi_Execute = Path.Join(Hamachi_ExecuteFolder, "hamachi-2.exe");
            if (!File.Exists(Hamachi_Execute))
            {
                Logger.Error($"{Hamachi_Execute} not found");
                return;
            }

            //config
            Config.Edit(
                    (data) => {
                        data.Hamachi = Hamachi_Execute;
                        return data;
                    }
                );

            Logger.Warn("please connect to your account before using");
        }

        public static IDictionary<string, string> HamachiOpening = new Dictionary<string, string>();

        public static string StartTunnel(string name)
        {
            name = name.Trim();

            string Hamachi_Path = Config.Get().Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            Hamachi_Command.Execute("--cli", "logon");

            // creates a number between 0 and 999999
            int password = new Random().Next(999999);

            Hamachi_Command.Execute("--cli", "create", name, password.ToString());
            HamachiOpening.Add(name, "create");

            return $"{name};{password}";
        }

        public static void DeleteTunnel(string name)
        {
            string Hamachi_Path = Config.Get().Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            if (HamachiOpening[name] == "create")
            {
                Hamachi_Command.Execute("--cli", "delete", name);
            }
            else
            {
                Hamachi_Command.Execute("--cli", "leave", name);
            }

            HamachiOpening.Remove(name);
        }

        public static void JoinTunnel(string joinInfo)
        {
            string Hamachi_Path = Config.Get().Hamachi;
            CommandPrompt Hamachi_Command = new(
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Hamachi_Path
                    }
                );

            Hamachi_Command.Execute("--cli", "logon");

            string[] Hamachi_JoinInfo = joinInfo.Split(';');
            string name = Hamachi_JoinInfo[0];
            string password = Hamachi_JoinInfo[1];

            Hamachi_Command.Execute("--cli", "join", name, password);
            HamachiOpening.Add(name, "join");
        }
    }
}
