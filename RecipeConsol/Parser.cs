using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CsQuery;

namespace RecipeConsole
{
    public class Parser
    {
        private static readonly Parser Instance = new Parser();

        public static void Add(ParserInfo info)
        {
            Instance.queue.Enqueue(info);
        }

        public static bool InWork
        {
            get { return Count > 0 || Instance.threads > 0; }
        }

        private int _threads;
        private int threads
        {
            get { return _threads; }
            set { _threads = value;
                if (value <= 0 && EmptyQueue != null) EmptyQueue(this, null);
            }
        }

        public static int Count
        {
            get { return Instance.queue.Count; }
        }

        private Queue<ParserInfo> queue = new Queue<ParserInfo>();
        List<string> urls = new List<string>();
        
        public static event EventHandler EmptyQueue;
        public static event EventHandler ParsingSuccess;

        private Parser()
        {
            var timer = new Timer(100);
            timer.Elapsed += (sender, e) =>
                {
                    //var self = sender as DispatcherTimer;
                    //if (self == null) return;
                    try
                    {
                        if (Instance.queue.Count > 0)
                        {
                            var info = Instance.queue.Dequeue();
                            Instance.Parse(info);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        //throw error
                    }

                };
            timer.Start();

            EmptyQueue += (sender, args) =>
                timer.Stop();

            ParsingSuccess += (sender, args) =>
                threads--;
        }

        private void Parse(ParserInfo info)
        {
            switch (info.type)
            {
                case ParserType.Global_2K:
                    Parse2KGlobal(info);
                    break;
            }

            if (ParsingSuccess != null) ParsingSuccess(info.url, null);
        }

        private void Parse2KGlobal(ParserInfo info)
        {
            threads++;
            Uri test = new Uri(info.url);
            var host = test.Host;

            try
            {
                var dom = CQ.CreateFromUrl(info.url);
                var links = dom["h2 a"];

                foreach (var link in links)
                    urls.Add(host + link["href"]);
            }
            catch (Exception)
            {

            }
        }
    }
}
