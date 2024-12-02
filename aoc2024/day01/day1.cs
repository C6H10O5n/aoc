using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2024
{
    internal partial class Program
    {
        static void day1()
        {           
            Console.WriteLine($"Answer1: {day1LogicPart1()}");
            Console.WriteLine($"Answer2: {day1LogicPart2()}");
        }

        static int day1LogicPart1() 
        {
            var l1 = new List<int>();var l2 = new List<int>();
            foreach (var s in d1_data)
            {
                var l = GetSpaceDelimDigitsAsListInt(s);
                l1.Add(l[0]);
                l2.Add(l[1]);
            }

            return l1.Order().Zip(l2.Order(),(a,b)=>Math.Abs(a-b)).Sum();
        }

        static int day1LogicPart2()
        {

            var l1 = new List<int>(); var l2 = new List<int>();
            foreach (var s in d1_data)
            {
                var l = GetSpaceDelimDigitsAsListInt(s);
                l1.Add(l[0]);
                l2.Add(l[1]);
            }
            var l2d = l2.Distinct().ToDictionary(x => x, x => l2.Count(lx => lx == x));

            return l1.Select(x => x * (l2d.ContainsKey(x)?l2d[x]:0)).Sum();
        }

        static string[] d1_data0 =
        """
        3   4
        4   3
        2   5
        1   3
        3   9
        3   3
        """.Split(Environment.NewLine);

    }
}
