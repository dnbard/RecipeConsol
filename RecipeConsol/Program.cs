using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.EmptyQueue += (sender, a) =>
                Console.WriteLine("All Completed");

            Parser.ParsingSuccess += (sender, a) =>
                Console.WriteLine("{0} completed", sender as string);
            

            for (int i = 1; i < 20; i++)
                Parser.Add(new ParserInfo { type = ParserType.Global_2K, url = "http://eda.2k.ua/recept/drinks/kokteyli/page_" + i });

            while (Parser.InWork) { }
            Console.ReadKey();
        }
    }
}
