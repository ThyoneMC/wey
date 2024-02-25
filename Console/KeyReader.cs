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
            try
            {
                return HoverKeys.Any(hover => keys.Contains(hover.KeyCode));
            }
            catch (Exception)
            {
                return false;
            }
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
}
