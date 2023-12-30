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

        public IPageGroup(params object[] args) : base(args)
        {

        }

        public void Reset()
        {
            IsLoaded = false;

            Selector.Reset();
        }

        public override void RenderNext()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                Selector = Selection<string>.Create(
                            GetPages().Select(page => page.GetName())
                        );
            }

            Selector.RenderNext();
        }

        public override PageType GetPageType()
        {
            return PageType.Group;
        }

        public abstract IEnumerable<IPage> GetPages();
    }
}
