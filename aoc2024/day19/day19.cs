using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        static void day19()
        {           
            Console.WriteLine($"Answer1: {day19LogicPart1()}");
            Console.WriteLine($"Answer2: {day19LogicPart2()}");
        }

        static long day19LogicPart1()
        {
            var data = d19_data;
            var tx = data[0].Split(", ");
            var designs = data[1].Split(Environment.NewLine).ToDictionary(x=>x, x=>false);
            int _ = 0;
            bool IsCorrect(string design, IEnumerable<string> searchList)
            {
                _++;
                if (design.Length == 0)
                {
                    return true;
                }
                
                foreach (string phrase in searchList)
                {
                    if (design.StartsWith(phrase))
                    {
                        if (IsCorrect(design[(phrase.Length)..], searchList))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }


            foreach (var dx0 in designs.Keys.Where(x => !designs[x]))
            {
               designs[dx0]=IsCorrect(dx0, tx);
            }
                       
            return designs.Values.Count(x=>x);
        }


        static long day19LogicPart2()
        {
            var data = d19_data;
            var tx = data[0].Split(", ");
            var designs = data[1].Split(Environment.NewLine).ToDictionary(x => x, x => 0L);
            
            var dCache = new Dictionary<string, long>();
            long IsCorrect(string design, IEnumerable<string> searchList)
            {
                if(dCache.ContainsKey(design))
                    return dCache[design];
                
                if (design.Length == 0)
                {
                    return 1;
                }

                long ccnt = 0;
                foreach (string phrase in searchList)
                {
                    if (design.StartsWith(phrase))
                    {
                        ccnt += IsCorrect(design[(phrase.Length)..], searchList);
                    }
                }

                dCache.Add(design,ccnt);
                return ccnt;
            }


            foreach (var dx0 in designs.Keys)
            {
                designs[dx0] = IsCorrect(dx0, tx);
            }

            return designs.Values.Sum(x => x);
        }

        static string[] d19_data0 =
        """        
        r, wr, b, g, bwu, rb, gb, br

        brwrr
        bggr
        gbbr
        rrbgbr
        ubwu
        bwurrg
        brgr
        bbrgwb
        """.Split(Environment.NewLine + Environment.NewLine);
    }
}
