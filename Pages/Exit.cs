using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Interface;

namespace wey.Pages
{
    class Exit : IPageCommand
    {
        public override string GetName()
        {
            return "exit";
        }

        public override string GetDescription()
        {
            return "exit";
        }

        public override void OnCommand()
        {
            if (Executable.Count > 0)
            {
                Logger.WriteMultiple(
                        $"There are {Executable.Count} process running",
                        string.Empty,
                        "Press [Shift, Control, Alt, Space] at same time to return"
                    );

                while (!(
                        KeyReader.GetHoverOnce(KeyCode.VcLeftShift, KeyCode.VcRightShift) &&
                        KeyReader.GetHoverOnce(KeyCode.VcLeftControl, KeyCode.VcRightControl) &&
                        KeyReader.GetHoverOnce(KeyCode.VcLeftAlt, KeyCode.VcRightAlt) &&
                        KeyReader.GetHoverAll(KeyCode.VcSpace)
                        ))
                {

                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
