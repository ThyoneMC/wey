using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPageGroup : IPageBase
    {
        private readonly Selection<string> Selector;

        public IPageBase? Selection
        {
            get
            {
                if (Selector.Result == null) return null;
                if (Selector.Result.Index == -1) return null;

                return GetPages()[Selector.Result.Index];
            }
        }

        public IPageGroup(params string[] args) : base(args)
        {
            Selector = Selection<string>.Create(
                            GetPages().Select(page => page.GetName())
                        );
        }

        public void Reset()
        {
            Selector.Reset();
        }

        public override void RenderNext()
        {
            Selector.RenderNext();
        }

        public override PageType GetPageType()
        {
            return PageType.Group;
        }

        public abstract IPageBase[] GetPages();
    }
}
