using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Interface
{
    abstract class IPageView : IPage
    {
        public override PageType GetPageType()
        {
            return PageType.View;
        }

        public override void Render()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;
                KeyReader.TimestampRange = 17500;

                OnLoad();
            }

            OnViewing();
        }

        public abstract void OnLoad();

        public abstract void OnViewing();

        public abstract void OnExit();
    }
}
