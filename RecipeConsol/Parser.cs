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
            get { return Count > 0 || Instance.ThreadsCount > 0; }
        }

        private int _threads;
        private int ThreadsCount
        {
            get { return _threads; }
            set
            {
                _threads = value;
                if (value <= 0 && EmptyQueue != null) EmptyQueue(this, null);
            }
        }

        public static int Count
        {
            get { return Instance.queue.Count; }
        }

        private Queue<ParserInfo> queue = new Queue<ParserInfo>();

        public static event EventHandler EmptyQueue;
        public static event EventHandler ParsingSuccess;

        Dictionary<string, Func<ParserInfo, bool>> Actions = new Dictionary<string, Func<ParserInfo, bool>>();

        public static void RegisterAction(string name, Func<ParserInfo, bool> action)
        {
            if (Instance.Actions.ContainsKey(name))
                Instance.Actions[name] = action;
            else Instance.Actions.Add(name, action);
        }

        Timer timer;

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set 
            { 
                _interval = value;
                if (timer != null)
                    timer.Interval = value;
            }
        }

        private Parser(int interval = 100)
        {
            _interval = interval;
            timer = new Timer(interval);
            timer.Elapsed += (sender, e) =>
                {
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
                    }

                };
            timer.Start();

            EmptyQueue += (sender, args) =>
                timer.Stop();

            ParsingSuccess += (sender, args) =>
                ThreadsCount--;
        }

        private void Parse(ParserInfo info)
        {
            if (Actions.ContainsKey(info.type))
            {
                ThreadsCount++;
                if (Actions[info.type](info) && ParsingSuccess != null) 
                    ParsingSuccess(info.url, null);
            }
        }
    }
}
