using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPageGroup : IPage
    {
        private Selection<string> Selector = Selection<string>.Create();

        public IPage? Selection
        {
            get
            {
                if (Selector.Result == null) return null;
                if (Selector.Result.Index == -1) return null;

                return GetPages().ElementAt(Selector.Result.Index);
            }
        }

        public void Reset()
        {
            IsLoaded = false;

            Selector.Reset();
        }

        public override PageType GetPageType()
        {
            return PageType.Group;
        }

        public override void Render()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                IEnumerable<IPage> Pages = GetPages();

                if (!Pages.Any())
                {
                    IsExit = true;
                    return;
                }

                Selector = Selection<string>.Create(
                            Pages.Select(page => page.GetName())
                        );
            }

            Selector.RenderNext();
        }

        public abstract IEnumerable<IPage> GetPages();
    }
}
