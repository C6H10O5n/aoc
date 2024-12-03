using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2024
{
    internal partial class Program
    {
        static void day3()
        {           
            Console.WriteLine($"Answer1: {day3LogicPart1()}");
            Console.WriteLine($"Answer2: {day3LogicPart2()}");
        }

        static int day3LogicPart1() 
        {

            var rx_p1 = new Regex("\\d{1,3}");
            var rx_p2 = new Regex("^\\d{1,3},\\d{1,3}\\)");

            var sum = 0;
            foreach (var s in d3_data)
            {
                foreach (var sm in s.Split("mul("))
                {
                    if (rx_p2.IsMatch(sm))
                    {
                        var vals = GetCommaDelimDigitsAsListInt(sm.Split(")")[0]);
                        sum += vals[0] * vals[1];
                    }
                }

            }

            return sum;
        }

        static int day3LogicPart2()
        {
            var rx_p1 = new Regex("\\d{1,3}");
            var rx_p2 = new Regex("^\\d{1,3},\\d{1,3}\\)");

            var mulDo = true;
            var sum = 0;
            foreach (var s in d3_data)
            {
                foreach (var sm in s.Split("mul("))
                {
                    if (mulDo && rx_p2.IsMatch(sm))
                    {
                        var vals = GetCommaDelimDigitsAsListInt(sm.Split(")")[0]);
                        sum += vals[0] * vals[1];
                    }

                    if (mulDo && sm.Contains("don't()"))
                    {
                        mulDo = false;
                        if (sm.Contains("do()") && sm.Split("don't()").Last().Contains("do()"))
                        {
                            mulDo = true;
                        }
                    }
                    else if(!mulDo && sm.Contains("do()"))
                    {
                        mulDo = true;
                        if (sm.Contains("don't()") && sm.Split("do()").Last().Contains("don't()"))
                        {
                            mulDo = false;
                        }
                    }
                }

            }

            return sum;
        }

        static string[] d3_data0 =
        """        
        xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
        xmul(2,4)&mul[3,7]!^don't()fffdo()kkk_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
        xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
        """.Split(Environment.NewLine);

    }
}
