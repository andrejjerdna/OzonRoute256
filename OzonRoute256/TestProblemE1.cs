using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemE1
    {
        private static int _readCount;
        private static string[] _args;
        private static SyncServer _server;
        public static void Main(string[] args)
        {
            _server = new SyncServer();
            _readCount = 0;

            //Данные для локального теста.
            //_args = new string[]
            //{
            //    "4", 
            //    "", 
            //    "3", 
            //    "1 10", 
            //    "5 10", 
            //    "100 7",
            //    "", 
            //    "2",
            //    "1 10000",
            //    "2 10000",
            //    "",
            //    "1",
            //    "1000000 10000",
            //    "",
            //    "7",
            //    "5 7",
            //    "6 1",
            //    "7 4",
            //    "20 1",
            //    "21 5",
            //    "22 2",
            //    "29 2"
            //};

            var dataCount = GetCount(ReadLine(_args));

            GetTasks(dataCount);
        }

        private static string? ReadLine(string[] args)
        {
            if (args == null || !args.Any())
            {
                return Console.ReadLine();
            }
            else
            {
                var arg = args[_readCount];
                _readCount++;
                return arg;
            }
        }

        private static int GetCount(string? input)
        {
            if (int.TryParse(input, out var collectionCount))
            {
                return collectionCount;
            }

            throw new Exception("Bad count");
        }

        private static void GetTasks(int dataSetCount)
        {
            for (int i = 0; i < dataSetCount; i++)
            {
                ReadLine(_args);
                var dataCount = GetCount(ReadLine(_args));

                for (int j = 0; j < dataCount; j++)
                {
                    var line = ReadLine(_args).Split(' ').Select(it => int.Parse(it));
                    _server.AddTask(new ServerTask(line.First(), line.Last()));
                }

                WriteLine(_server.DoneTasks);
                _server.ClearDoneTasks();
            }
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);

        private static void WriteLine(IEnumerable<ServerTask> output)
        {
            WriteLine(string.Join(" ", output.Select(t => t.EndTime.ToString()).ToArray()));
        }
    }

    public class ServerTask
    {
        public int StartTime { get; }
        public int WorkTime { get; }
        public int EndTime { get; set; }

        public ServerTask(int t, int d)
        {
            StartTime = t;
            WorkTime = d;
        }
    }

    public class SyncServer
    {
        private readonly Queue<ServerTask> _tasks;
        private readonly List<ServerTask> _doneTasks;

        private int _endTime;

        public IEnumerable<ServerTask> DoneTasks => _doneTasks;

        public SyncServer()
        {
            _tasks = new Queue<ServerTask>();
            _doneTasks = new List<ServerTask>();
            _endTime = 0;
        }

        public void AddTask(ServerTask serverTask)
        {
            if(_endTime <= serverTask.StartTime)
            {
                serverTask.EndTime = serverTask.StartTime + serverTask.WorkTime;
                _endTime = serverTask.EndTime;
            }
            else
            {
                serverTask.EndTime = _endTime + serverTask.WorkTime;
                _endTime = serverTask.EndTime;
            }

            _doneTasks.Add(serverTask);
        }

        public void ClearDoneTasks()
        {
            _doneTasks.Clear();
            _endTime = 0;
        }
    }
}
