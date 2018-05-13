using System.Collections.Generic;
using System;

namespace progLab2
{
    public class Menu
    {
        public List<MenuItem> Items;
        private string title;
        

        public Menu(string title)
        {
            Items = new List<MenuItem>();
            this.title = title;
            
        }

        public void Display()
        {
            MenuItem result;

            do
            {
                foreach (MenuItem item in Items)
                {
                    Console.WriteLine($"{item.Key}. {item.Label}");
                }

                string key;

                do
                {
                    Console.Write(">");
                    key = Console.ReadLine().Trim().ToLower();
                } while (key.Length != 1 || Items.Find(search => search.Key == key[0]) == null);

                Console.WriteLine("");

                result = Items.Find(search => search.Key == key[0]);
            } while (result.Function());
        }
    }
}