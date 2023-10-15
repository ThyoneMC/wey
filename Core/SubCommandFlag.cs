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

        public static bool GetUsed(string[] flags, string flagFind)
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

        public static string? GetContent(string[] flags, string flagFind)
        {
            foreach (string flag in flags)
            {
                // used flag
                if (flag.StartsWith(flagFind))
                {
                    int indexOfEqualSign = flag.IndexOf("=");
                    if (indexOfEqualSign == -1)
                    {
                        // no value
                        return string.Empty;
                    }

                    string content = flag[(indexOfEqualSign + "=".Length)..];
                    if (!content.StartsWith("\"")) return content;

                    // in quote
                    List<string> stringBuilder = new();

                    int indexOf = Array.IndexOf(flags, flag);

                    bool isInQuote = false;
                    foreach (string flagArg in flags[indexOf..])
                    {
                        if (flagArg.StartsWith("\""))
                        {
                            isInQuote = true;

                            stringBuilder.Add(flagArg[1..]);
                        }

                        if (isInQuote) stringBuilder.Add(flagArg);

                        if (flagArg.EndsWith("\""))
                        {
                            // 2 --> (1 from index) + (1 from quote sign)
                            stringBuilder.Add(flagArg[..(flagArg.Length - 2)]);

                            break;
                        }
                    }

                    return string.Join(" ", stringBuilder);
                }
            }

            return null;
        }
    }
}
