using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Core;

namespace wey.Command
{
    class Create: SubCommand
    {
        public override string GetName()
        {
            return "create";
        }

        public override string GetDescription()
        {
            return "create new server";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[]
            {
                new CreateNameSyntax()
            };
        }

        public override string[] GetFlag()
        {
            return new string[0];
        }

        public override void Execute(string[] args, string[] flags)
        {
            string ServerType;
            if (args.Length > 0)
            {
                ServerType = args[0];
            } 
            else
            {
                ServerType = Choice.Start("vanilla", "paper", "fabric");
            }

            switch (ServerType)
            {
                case "vanilla":
                    {
                        break;
                    }
                case "paper":
                    {
                        break;
                    }
                case "fabric":
                    {
                        break;
                    }
                default:
                    {
                        System.Console.WriteLine("Not Found");
                        return;
                    }
            }

            System.Console.WriteLine(ServerType);
        }
    }
}
