using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Style
{
    class Style
    {
        public static string prefix = "\u001b[";
        public static string subfix = "m";

        private static string ANSI_CODE(sbyte code)
        {
            return $"{prefix}{code}{subfix}";
        }

        public static string Colorize(string text, Color? color = null, Decoration? decoration = null)
        {
            StringBuilder stringBuilder = new();

            if (color != null) stringBuilder.Append(ANSI_CODE((sbyte)color));
            if (decoration != null) stringBuilder.Append(ANSI_CODE((sbyte)decoration));

            stringBuilder.Append(text);
            stringBuilder.Append(ANSI_CODE((sbyte)Decoration.Reset));

            return stringBuilder.ToString();
        }
    }
}
