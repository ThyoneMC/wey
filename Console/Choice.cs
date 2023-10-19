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
        public static int StartGetIndex(params string[] values)
        {
            return int.Parse(Start(values).ID) - 1;
        }

        public static string StartGetValue(params string[] values)
        {
            return Start(values).Name;
        }

        public static ChoiceValue Start(string[] values)
        {
            ChoiceValue[] choices = new ChoiceValue[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                choices[i] = new ChoiceValue(values[i], (i + 1).ToString());
            }

            return Start(choices);
        }

        public static ChoiceValue Start(ChoiceValue[] values)
        {
            if (values.Length == 0) throw new Exception($"values is empty");
            if (values.Length == 1) return values[0];

            foreach (ChoiceValue choice in values)
            {
                System.Console.WriteLine($"[{choice.ID}] {choice.Name}");
            }

            while (true)
            {
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
            }
        }
    }
}
