using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    enum PageType
    {
        Page,
        Command,
        Group
    }

    abstract class IPageBase
    {
        protected string[] arguments;

        public bool IsLoaded { get; protected set; }

        public IPageBase(params string[] args)
        {
            arguments = args;
        }

        public abstract void RenderNext();

        public abstract PageType GetPageType();

        public abstract string GetName();

        public abstract string GetDescription();
    }
}
