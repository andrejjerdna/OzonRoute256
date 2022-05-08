using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemA
    {
        public static void Main(string[] args)
        {
            if (int.TryParse(Console.ReadLine(), out var count))
            {
                for (var i = 0; i < count; i++)
                {
                    var sum = GetSum(Console.ReadLine());
                    WriteLine(sum);
                }
            }
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);

        private static int? GetSum(string? input)
        {
            return input?.Split(' ').Select(it => int.Parse(it)).Sum();
        }
    }
}
