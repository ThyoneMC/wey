using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    class InputReadCharData
    {
        public KeyCode Key { get; set; }
        public string? Character { get; set; }
    }

    class Input
    {
        private static string? TranslateChar(KeyCode key)
        {
            string? translate = key switch
            {
                KeyCode.VcA => "a",
                KeyCode.VcB => "b",
                KeyCode.VcC => "c",
                KeyCode.VcD => "d",
                KeyCode.VcE => "e",
                KeyCode.VcF => "f",
                KeyCode.VcG => "g",
                KeyCode.VcH => "h",
                KeyCode.VcI => "i",
                KeyCode.VcJ => "j",
                KeyCode.VcK => "k",
                KeyCode.VcL => "l",
                KeyCode.VcM => "m",
                KeyCode.VcN => "n",
                KeyCode.VcO => "o",
                KeyCode.VcP => "p",
                KeyCode.VcQ => "q",
                KeyCode.VcR => "r",
                KeyCode.VcS => "s",
                KeyCode.VcT => "t",
                KeyCode.VcU => "u",
                KeyCode.VcV => "v",
                KeyCode.VcW => "w",
                KeyCode.VcX => "x",
                KeyCode.VcY => "y",
                KeyCode.VcZ => "z",

                KeyCode.Vc1 or KeyCode.VcNumPad1 => "1",
                KeyCode.Vc2 or KeyCode.VcNumPad2 => "2",
                KeyCode.Vc3 or KeyCode.VcNumPad3 => "3",
                KeyCode.Vc4 or KeyCode.VcNumPad4 => "4",
                KeyCode.Vc5 or KeyCode.VcNumPad5 => "5",
                KeyCode.Vc6 or KeyCode.VcNumPad6 => "6",
                KeyCode.Vc7 or KeyCode.VcNumPad7 => "7",
                KeyCode.Vc8 or KeyCode.VcNumPad8 => "8",
                KeyCode.Vc9 or KeyCode.VcNumPad9 => "9",
                KeyCode.Vc0 or KeyCode.VcNumPad0 => "0",

                KeyCode.VcMinus or KeyCode.VcNumPadSubtract => "-",
                KeyCode.VcEquals or KeyCode.VcNumPadEquals => "=",

                KeyCode.VcOpenBracket => "[",
                KeyCode.VcCloseBracket => "]",

                KeyCode.VcSemicolon => ";",
                KeyCode.VcQuote => "'",

                KeyCode.VcComma => ",",
                KeyCode.VcPeriod or KeyCode.VcNumPadDecimal => ".",
                KeyCode.VcSlash or KeyCode.VcNumPadDivide => "/",

                KeyCode.VcSpace => " ",

                _ => null,
            };

            if (translate == null) return null;

            if (KeyReader.GetHoverOnce(KeyCode.VcLeftShift, KeyCode.VcRightShift))
            {
                string upper = translate.ToUpper();
                if (!translate.Equals(upper)) return upper;

                string? shift = translate switch
                {
                    "1" => "!",
                    "2" => "@",
                    "3" => "#",
                    "4" => "$",
                    "5" => "%",
                    "6" => "^",
                    "7" => "&",
                    "8" => "*",
                    "9" => "(",
                    "0" => ")",

                    "-" => "_",
                    "=" => "+",

                    "[" => "{",
                    "]" => "}",

                    ";" => ":",
                    "'" => "\"",

                    "," => "<",
                    "." => ">",
                    "?" => "?",

                    _ => null,
                };
                return shift ?? translate;
            }

            return translate;
        }

        public static InputReadCharData? ReadChar()
        {
            KeyboardEventData? raw = KeyReader.GetOnce();
            if (raw == null) return null;

            return new()
            {
                Key = raw.Value.KeyCode,
                Character = TranslateChar(raw.Value.KeyCode),
            };
        }

        public static string ReadLine()
        {
            KeyReader.GetOnce(); //debug

            List<string> builder = new();

            while (true)
            {
                InputReadCharData? raw = ReadChar();
                if (raw == null) continue;

                if (raw.Key == KeyCode.VcEnter) break;

                if ((raw.Key == KeyCode.VcBackspace || raw.Key == KeyCode.VcDelete) && builder.Count > 0)
                {
                    builder.RemoveAt(builder.Count - 1);

                    System.Console.CursorLeft--;
                    System.Console.Write(" ");
                    System.Console.CursorLeft--;

                    continue;
                }

                if (raw.Character == null) continue;

                builder.Add(raw.Character);
                System.Console.Write(raw.Character);
            }

            System.Console.WriteLine();
            KeyReader.Reset();

            return string.Join("", builder);
        }

        public static string ReadString(string? message = null, bool required = false, bool clear = false)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");

            (int StartingCursorLeft, int StartingCursorTop) = System.Console.GetCursorPosition();

            while (true)
            {
                System.Console.SetCursorPosition(StartingCursorLeft, StartingCursorTop);

                string? read = Input.ReadLine();

                if (read != null)
                {
                    if (required && string.IsNullOrWhiteSpace(read)) continue;

                    if (clear)
                    {
                        Logger.ClearLine(StartingCursorTop);
                        System.Console.CursorTop = StartingCursorTop;
                    }

                    return read;
                }
            }
        }

        public static SelectionChoice<string> SelectionString(IEnumerable<string> choices, string? message = null)
        {
            int StartingCursorTop = System.Console.CursorTop;

            if (!string.IsNullOrEmpty(message)) System.Console.WriteLine($"{message}: ");

            Selection<string> Selector = Selection<string>.Create(choices);
            while (true)
            {
                Selector.RenderNext();

                if (Selector.Result != null)
                {
                    break;
                }
            }

            Logger.ClearFromLine(StartingCursorTop);

            if (!string.IsNullOrEmpty(message))
            {
                System.Console.CursorTop = StartingCursorTop;
                System.Console.WriteLine($"{message}: {Selector.Result.Value}");
            }

            return Selector.Result;
        }

        public static bool ReadBoolean(string? message = null, bool defaultOption = true)
        {
            if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message} (default: {defaultOption}): ");

            string read = ReadString(required: false).ToLower();

            // contains "no"
            if (defaultOption == true && (read.Contains('n') || read.ToLower() == "false"))
            {
                return false;
            }

            // contains "yes"
            if (defaultOption == false && (read.Contains('y') || read.ToLower() == "true"))
            {
                return true;
            }

            return defaultOption;
        }

        public static int ReadInteger(string? message = null, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");
                string read = ReadString(clear: true);

                if (int.TryParse(read, out int number))
                {
                    if (number < min || number > max) continue;

                    System.Console.WriteLine($"{message}: {number}");
                    return number;
                }

                Logger.ClearLine(System.Console.CursorTop);
            }
        }

        public static float ReadFloat(string? message = null, float min = float.MinValue, float max = float.MaxValue)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(message)) System.Console.Write($"{message}: ");
                string read = ReadString(clear: true);

                if (float.TryParse(read, out float number))
                {
                    if (number < min || number > max) continue;

                    System.Console.WriteLine($"{message}: {number}");
                    return number;
                }

                Logger.ClearLine(System.Console.CursorTop);
            }
        }
    }
}
