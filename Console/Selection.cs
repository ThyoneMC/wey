using SharpHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Global;
using SharpHook.Native;

namespace wey.Console
{
    class SelectionChoice<T>
    {
        public int Index { get; set; } = -1;
        public T Value { get; set; } = JsonFileController<T>.CreateTypeInstance();
    }

    class Selection<T>
    {
        protected int SelectionIndex = 0;

        private List<SelectionChoice<T>> Choices = new();

        private Selection(IEnumerable<T> choices)
        {
            int index = 0;

            foreach (T choice in choices)
            {
                Choices.Add(
                        new SelectionChoice<T>()
                        {
                            Index = index,
                            Value = choice,
                        }
                    );

                index++;
            }
        }

        public static Selection<T> Create(IEnumerable<T> choices)
        {
            return new (choices);
        }

        public static Selection<T> Create(params T[] choices)
        {
            return new (choices);
        }

        public static Selection<T> Create(SelectionChoice<T>[] choices)
        {
            return new (choices.OrderBy(choice => choice.Index).Select(choice => choice.Value));
        }

        public SelectionChoice<T>? Result = null;

        private int StartingCursorPosition = -1;
        private int _TempSelectionIndex = -1;

        private int StartIndex = 0;
        private int EndIndex = 0;

        private void OnLoad()
        {
            StartingCursorPosition = System.Console.GetCursorPosition().Top;

            KeyReader.TimestampRange = 4250;

            if (Choices.Count > 10)
            {
                EndIndex = 9;
            }
            else
            {
                EndIndex = Choices.Count - 1;
            }
        }

        private void OnViewing()
        {
            if (_TempSelectionIndex != SelectionIndex)
            {
                _TempSelectionIndex = SelectionIndex;

                System.Console.SetCursorPosition(0, StartingCursorPosition);

                int index = StartIndex;
                while (index != EndIndex + 1)
                {
                    SelectionChoice<T> choice = Choices[index];

                    string BaseString = $"[{choice.Index + 1}] {choice.Value}";

                    if (index == SelectionIndex)
                    {
                        Logger.WriteSingle($" > {BaseString}");
                    }
                    else
                    {
                        Logger.WriteSingle($"   {BaseString}");
                    }

                    index++;
                }
            }

            //selector
            switch (KeyReader.Get())
            {
                case KeyCode.VcUp:
                    {
                        if (Choices.Count == 1) break;
                        SelectionIndex--;

                        if (SelectionIndex < 0)
                        {
                            SelectionIndex = Choices.Count - 1;

                            EndIndex = Choices.Count - 1;
                            if (Choices.Count > 10)
                            {
                                StartIndex = (int)(Choices.Count / 10) * 10;

                                if (StartIndex == Choices.Count)
                                {
                                    StartIndex = EndIndex - 9;
                                }
                            }

                            Logger.ClearFromLine(StartingCursorPosition + 1);
                        }
                        else if (SelectionIndex < StartIndex)
                        {
                            StartIndex = StartIndex - 10;
                            EndIndex = StartIndex + 9;

                            Logger.ClearFromLine(StartingCursorPosition + 1);
                        }

                        break;
                    }
                case KeyCode.VcDown:
                    {
                        if (Choices.Count == 1) break;
                        SelectionIndex++;

                        if (SelectionIndex > Choices.Count - 1)
                        {
                            SelectionIndex = 0;

                            StartIndex = 0;
                            EndIndex = (Choices.Count) > 10 ? 9 : Choices.Count - 1;

                            Logger.ClearFromLine(StartingCursorPosition + 1);
                        }
                        else if (SelectionIndex > EndIndex)
                        {
                            int IndexLeft = (Choices.Count - 1) - EndIndex;

                            StartIndex = EndIndex + 1;
                            EndIndex = (IndexLeft > 10) ? EndIndex + 10 : Choices.Count - 1;

                            Logger.ClearFromLine(StartingCursorPosition + 1);
                        }

                        break;
                    }
                case KeyCode.VcEnter:
                case KeyCode.VcRight:
                    {
                        Result = Choices[SelectionIndex];
                        return;
                    }
            }
        }

        private bool IsLoaded = false;

        public void Render()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                OnLoad();
            }

            OnViewing();
        }

        public SelectionChoice<T> Get()
        {
            while (true)
            {
                Render();

                if (Result != null) return Result;
            }
        }

        public void Reset()
        {
            SelectionIndex = 0;

            Result = null;

            StartingCursorPosition = -1;
            _TempSelectionIndex = -1;

            IsLoaded = false;
        }
    }
}
