using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Core;

namespace wey.Command
{
    class Echo : SubCommand
    {
        public override string GetName()
        {
            return "echo";
        }

        public override string GetDescription()
        {
            return "return what you say";
        }

        public override SubCommandSyntax[] GetSyntax()
        {
            return new SubCommandSyntax[]
            {
                new EchoSyntax(), new EchoSyntax()
            };
        }

        public override void Execute(string[] args)
        {
            System.Console.WriteLine(string.Join(", ", args));
        }
    }
}
