using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.CLI;

namespace wey.Pages
{
    public class Add : Command
    {
        public Add() : base("add")
        {
            this.Description = "add mods";

            this.Subcommand.Add(new AddCurseforge());
            this.Subcommand.Add(new AddModrinth());
        }

        public override void Execute()
        {
            
        }
    }
}
