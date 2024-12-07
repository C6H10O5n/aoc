using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace aoc2024
{
    internal partial class Program
    {

        static void day7()
        {           
            Console.WriteLine($"Answer1: {day7LogicPart1()}");
            Console.WriteLine($"Answer2: {day7LogicPart2()}");
        }
                

        static long day7LogicPart1() 
        {
            var opers = new List<char>() {'+','*' };

            var sumGood = (long)0;
            var xans = (long)0;
            foreach (var s in d7_data)
            {
                var ans = long.Parse(s.Split(':')[0]);
                var nms = GetSpaceDelimDigitsAsListInt(s.Split(':')[1].Trim());
                var ops = opers.DifferentPermutations(nms.Count()-1).Select(x=>x.ToArray()).ToArray();

                foreach (var op in ops)
                {
                    xans = nms[0];
                    for (int i = 1; i < nms.Count() && xans <= ans; i++)
                    {
                        xans =  op[i - 1] == '*' ? xans * nms[i]
                              : op[i - 1] == '+' ? xans + nms[i]
                              : 0;
                    }
                    if (xans == ans)
                    {
                        sumGood+=ans;
                        break;
                    }
                }
                                
            }

            return sumGood;
        }

        static long day7LogicPart2()
        {

            var opers = new List<char>() { '+', '*', '|' };

            var sumGood = (long)0;
            var xans = (long)0;
            foreach (var s in d7_data)
            {
                var ans = long.Parse(s.Split(':')[0]);
                var nms = GetSpaceDelimDigitsAsListInt(s.Split(':')[1].Trim());
                var ops = opers.DifferentPermutations(nms.Count() - 1).Select(x => x.ToArray()).ToArray();

                foreach (var op in ops)
                {
                    xans = nms[0];
                    for (int i = 1; i < nms.Count() && xans <= ans; i++)
                    {
                        xans =  op[i - 1] == '*' ? xans * nms[i]
                              : op[i - 1] == '+' ? xans + nms[i]
                              : op[i - 1] == '|' ? long.Parse(xans.ToString() + nms[i].ToString())
                              : 0;
                    }
                    if (xans == ans)
                    {
                        sumGood += ans;
                        break;
                    }
                }

            }

            return sumGood;
        }

        static string[] d7_data0 =
        """        
        190: 10 19
        3267: 81 40 27
        83: 17 5
        156: 15 6
        7290: 6 8 6 15
        161011: 16 10 13
        192: 17 8 14
        21037: 9 7 18 13
        292: 11 6 16 20
        """.Split(Environment.NewLine);

    }
}
