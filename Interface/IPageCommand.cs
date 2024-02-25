using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPageCommand : IPage
    {
        public override PageType GetPageType()
        {
            return PageType.Command;
        }

        public override void Render()
        {
            IsLoaded = true;

            OnCommand();
        }

        public abstract void OnCommand();
    }
}
