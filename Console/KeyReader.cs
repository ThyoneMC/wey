using System;
using System.Collections.Generic;
using System.Linq;
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

        private static long GetTimestamp()
        {
            return DateTime.Now.ToFileTime();
        }

        private static CancellationTokenSource TokenSource = new();

        static KeyReader()
        {
            long _Timestamp = GetTimestamp() + TimestampRange + 1;
            LatestPressedTimestamp = _Timestamp;
            LatestReleasedTimestamp = _Timestamp;

            GlobalHook.KeyPressed += (_sender, keyboardEvent) =>
            {
                LatestPressed = keyboardEvent.Data;
                LatestPressedTimestamp = GetTimestamp();
            };

            GlobalHook.KeyReleased += (_sender, keyboardEvent) =>
            {
                LatestReleased = keyboardEvent.Data;
                LatestReleasedTimestamp = GetTimestamp();
            };

            Task.Run(GlobalHook.RunAsync, TokenSource.Token);
        }

        public static void Stop()
        {
            TokenSource.Cancel();
        }

        public static long TimestampRange = 500;

        public static KeyCode? Get()
        {
            if (LatestPressed.KeyCode != LatestReleased.KeyCode) return null;

            if (LatestReleasedTimestamp + TimestampRange < GetTimestamp()) return null;

            KeyboardEventData _LatestKey = LatestReleased;

            return _LatestKey.KeyCode;
        }
    }
}
