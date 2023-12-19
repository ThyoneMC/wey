using SharpHook.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCopy;
using wey.Global;

namespace wey.Console
{
    class Panel
    {
        //status

        private static string Status = "wey";

        private static int StatusCursorEnd = -1;
        private static int StatusCursor = -1;

        public static void SetStatus(string? text = null)
        {
            if (text != null)
            {
                Status = text;
            }

            RenderStatus();
        }

        protected static void RenderStatus()
        {
            if (StatusCursor == -1)
            {
                System.Console.CursorTop = 0;

                (StatusCursor, StatusCursorEnd) = Logger.WriteMultiple(
                    Status,
                    string.Empty,
                    new string('-', System.Console.WindowWidth),
                    string.Empty
                );

                return;
            }

            Logger.ClearLine(StatusCursor);

            System.Console.CursorTop = StatusCursor;
            Logger.WriteSingle(Status);
        }

        //input

        private static string Input = ">";

        private static int InputCursorBegin = -1;
        private static int InputCursor = -1;

        public static bool InputDisable = false;
        public static string InputDisableMessage = "https://github.com/ThyoneMC/wey";

        public static void SetInput(string? text = null)
        {
            if (text != null)
            {
                Input = $"> {text}";
            }

            RenderInput();
        }

        protected static void RenderInput()
        {
            string _Input = Input;
            if (InputDisable) _Input = InputDisableMessage;

            if (InputCursor == -1)
            {
                System.Console.CursorTop = System.Console.WindowHeight - 5;

                (InputCursorBegin, InputCursor) = Logger.WriteMultiple(
                    string.Empty,
                    new string('-', System.Console.WindowWidth),
                    string.Empty,
                    _Input
                    //string.Empty (EndLine - 1)
                );

                return;
            }

            Logger.ClearLine(InputCursor);

            System.Console.CursorTop = InputCursor;
            Logger.WriteSingle(_Input);
        }

        //canvas

        private static List<string> CanvasLines = new()
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6"
        };

        //<-- protected
        public static void RenderCanvas()
        {
            int CanvasCursorBegin = StatusCursorEnd + 1;
            int CanvasCursorEnd = InputCursorBegin - 1;

            Logger.ClearFromLine(CanvasCursorBegin, CanvasCursorEnd);

            int Size = CanvasCursorEnd - CanvasCursorBegin;
            double StartIndexPercent = 28;
            int StartIndex = (int)Math.Floor(StartIndexPercent / 100.0 * CanvasLines.Count);
            int EndIndex = -1;
            if (CanvasLines.Count - StartIndex <= Size)
            {
                EndIndex = CanvasLines.Count - 1;
                StartIndex = EndIndex > Size ? EndIndex - Size : 0;
            }
            else
            {
                EndIndex = StartIndex + Size;
            }

            System.Console.CursorTop = CanvasCursorBegin;
            while (StartIndex <= EndIndex && StartIndex < CanvasLines.Count)
            {
                Logger.WriteSingle(CanvasLines[StartIndex]);

                StartIndex++;
            }
        }
    }
}
