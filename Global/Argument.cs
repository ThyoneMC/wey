using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Global
{
    public class Argument
    {
        public static List<string> Arguments { get; protected set; } = new();

        public static Dictionary<string, string?> Flags { get; protected set; } = new();

        public static void Import(params string[] args)
        {
            foreach (string text in args)
            {
                // args
                if (!text.StartsWith("--"))
                {
                    Arguments.Add(text);
                    continue;
                }

                // flag
                int indexOfEqualSign = text.IndexOf("=");
                if (indexOfEqualSign == -1)
                {
                    Flags.Add(text, string.Empty);
                    continue;
                }

                // flag=
                string content = text[(indexOfEqualSign + "=".Length)..];
                if (string.IsNullOrWhiteSpace(content))
                {
                    Flags.Add(text[..indexOfEqualSign], string.Empty);
                    continue;
                }

                // flag=content
                Flags.Add(text[..indexOfEqualSign], content);
            }
        }
    }
}
