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
        View,
        Command,
        Group
    }

    abstract class IPage
    {
        public bool IsLoaded { get; protected set; }

        public bool IsExit = false;

        public abstract PageType GetPageType();

        public abstract string GetName();

        public abstract string GetDescription();

        public abstract void Render();
    }
}
