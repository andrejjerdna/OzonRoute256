using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonRoute256
{
    public class TestProblemD
    {
        public static void Main(string[] args)
        {
            var s = Console.ReadLine();
            var t = Console.ReadLine();

            if (s == null || t == null)
                return;

            var count = s.Length;

            var dictS = GetCharDictionary(s);

            var result = new StringBuilder();

            for (var charNumber = 0; charNumber < count; charNumber++)
            {
                var sChar = s[charNumber];
                var tChar = t[charNumber];

                if (sChar == tChar)
                {
                    if (dictS.TryGetValue(tChar, out var val1))
                    {
                        if (val1 > 0)
                        {
                            result.Append('G');
                            dictS[tChar] = val1 - 1;
                            continue;
                        }
                        else
                        {
                            dictS.Remove(tChar);
                        }
                    }
                }

                result.Append(".");
            }

            for (var charNumber = 0; charNumber < count; charNumber++)
            {
                if (result[charNumber] == 'G')
                    continue;
                
                var tChar = t[charNumber];

                if (dictS.TryGetValue(tChar, out var val))
                {
                    if (val > 0)
                    {
                        result[charNumber] = 'Y';
                        dictS[tChar] = val - 1;
                        continue;
                    }
                    else
                    {
                        dictS.Remove(tChar);
                    }
                }
            }

            WriteLine(result.ToString());
        }

        private static IDictionary<char, int> GetCharDictionary(string input)
        {
            var result = new Dictionary<char, int>();

            foreach (var c in input)
            {
                if (result.TryGetValue(c, out var count))
                    result[c] = count + 1;
                else
                    result.Add(c, 1);
            }

            return result;
        }

        private static void WriteLine(object? output) => Console.WriteLine(output);
    }
}
