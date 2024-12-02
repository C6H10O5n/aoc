using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2024
{
    internal partial class Program
    {
        static void day2()
        {           
            Console.WriteLine($"Answer1: {day2LogicPart1()}");
            Console.WriteLine($"Answer2: {day2LogicPart2()}");
        }

        static int day2LogicPart1() 
        {
            var cntGood = 0;
            foreach (var s in d2_data)
            {
                var l = GetSpaceDelimDigitsAsListInt(s);
                var ld = l.Skip(1).Zip(l, (a, b) => a - b);
                if ((ld.Min() >= -3 && ld.Max() <= -1)|| (ld.Min() >= 1 && ld.Max() <= 3))
                    cntGood++;
            }

            return cntGood;
        }

        static int day2LogicPart2()
        {

            var cntGood = 0;
            foreach (var s in d2_data)
            {
                var l = GetSpaceDelimDigitsAsListInt(s);
                var ld = l.Skip(1).Zip(l, (a, b) => a - b);
                if ((ld.Min() >= -3 && ld.Max() <= -1) || (ld.Min() >= 1 && ld.Max() <= 3))
                {
                    cntGood++;
                }
                else
                {
                    for(int i=0; i<l.Count(); i++)
                    {
                        var l2 = l.Where((v, idx) => idx!=i);
                        var ld2 = l2.Skip(1).Zip(l2, (a, b) => a - b);
                        if ((ld2.Min() >= -3 && ld2.Max() <= -1) || (ld2.Min() >= 1 && ld2.Max() <= 3))
                        {
                            cntGood++;
                            break;
                        }
                    }
                }
            }

            return cntGood;
        }

        static string[] d2_data0 =
        """
        7 6 4 2 1
        1 2 7 8 9
        9 7 6 2 1
        1 3 2 4 5
        8 6 4 4 1
        1 3 6 7 9
        """.Split(Environment.NewLine);

    }
}
