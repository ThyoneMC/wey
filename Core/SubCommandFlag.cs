using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Core
{
    public enum SubCommandFlagType
    {
        String,
        Integer,
        Decimal,
        Boolean
    }

    public abstract class SubCommandFlag: SubCommandBase
    {
        // class

        public override SubCommandType GetType()
        {
            return SubCommandType.Flag;
        }

        public abstract SubCommandFlagType GetFlagType();

        public abstract bool GetRequiredValue();

        // static

        public static bool GetUsed(IDictionary<string, string?> flags, string flagFind)
        {
            foreach(string flagString in flags.Keys)
            {
                if (flagString == flagFind)
                {
                    return true;
                }
            }

            return false;
        }

        public static string? GetContent(IDictionary<string, string?> flags, string flagFind)
        {
            foreach (string flagString in flags.Keys)
            {
                if (flagString == flagFind)
                {
                    return flags[flagString];
                }
            }

            return null;
        }

        public static string GetContentRequired(IDictionary<string, string?> flags, string flagFind)
        {
            return GetContent(flags, flagFind) ?? string.Empty;
        }
    }
}
