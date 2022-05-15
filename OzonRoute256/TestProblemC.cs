using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemC
    {
        private static int _readCount;
        private static string[] _args;

        public static void Main(string[] args)
        {
            var testFiles = GetTestFiles();

            if (testFiles == null || !testFiles.Any())
            {
                Start();
            }
            else
            {
                foreach (var file in testFiles)
                {
                    _readCount = 0;
                    _args = GetDataFromFile(file);
                    Start();
                }
            }            
        }

        private static IEnumerable<string>? GetTestFiles()
        {
            try
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "tests", nameof(TestProblemC));
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

        private static void Start()
        {
            var k = GetCount(ReadLine(_args));
            var data = GetData(k);

            if (data == null)
                return;

            var result = GetEmptyResuls(k + 1);

            var list = new List<int>();

            var beNull = false;
            var last = 0;

            for (var currentNumber = 0; currentNumber < k; currentNumber++)
            {
                var sum = data.Skip(currentNumber).Sum();
                
                if (sum > 0 && !beNull && currentNumber == 0)
                {
                    result[currentNumber] = 0;
                    currentNumber--;
                    beNull = true;
                    last = 1;
                    continue;
                }

                if (sum == 0 && !beNull && k == 2 && currentNumber == 0)
                {
                    var nextSum = Math.Abs(data.Skip(currentNumber + 1).Sum());
                        result[currentNumber] = nextSum;

                    beNull = true;
                    last++;

                    currentNumber--;
                    continue;
                }

                result[currentNumber + last] = Math.Abs(sum);
            }

            list.Add(k);

            WriteLine(result);
        }

        private static int GetCount(string? input)
        {
            if (int.TryParse(input, out var collectionCount))
            {
                return collectionCount;
            }

            throw new Exception("Bad count");
        }

        private static void WriteLine(IEnumerable<int> output)
        {
            WriteLine(string.Join(" ", output.Select(i => i.ToString()).ToArray()));
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);

        private static IEnumerable<int> GetData(int count)
        {
            var result = new List<int>(count);
            result.AddRange(ReadLine(_args).Split(' ').Select(it => int.Parse(it)));
            return result;
        }

        private static List<int> GetEmptyResuls(int count)
        {
            var result = new List<int>(count);

            for (var i = 0; i < count; i++)
                result.Add(0);

            return result;
        }
    }
}
