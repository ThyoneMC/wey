using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class Ping: SubCommand
    {
        public override string GetName()
        {
            return "ping";
        }

        public override string GetDescription()
        {
            return "pong";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[0];
        }

        public override void Execute(string[] args)
        {
            System.Console.WriteLine("pong");
        }
    }
}
