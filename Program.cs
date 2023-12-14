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

        private static IPageBase CurrentPage = new Main();

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

        public static void Main(string[] args)
        {
            AddPage(new Main());

            new TaskWorker(() =>
            {
                if (PageList.Count > 1 && KeyReader.Get() == KeyCode.VcLeft)
                {
                    PageList.Pop();

                    ChangePage();
                    return;
                }

                if (CurrentPage.GetPageType() == PageType.Group)
                {
                    IPageGroup Group = (IPageGroup)CurrentPage;

                    Group.RenderNext();

                    if (Group.Selection != null)
                    {
                        AddPage(Group.Selection);

                        Group.Reset();

                        return;
                    }
                }
                else
                {
                    CurrentPage.RenderNext();
                }
            }).Start();
        }
    }
}