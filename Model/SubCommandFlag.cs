using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Model
{
    public class ISubCommandFlags : Dictionary<string, string?>
    {
       
    }

    public enum SubCommandFlagType
    {
        String,
        Integer,
        Decimal,
        Boolean
    }

    public abstract class SubCommandFlag : SubCommandBase
    {
        // class

        public override SubCommandType GetType()
        {
            return SubCommandType.Flag;
        }

        public abstract SubCommandFlagType GetFlagType();

        public abstract bool GetRequiredValue();

        // static

        public static bool GetUsed(ISubCommandFlags flags, string flagFind)
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

        public static string? GetContent(ISubCommandFlags flags, string flagFind)
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

        public static string GetContentRequired(ISubCommandFlags flags, string flagFind)
        {
            return GetContent(flags, flagFind) ?? string.Empty;
        }

        public static ISubCommandFlags GenerateDictionary(params SubCommandFlag[] flags)
        {
            ISubCommandFlags flagDictionary = new();

            foreach (SubCommandFlag flag in flags)
            {
                if (flag.GetRequiredValue()) throw new ArgumentException($"{flag.GetName()} flag needs value");

                flagDictionary.Add(flag.GetName(), string.Empty);
            }

            return flagDictionary;
        }
    }
}
