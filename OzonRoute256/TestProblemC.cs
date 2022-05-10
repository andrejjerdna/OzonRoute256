using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemC
    {
        public static void Main(string[] args)
        {
            var k = GetCount(Console.ReadLine());
            var data = GetData(k);

            if (data == null)
                return;

            var result = GetEmptyResuls(k + 1);

            var lastValue = 0;
            var list = new List<int>();

            for (var currentNumber = 0; currentNumber < k + 1; currentNumber++)
            {
                var sum = data.Skip(currentNumber).Sum();
                result[currentNumber] = Math.Abs(sum);
            }

            list.Add(k);

            foreach(var index in list)
                result[index] = lastValue;

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
            result.AddRange(Console.ReadLine().Split(' ').Select(it => int.Parse(it)));
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
