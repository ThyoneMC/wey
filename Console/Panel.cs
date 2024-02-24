﻿using SharpHook.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TextCopy;
using wey.Global;

namespace wey.Console
{
    class Panel
    {
        protected static void Reset()
        {
            Status = "wey";

            StatusCursorEnd = -1;
            StatusCursor = -1;

            IsStatusChange = true;

            Input = ">";

            InputCursorBegin = -1;
            InputCursor = -1;

            InputDisable = false;

            IsInputChange = true;

            IsCanvasChange = true;

            ClearCanvas();
        }

        private static readonly TaskWorker Worker = new(() =>
        {
            if (IsStatusChange)
            {
                IsStatusChange = false;

                RenderStatus();
            }

            if (IsInputChange)
            {
                IsInputChange = false;

                RenderInput();
            }

            if (IsCanvasChange) 
            {
                IsCanvasChange = false;

                RenderCanvas();
            }

            RenderInputBuilder();
            RenderCanvasScroller();
        });

        public static void Start()
        {
            System.Console.CursorVisible = false;
            System.Console.Clear();

            Reset();

            Worker.Start();
        }

        public static void Stop()
        {
            ClearInput();

            Worker.Stop();

            System.Console.CursorVisible = true;
            System.Console.Clear();
        }

        //status

        private static string Status = string.Empty;

        private static int StatusCursorEnd;
        private static int StatusCursor;

        private static bool IsStatusChange;

        public static void SetStatus(string? text = null)
        {
            if (text != null)
            {
                Status = text;
            }

            IsStatusChange = true;
        }

        protected static void RenderStatus()
        {
            if (StatusCursor == -1)
            {
                System.Console.CursorTop = 0;

                (int StartLine, StatusCursorEnd) = Logger.WriteMultiple(
                    string.Empty,
                    Status,
                    string.Empty,
                    new string('-', System.Console.WindowWidth),
                    string.Empty
                );

                StatusCursor = StartLine + 1;

                return;
            }

            Logger.ClearLine(StatusCursor);

            System.Console.CursorTop = StatusCursor;
            Logger.WriteSingle(Status);
        }

        //input

        private static string Input = string.Empty;

        private static int InputCursorBegin;
        private static int InputCursor;

        public static bool InputDisable;
        public static string InputDisableMessage = "https://github.com/ThyoneMC/wey";

        private static bool IsInputChange;

        public static void SetInput(string? text = null)
        {
            if (text != null)
            {
                Input = $"> {text}";
            }

            IsInputChange = true;
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

        // input - builder

        public static List<string> InputList = new();
        private static StringBuilder InputBuilder = new();
        
        private static bool InputBuilderNext()
        {
            ConsoleReaderKeybaordData? raw = ConsoleReader.ReadChar();

            if (raw == null) return false;

            if (raw.Key == KeyCode.VcUp && InputList.Count > 0)
            {
                InputBuilder.Clear();
                InputBuilder.Append(InputList.Last());

                return true;
            }

            if (raw.Key == KeyCode.VcEnter)
            {
                InputList.Add(InputBuilder.ToString());
                InputBuilder = new();

                return true;
            }

            if (KeyReader.GetHoverOnce(KeyCode.VcLeftControl, KeyCode.VcRightControl) && raw.Key == KeyCode.VcV)
            {
                string? Clipboard = ClipboardService.GetText();
                if (Clipboard == null) return false;

                InputBuilder.Append(Clipboard);

                return true;
            }

            if ((raw.Key == KeyCode.VcBackspace || raw.Key == KeyCode.VcDelete) && InputBuilder.Length > 0)
            {
                InputBuilder.Remove(InputBuilder.Length - 1, 1);

                return true;
            }

            if (raw.Character == null) return false;

            InputBuilder.Append(raw.Character);
            return true;
        }

        protected static void RenderInputBuilder()
        {
            if (InputBuilderNext()) //IsInputChange
            {
                SetInput(InputBuilder.ToString());
            }
        }

        public static string? GetInput(bool remove = true)
        {
            if (InputList.Count == 0) return null;

            string _Input = InputList.Last();

            if (remove) InputList.RemoveAt(InputList.Count - 1);

            return _Input;
        }

        public static string GetCurrentInput()
        {
            return InputBuilder.ToString();
        }

        public static void ClearInput()
        {
            InputBuilder.Clear();
            InputList.Clear();

            SetInput(null);
        }

        // canvas

        private static readonly List<string> CanvasLines = new();

        protected static string[] ParseCanvasLines(string[] lines)
        {
            List<string> ParsedLines = new();

            foreach (string line in lines)
            {
                if (line.Contains('\n'))
                {
                    ParsedLines.AddRange(ParseCanvasLines(line.Split('\n')));
                    continue;
                }

                if (line.Length > System.Console.WindowWidth)
                {
                    for (var i = 0; i < line.Length; i += System.Console.WindowWidth)
                    {
                        ParsedLines.Add(line.Substring(i, Math.Min(System.Console.WindowWidth, line.Length - i)));
                    }
                }
                else
                {
                    ParsedLines.Add(line);
                }
            }

            return ParsedLines.ToArray();
        }

        protected static double StartIndexPercent = 100;

        protected static void RenderCanvas()
        {
            int CanvasCursorBegin = StatusCursorEnd + 1;
            int CanvasCursorEnd = InputCursorBegin - 1;

            Logger.ClearFromLine(CanvasCursorBegin, CanvasCursorEnd);

            string[] ParsedLines = ParseCanvasLines(CanvasLines.ToArray());

            int Size = CanvasCursorEnd - CanvasCursorBegin;
            int StartIndex = (int)Math.Floor(StartIndexPercent / 100.0 * ParsedLines.Length);
            int EndIndex;
            if (ParsedLines.Length - StartIndex <= Size)
            {
                EndIndex = ParsedLines.Length - 1;
                StartIndex = EndIndex > Size ? EndIndex - Size : 0;
            }
            else
            {
                EndIndex = StartIndex + Size;
            }

            System.Console.CursorTop = CanvasCursorBegin;
            while (StartIndex <= EndIndex && StartIndex < ParsedLines.Length)
            {
                Logger.WriteSingle(ParsedLines[StartIndex]);

                StartIndex++;
            }
        }

        private static bool IsCanvasChange;

        public static void AddCanvas(params string[] lines)
        {
            CanvasLines.AddRange(lines);

            IsCanvasChange = true;
        }

        public static void ClearCanvas()
        {
            CanvasLines.Clear();

            IsCanvasChange = true;
        }

        protected static void RenderCanvasScroller()
        {
            if (KeyReader.GetHoverOnce(KeyCode.VcUp))
            {
                if (StartIndexPercent != 0)
                {
                    StartIndexPercent--;
                }

                IsCanvasChange = true;
                return;
            }

            if (KeyReader.GetHoverOnce(KeyCode.VcDown))
            {
                if (StartIndexPercent != 100)
                {
                    StartIndexPercent++;
                }

                IsCanvasChange = true;
                return;
            }
        }
    }
}
