using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.CLI
{
    public enum IHelpOptionsType
    {
        Boolean,
        String,
        Int,
        StringArray,
        IntArray
    }

    public class IHelpOptions
    {
        public required string Name { get; set; }
        public required IHelpOptionsType Type { get; set; }
        public bool IsConfig { get; set; } = false;
    }

    public class IHelpCommand
    {
        public string? Description { get; set; } = null;
        public IHelpOptions[] Options { get; set;} = Array.Empty<IHelpOptions>();
    }

    public abstract class Command
    {
        public abstract string GetName();

        public abstract IHelpCommand GetHelp();

        public abstract Command[] GetSubCommand();

        public abstract void Execute();
    }
}
