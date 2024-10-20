using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.CLI
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CommandOptionsType
    {
        Boolean,
        String,
        Integer,
        StringArray,
        IntegerArray
    }

    public class CommandOptions
    {
        public required string Name { get; set; }
        public required CommandOptionsType Type { get; set; }
        public bool InConfigFile { get; set; } = false;
        public bool Optional { get; set; } = false;
    }

    public abstract class Command
    {
        public string Name;
        public string? Description = null;
        public readonly List<CommandOptions> Options = new();
        public readonly List<Command> Subcommand = new();

        protected Command(string name)
        {
            this.Name = name;
        }

        public abstract void Execute();
    }
}
