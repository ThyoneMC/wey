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
        public T Value { get; set; } = FileController<T>.CreateTypeInstance();
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
            return new Selection<T>(choices);
        }

        public static Selection<T> Create(params T[] choices)
        {
            return new Selection<T>(choices);
        }

        public static Selection<T> Create(SelectionChoice<T>[] choices)
        {
            return new(Array.Empty<T>())
            {
                Choices = choices.ToList()
            };
        }

        public SelectionChoice<T>? Result = null;

        private int StartingCursorPosition = -1;
        private int _TempSelectionIndex = -1;

        private void OnLoad()
        {
            System.Console.CursorVisible = false;

            StartingCursorPosition = System.Console.GetCursorPosition().Top;

            KeyReader.TimestampRange = 5000;
        }

        private void OnViewing()
        {
            if (_TempSelectionIndex != SelectionIndex)
            {
                _TempSelectionIndex = SelectionIndex;

                System.Console.SetCursorPosition(0, StartingCursorPosition);
                foreach (SelectionChoice<T> choice in Choices)
                {
                    string BaseString = $"[{choice.Index + 1}] {choice.Value}";

                    if (choice.Index == SelectionIndex)
                    {
                        Logger.WriteSingle($" > {BaseString}");
                    }
                    else
                    {
                        Logger.WriteSingle($"   {BaseString}");
                    }
                }
            }

            //selector

            switch (KeyReader.Get())
            {
                case KeyCode.VcUp:
                case KeyCode.VcW:
                    {
                        SelectionIndex--;

                        break;
                    }
                case KeyCode.VcDown:
                case KeyCode.VcS:
                    {
                        SelectionIndex++;

                        break;
                    }
                case KeyCode.VcEnter:
                case KeyCode.VcTab:
                    {
                        Result = Choices[SelectionIndex];
                        return;
                    }
            }

            if (SelectionIndex > Choices.Count - 1)
            {
                SelectionIndex = 0;
            }

            if (SelectionIndex < 0)
            {
                SelectionIndex = Choices.Count - 1;
            }
        }

        private bool IsLoaded = false;

        public void RenderNext()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                OnLoad();
            }

            OnViewing();
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
