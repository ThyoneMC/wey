using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wey.Core
{
    public abstract class SubCommand: SubCommandBase
    {
        // class

        public override SubCommandType GetType()
        {
            return SubCommandType.Executable;
        }

        public abstract SubCommandSyntax[] GetSyntax();

        public abstract string[] GetFlags();

        public abstract Task Execute(string[] args, string[] flags);

        // static

        public static bool GetFlagUsed(string[] flags, string flagFind)
        {
            foreach (string flag in flags)
            {
                if (flag.StartsWith(flagFind))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFlagContent(string[] flags, string flagFind)
        {
            foreach (string flag in flags)
            {
                if (flag.StartsWith(flagFind))
                {
                    int indexOfEqualSign = flag.IndexOf("=");
                    if (indexOfEqualSign == -1)
                    {
                        return "NO_INPUT";
                    }

                    return flag.Substring(indexOfEqualSign + "=".Length);
                }
            }

            return string.Empty;
        }
    }
}
