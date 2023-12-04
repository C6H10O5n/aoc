using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2023_02
{
    internal partial class Program
    {
        static void day1()
        {           
            Console.WriteLine($"Answer1: {day1Logic(false)}");
            Console.WriteLine($"Answer2: {day1Logic(true)}");
        }

        static int day1Logic(bool useWords) 
        {
            int sum = 0;
            foreach (var s in d1_data)
            {
                var sp = s.AsSpan();
                int d1 = 0, d2 = 0;

                //Find first digit from start
                for (int i = 0; d1 == 0 && i < sp.Length; i++)
                {
                    if (Char.IsAsciiDigit(sp[i])) { d1 = (int)Char.GetNumericValue(sp[i]); }
                    if (useWords) foreach (var k in nums.Keys) { if (sp[i..].StartsWith(k)) { d1 = nums[k][0]; break; } }
                }

                //Find 2nd digit from end
                for (int i = sp.Length - 1; d2 == 0 && i >= 0; i--)
                {
                    if (Char.IsAsciiDigit(sp[i])) { d2 = (int)Char.GetNumericValue(sp[i]); }
                    if (useWords) foreach (var k in nums.Keys) { if (sp[i..].StartsWith(k)) { d2 = nums[k][0]; break; } }
                }

                sum += d1 * 10 + d2;
            }
            return sum;
        }

        static Dictionary<string, int[]> nums = "one|1,two|2,three|3,four|4,five|5,six|6,seven|7,eight|8,nine|9".Split(",").ToDictionary(x => x.Split("|")[0], x => new int[] { int.Parse(x.Split("|")[1]), -1 });

        static string[] d1_data0 =
        """
        9cpjmdgf
        1eightwom
        bbm4twoeight8oneone3one
        threenine3
        1111
        two1nine
        eightwothree
        abcone2threexyz
        xtwone3four
        4nineeightseven2
        zoneight234
        7pqrstsixteen
        """.Split("\r\n");

    }
}
