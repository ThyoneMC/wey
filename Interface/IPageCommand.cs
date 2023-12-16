using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPageCommand : IPageBase
    {
        public IPageCommand(params string[] args) : base(args)
        {

        }

        public override void RenderNext()
        {
            IsLoaded = true;

            OnCommand();
        }

        public override PageType GetPageType()
        {
            return PageType.Command;
        }

        public abstract void OnCommand();
    }
}
