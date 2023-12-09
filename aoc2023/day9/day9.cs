using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {
        class pc9Sequence
        {
            public pc9Sequence(string s) 
            {
                nums = s.Split(' ').Select(x=>int.Parse(x)).ToArray();
                
                //Run Solve
                solve();

                //GetNext
                GetNext = seqLadderForecast.Sum();

               //GetPrev
                var sx = seqLadderBackcast.ToList();
                for (int i = sx.Count - 2; i >= 0; i--)
                    sx[i] = seqLadderBackcast[i] - sx[i + 1];
                GetPrev = sx.First();

                //GetNext =  Enumerable.Range(1, seqLadder.Count - 1).Reverse().Select(i => seqLadder[i] + seqLadder[i - 1]).Last();
            }
            public int[] nums { get; set; }
            public int GetNext { get; private set; }
            public int GetPrev { get; private set; }


            List<int> seqLadderForecast = null;
            List<int> seqLadderBackcast = null;
            public void solve()
            {
                var sf = new List<int>();
                var sb = new List<int>();

                //solve sequence ladder
                var n1 = nums.ToList();
                var n2 = new List<int>();
                sf.Clear();
                sf.Add(n1.Last());
                sb.Clear();
                sb.Add(n1.First());
                do
                {
                    n2 = Enumerable.Range(1, n1.Count - 1).Select(i => n1[i] - n1[i - 1]).ToList();
                    n1 = n2;
                    sf.Add(n1.Last());
                    sb.Add(n1.First());

                } while (n2.Any(x=>x!=0));

                seqLadderForecast = sf;
                seqLadderBackcast = sb;
            }

        }
        
        static void day9()
        {
            var d = d9_data;
            var seqLines = d.Select(d => new pc9Sequence(d)).ToList();

           
            Console.WriteLine($"Answer1: {seqLines.Sum(x=>x.GetNext)}");


            Console.WriteLine($"Answer2: {seqLines.Sum(x => x.GetPrev)}");


        }

        static string[] d9_data0 =
        """
        0 3 6 9 12 15
        1 3 6 10 15 21
        10 13 16 21 30 45
        """.Split("\r\n");


    }
}
