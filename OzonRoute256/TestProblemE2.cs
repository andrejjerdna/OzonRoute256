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
        private SortedSet<ServerThread> _threadPool;
        private int _threadCount;

        public IEnumerable<ServerTask> DoneTasks => _doneTasks;

        public void StartSession(int threadCount)
        {
            _threadCount = threadCount;
            _threadPool = new SortedSet<ServerThread>(new ThreadsComparer());
            _doneTasks = new List<ServerTask>();
        }

        public void AddTask(ServerTask serverTask)
        {
            if (_threadPool.Count < _threadCount)
            {
                var newThread = new ServerThread();
                newThread.AddTask(serverTask);
                _threadPool.Add(newThread);
            }
            else
            {
                var thread = _threadPool.Min;
                _threadPool.Remove(thread);
                thread.AddTask(serverTask);
                _threadPool.Add(thread);
            }

             _doneTasks.Add(serverTask);
        }

        public class ThreadsComparer : IComparer<ServerThread>
        {
            public int Compare(ServerThread? firstThread, ServerThread? secondThread)
            {
                if (secondThread == null || firstThread == null)
                    return 0;

                return secondThread.CompareTo(firstThread);
            }
        }
    }

    public class ServerThread : IComparable<ServerThread>
    {
        private int _endTime = 0;
        public int EndTime => _endTime;

        public Guid Guid = Guid.NewGuid();

        public void AddTask(ServerTask serverTask)
        {
            if (_endTime > serverTask.StartTime)
            {
                serverTask.EndTime = _endTime + serverTask.WorkTime;
                _endTime = serverTask.EndTime;

            }
            else
            {
                serverTask.EndTime = serverTask.StartTime + serverTask.WorkTime;
                _endTime = serverTask.EndTime;
            }
        }

        public int CompareTo(ServerThread? thread)
        {
            if (thread == null)
                return 1;

            if (Guid == thread.Guid)
                return 0;

            if (thread.EndTime > _endTime)
                return 1;
            else
                return -1;
        }
    }
}
