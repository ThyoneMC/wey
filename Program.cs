using SharpHook.Native;
using System.Diagnostics;
using System.Text.RegularExpressions;
using wey.Console;
using wey.Global;
using wey.Host.Provider;
using wey.Interface;
using wey.Pages;

namespace wey
{
    class Program
    {
        private static readonly Stack<IPage> PageList = new();

        private static IPage CurrentPage = new Home();

        private static void AddPage(IPage page)
        {
            PageList.Push(page);

            ChangePage();
        }

        private static void ChangePage()
        {
            System.Console.Clear();

            CurrentPage = PageList.First();

            Logger.WriteMultiple(
                    string.Empty,
                    string.Join(" / ", PageList.Reverse().Select(p => p.GetName())),
                    string.Empty,
                    new string('-', System.Console.WindowWidth),
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
            ExecutableArgument.Import(args);

            AddPage(new Home());

            new TaskWorker(() =>
            {
                PageType CurrentType = CurrentPage.GetPageType();

                if 
                (
                    (CurrentPage.IsExit) ||
                    (PageList.Count > 1 && KeyReader.Get() == KeyCode.VcLeft)
                )
                {
                    if (CurrentType == PageType.View) ((IPageView)CurrentPage).OnExit();

                    ReturnPage();
                    return;
                }

                switch (CurrentType)
                {
                    case PageType.Group:
                        {
                            IPageGroup Group = (IPageGroup)CurrentPage;

                            Group.Render();

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

                            Command.Render();
                            Thread.Sleep(1000);

                            ReturnPage();
                            break;
                        }
                    case PageType.View:
                        {
                            IPageView Page = (IPageView)CurrentPage;

                            Page.Render();

                            break;
                        }
                }
            }).Start();
        }
    }
}