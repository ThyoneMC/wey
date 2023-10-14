using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Console
{
    class ChoiceValue
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public ChoiceValue(string name, string id)
        {
            this.Name = name;
            this.ID = id;
        }
    }

    class Choice
    {
        public static string Start(string[] values)
        {
            ChoiceValue[] choices = new ChoiceValue[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                choices[i] = new ChoiceValue(values[i], (i + 1).ToString());
            }

            return Start(choices).Name;
        }

        public static ChoiceValue Start(ChoiceValue[] values)
        {
            while (true)
            {
                foreach (ChoiceValue choice in values)
                {
                    System.Console.WriteLine($"[{choice.ID}] {choice.Name}");
                }

                System.Console.Write("> ");
                string key = Input.ReadString();

                foreach (ChoiceValue choice in values)
                {
                    if (choice.ID == key)
                    {
                        return choice;
                    }

                    if (choice.Name == key)
                    {
                        return choice;
                    }
                }

                System.Console.Clear();
            }
        }
    }
}
