using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemE2
    {
        private static int _readCount;
        private static string[] _args;
        private static SyncServer _server;
        public static void Main(string[] args)
        {
            _server = new SyncServer();

            var testFiles = GetTestFiles();

            if (testFiles == null || !testFiles.Any())
            {
                var dataCount = GetCount(ReadLine(_args));
                GetTasks(dataCount);
            }
            else
            {
                foreach (var file in testFiles)
                {
                    _readCount = 0;
                    _args = GetDataFromFile(file);
                    var dataCount = GetCount(ReadLine(_args));
                    GetTasks(dataCount);
                }
            }
        }

        private static IEnumerable<string>? GetTestFiles()
        {
            try
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "tests", nameof(TestProblemE2));
                return Directory.EnumerateFiles(dir);
            }
            catch
            {
                return null;
            }
        }

        private static string[] GetDataFromFile(string file)
        {
            return File.ReadAllLines(file);
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
                var threadLine = ReadLine(_args).Split(' ').Select(it => int.Parse(it));

                var threadCount = threadLine.First();
                var dataCount = threadLine.Last();

                _server.StartSession(threadCount);

                for (int j = 0; j < dataCount; j++)
                {
                    var line = ReadLine(_args).Split(' ').Select(it => int.Parse(it));
                    _server.AddTask(new ServerTask(line.First(), line.Last()));
                }

                _server.SessionWait();

                WriteLine(_server.DoneTasks);
            }
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);

        private static void WriteLine(IEnumerable<ServerTask> output)
        {
            const string separator = " ";
            var stringBuilder = new StringBuilder();

            foreach (var task in output)
            {
                stringBuilder.Append(task.EndTime);
                stringBuilder.Append(separator);
            }

            WriteLine(stringBuilder.ToString());
        }
    }

    public class ServerTask
    {
        public int StartTime { get; set; }
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
        private List<ServerTask> _doneTasks;
        private IEnumerable<ServerThread> _threadPool;
        private Queue<ServerTask> _tasks;

        public IEnumerable<ServerTask> DoneTasks => _doneTasks;

        public void StartSession(int threadCount)
        {
            _threadPool = GetThreadPool(threadCount);
            _tasks = new Queue<ServerTask>();
            _doneTasks = new List<ServerTask>();
        }

        public void SessionWait()
        {
            CheckQueue();
        }

        public void AddTask(ServerTask serverTask)
        {
            CheckQueue();

            var getTaskSatus = false;

            var minEndTimes = int.MaxValue;

            foreach (var thread in _threadPool)
            {
                getTaskSatus = thread.AddTask(serverTask);

                if (minEndTimes > thread.EndTime)
                    minEndTimes = thread.EndTime;

                if (getTaskSatus)
                    break;
            }

            if (!getTaskSatus)
            {
                serverTask.StartTime = minEndTimes;
                _tasks.Enqueue(serverTask);
            }
            else
            {
                _doneTasks.Add(serverTask);
            }
        }

        private IEnumerable<ServerThread> GetThreadPool(int threadCount)
        {
            var result = new List<ServerThread>();

            for (int i = 0; i < threadCount; i++)
                result.Add(new ServerThread());

            return result;
        }

        private void CheckQueue()
        {
            if (!_tasks.Any())
                return;

            for (var i = 0; i < _tasks.Count; i++)
                AddTask(_tasks.Dequeue());
        }
    }

    public class ServerThread
    {
        private int _endTime;

        public int EndTime => _endTime;

        public bool AddTask(ServerTask serverTask)
        {
            if (_endTime > serverTask.StartTime)
                return false;

            if (_endTime <= serverTask.StartTime)
            {
                serverTask.EndTime = serverTask.StartTime + serverTask.WorkTime;
                _endTime = serverTask.EndTime;
            }

            return true;
        }
    }
}
