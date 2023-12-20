using SharpHook.Native;
using System.Diagnostics;
using System.Text.RegularExpressions;
using wey.Console;
using wey.Global;
using wey.Interface;
using wey.Pages;

namespace wey
{
    class Program
    {
        private static readonly Stack<IPageBase> PageList = new();

        private static IPageBase CurrentPage = new Home();

        private static void AddPage(IPageBase page)
        {
            PageList.Push(page);

            ChangePage();
        }

        private static void ChangePage()
        {
            System.Console.Clear();

            CurrentPage = PageList.First();

            Logger.WriteMultiple(
                    string.Join(" / ", PageList.Reverse().Select(p => p.GetName())),
                    new String('-', 10),
                    string.Empty
                );
        }

        private static void ReturnPage()
        {
            PageList.Pop();

            ChangePage();
        }

        public static void Main(string[] args)
        {
            System.Console.CursorVisible = false;

            new Panel();

            return;

            var c = new ConsoleReader();

            while (true)
            {
                c.RenderNext();

                Logger.WriteSingle("aiusdh918");
            }

            return;

            ExecutableArgument.Import(args);

            AddPage(new Home());
            new TaskWorker(() =>
            {
                if (PageList.Count > 1 && KeyReader.Get() == KeyCode.VcLeft)
                {
                    ReturnPage();
                    return;
                }

                switch (CurrentPage.GetPageType())
                {
                    case PageType.Group:
                        {
                            IPageGroup Group = (IPageGroup)CurrentPage;

                            Group.RenderNext();

                            if (Group.Selection != null)
                            {
                                AddPage(Group.Selection);

                                Group.Reset();

                                return;
                            }

                            break;
                        }
                    case PageType.Command:
                        {
                            IPageCommand Command = (IPageCommand)CurrentPage;

                            Command.RenderNext();

                            ReturnPage();
                            break;
                        }
                    case PageType.Page:
                        {
                            CurrentPage.RenderNext();
                            break;
                        }
                }
            }).Start();
        }
    }
}