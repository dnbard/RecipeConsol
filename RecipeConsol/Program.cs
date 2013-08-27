using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace RecipeConsole
{
    class Program
    {
        static List<string> urls = new List<string>();

        static void Main(string[] args)
        {            
            Parser.EmptyQueue += (sender, a) =>
                Console.WriteLine("All Completed, {0} urls processed", urls.Count);

            Parser.ParsingSuccess += (sender, a) =>
                Console.WriteLine("{0} completed", sender as string);

            Parser.RegisterAction("global_2k", Parse2KGlobal);
            Parser.RegisterAction("local_2k", Parse2KLocal);
            

            for (int i = 1; i < 20; i++)
                Parser.Add(new ParserInfo { type = "global_2k", url = "http://eda.2k.ua/recept/drinks/kokteyli/page_" + i });

            while (Parser.InWork) { }
            Console.ReadKey();
        }

        private static bool Parse2KGlobal(ParserInfo info)
        {
            Uri test = new Uri(info.url);
            var host = string.Format("{0}://{1}", test.Scheme, test.Host);

            try
            {
                var dom = CQ.CreateFromUrl(info.url);
                var links = dom["h2 a"];

                foreach (var link in links)
                    //urls.Add(host + link["href"]);
                    Parser.Add(new ParserInfo { type = "local_2k", url = host + link["href"] });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool Parse2KLocal(ParserInfo info)
        {
            Uri test = new Uri(info.url);
            var host = test.Host;
            try 
            { 
                var dom = CQ.CreateFromUrl(info.url);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
