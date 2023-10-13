using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Style;

namespace wey.Console
{
    class ChoiceValue
    {
        public string Name { get; set; }
        public ConsoleKey Key { get; set; }
        public string KeyName { get; set; }

        public ChoiceValue(string name, ConsoleKey key, string keyName)
        {
            this.Name = name;
            this.Key = key;
            KeyName = keyName;

        }
    }

    class Choice
    {
        private ChoiceValue[] values = Array.Empty<ChoiceValue>();

        private readonly ConsoleKey[] keys = new ConsoleKey[]
        {
            ConsoleKey.D1,
            ConsoleKey.D2,
            ConsoleKey.D3,
            ConsoleKey.D4,
            ConsoleKey.D5,
            ConsoleKey.D6,
            ConsoleKey.D7,
            ConsoleKey.D8,
            ConsoleKey.D9,
        };

        public Choice(string[] values)
        {
            this.values = new ChoiceValue[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                this.values[i] = new ChoiceValue(values[i], keys[i], (i + 1).ToString());
            }
        }

        public ChoiceValue Start()
        {
            foreach (ChoiceValue choice in values)
            {
                System.Console.WriteLine($"[{choice.KeyName}] {choice.Name}");
            }

            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey();

                foreach (ChoiceValue choice in values)
                {
                    if (key.Key == choice.Key)
                    {
                        return choice;
                    }
                }
            }
        }
    }
}
