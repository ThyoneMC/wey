using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Native;

namespace wey.Console
{
    class KeyReader
    {
        public static readonly IGlobalHook GlobalHook = new TaskPoolGlobalHook();

        public static KeyboardEventData LatestPressed { get; private set; } = new();
        public static long LatestPressedTimestamp { get; private set; }

        public static KeyboardEventData LatestReleased { get; private set; } = new();
        public static long LatestReleasedTimestamp { get; private set; }

        private static readonly List<KeyboardEventData> HoverKeys = new();

        private static KeyboardEventData? _OncePressed = null;
        private static KeyboardEventData? OncePressed = null;

        public static void Reset()
        {
            LatestPressed = new();
            LatestPressedTimestamp = -1;

            LatestReleased = new();
            LatestReleasedTimestamp = -1;

            HoverKeys.Clear();

            OncePressed = null;
        }

        private static long GetTimestamp()
        {
            return DateTime.Now.ToFileTime();
        }

        private readonly static CancellationTokenSource TokenSource = new();

        static KeyReader()
        {
            long _Timestamp = GetTimestamp() + TimestampRange + 1;
            LatestPressedTimestamp = _Timestamp;
            LatestReleasedTimestamp = _Timestamp;

            GlobalHook.KeyPressed += (_sender, keyboardEvent) =>
            {
                LatestPressed = keyboardEvent.Data;
                LatestPressedTimestamp = GetTimestamp();

                HoverKeys.Add(keyboardEvent.Data);

                OncePressed = keyboardEvent.Data;
            };

            GlobalHook.KeyReleased += (_sender, keyboardEvent) =>
            {
                LatestReleased = keyboardEvent.Data;
                LatestReleasedTimestamp = GetTimestamp();

                HoverKeys.RemoveAll(key => key == keyboardEvent.Data);
            };

            Task.Run(GlobalHook.RunAsync, TokenSource.Token);
        }

        public static void Stop()
        {
            TokenSource.Cancel();
        }

        public static KeyboardEventData? GetOnce()
        {
            if (OncePressed != null)
            {
                _OncePressed = OncePressed.Value;
                OncePressed = null;

                return _OncePressed;
            }

            return null;
        }

        public static bool GetHoverOnce(params KeyCode[] keys)
        {
            return HoverKeys.Any(hover => keys.Contains(hover.KeyCode));
        }

        public static bool GetHoverAll(params KeyCode[] keys)
        {
            var hover = HoverKeys.Select(hover => hover.KeyCode);

            return keys.All(key => hover.Contains(key));
        }

        public static long TimestampRange = 500;

        public static KeyboardEventData? GetRaw()
        {
            if (LatestPressed.KeyCode != LatestReleased.KeyCode) return null;

            if (LatestReleasedTimestamp + TimestampRange < GetTimestamp()) return null;

            return LatestReleased;
        }

        public static KeyCode? Get()
        {
            KeyboardEventData? raw = GetRaw();

            if (raw == null) return null;

            return raw.Value.KeyCode;
        }
    }

    class ConsoleReaderKeybaordData
    {
        public KeyCode Key { get; set; }
        public string? Character { get; set; }
    }

    class ConsoleReader
    {
        // static

        private static string? Translate(KeyCode key)
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

        public static ConsoleReaderKeybaordData? ReadChar()
        {
            KeyboardEventData? raw = KeyReader.GetOnce();
            if (raw == null) return null;

            return new()
            {
                Key = raw.Value.KeyCode,
                Character = Translate(raw.Value.KeyCode),
            };
        }

        public static string ReadLine()
        {
            KeyReader.GetOnce(); //debug

            int _top = System.Console.CursorTop;
            List<string> builder = new();

            while (true)
            {
                ConsoleReaderKeybaordData? raw = ReadChar();
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

        // class

        private bool IsLoaded = false;

        private int StartingCursorLeft = -1;
        private int StartingCursorTop = -1;

        private void OnLoad()
        {
            (StartingCursorLeft, StartingCursorTop) = System.Console.GetCursorPosition();

            StartingCursorLeft++;
            StartingCursorTop++;
        }

        private void OnRead()
        {
            System.Console.CursorTop = System.Console.WindowHeight - 1;
        }

        public void RenderNext()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                OnLoad();
            }

            OnRead();
        }
    }
}
