﻿using System;
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
                KeyboardEventData tempOncePressed = OncePressed.Value;
                OncePressed = null;

                return tempOncePressed;
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

                KeyCode.Vc0 or KeyCode.VcNumPad0 => "0",
                KeyCode.Vc1 or KeyCode.VcNumPad1 => "1",
                KeyCode.Vc2 or KeyCode.VcNumPad2 => "2",
                KeyCode.Vc3 or KeyCode.VcNumPad3 => "3",
                KeyCode.Vc4 or KeyCode.VcNumPad4 => "4",
                KeyCode.Vc5 or KeyCode.VcNumPad5 => "5",
                KeyCode.Vc6 or KeyCode.VcNumPad6 => "6",
                KeyCode.Vc7 or KeyCode.VcNumPad7 => "7",
                KeyCode.Vc8 or KeyCode.VcNumPad8 => "8",
                KeyCode.Vc9 or KeyCode.VcNumPad9 => "9",

                KeyCode.VcPeriod or KeyCode.VcNumPadDecimal => ".",
                KeyCode.VcMinus or KeyCode.VcNumPadSubtract => "-",

                KeyCode.VcSpace => " ",

                _ => null,
            };

            if (translate != null && GetHoverOnce(KeyCode.VcLeftShift, KeyCode.VcRightShift))
            {
                translate = translate.ToUpper();
            }

            return translate;
        }

        public static string ReadLine()
        {
            List<string> builder = new();

            OncePressed = null;
            while (true)
            {
                KeyboardEventData? raw = GetOnce();
                if (raw == null) continue;

                if (raw.Value.KeyCode == KeyCode.VcEnter) break;

                if ((raw.Value.KeyCode == KeyCode.VcBackspace || raw.Value.KeyCode == KeyCode.VcDelete) && builder.Count > 0)
                {
                    builder.RemoveAt(builder.Count - 1);

                    System.Console.CursorLeft--;
                    System.Console.Write(" ");
                    System.Console.CursorLeft--;
                    continue;
                }

                string? translate = Translate(raw.Value.KeyCode);
                if (translate == null) continue;

                builder.Add(translate);
                System.Console.Write(translate);
            }

            System.Console.WriteLine();
            KeyReader.Reset();

            return string.Join("", builder);
        }
    }
}