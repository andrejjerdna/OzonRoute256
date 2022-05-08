using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemB
    {
        public static void Main(string[] args)
        {
            var collectionCount = GetCount(Console.ReadLine());

            for (var collectionNumber = 0; collectionNumber < collectionCount; collectionNumber++)
            {
                var dataCount = GetCount(Console.ReadLine());

                var data = GetData(dataCount).Select((d, i) => new {val = d, index = i})
                                             .GroupBy(d => d.val)
                                             .OrderByDescending(g => g.Key);

                var result = GetEmptyResuls(dataCount);

                var priorityNumber = 1;
                var tempMaxValue = data.First().Key;

                foreach(var dataGroup in data)
                {
                    if (dataGroup.Key < tempMaxValue - 1)
                    {
                        tempMaxValue = dataGroup.Key;
                        priorityNumber++;
                    }

                    foreach (var d in dataGroup)
                        result[d.index] = priorityNumber;
                }

                WriteLine(result);
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

        private static void WriteLine(IEnumerable<int> output)
        {
            WriteLine(string.Join(" ", output.Select(i => i.ToString()).ToArray()));
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);

        private static IEnumerable<int> GetData(int count)
        {
            var result = new List<int>(count);
            result.AddRange(Console.ReadLine().Split(' ').Select(it => int.Parse(it)));
            return result;
        }

        private static IList<int> GetEmptyResuls(int count)
        {
            var result = new List<int>(count);

            for (var i = 0; i < count; i++)
                result.Add(0);

            return result;
        }
    }
}
