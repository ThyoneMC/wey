using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Model;
using wey.Tool;

namespace wey.Command
{
    class TunnelPathFlag : SubCommandFlag
    {
        //class

        public override SubCommandFlagType GetFlagType()
        {
            return SubCommandFlagType.String;
        }

        public override string GetName()
        {
            return "path";
        }

        public override string GetDescription()
        {
            return "path of executable";
        }

        public override bool GetRequiredValue()
        {
            return true;
        }

        //static

        public static string GetRequiredPath(ISubCommandFlags flags, string? path = null)
        {
            string? executable;

            //system env
            if (!string.IsNullOrEmpty(path))
            {
                executable = CommandPrompt.Where(path);

                if (!string.IsNullOrEmpty(executable)) return executable;
            }

            //path
            if (SubCommandFlag.GetUsed(flags, "path"))
            {
                executable = SubCommandFlag.GetContentRequired(flags, "path");
            }
            else
            {
                executable = Input.ReadString($"{path} path: ");
            }

            if (!File.Exists(Path.GetFullPath(executable))) throw new FileNotFoundException();

            return executable;
        }
    }
}
