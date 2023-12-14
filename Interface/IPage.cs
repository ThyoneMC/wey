using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPage : IPageBase
    {
        public IPage(params string[] args) : base(args)
        {
            
        }

        public override void RenderNext()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;
                KeyReader.TimestampRange = 10000;

                OnLoad();
            }

            OnViewing();
        }

        public override PageType GetPageType()
        {
            return PageType.Page;
        }

        public abstract void OnLoad();

        public abstract void OnViewing();
    }
}
