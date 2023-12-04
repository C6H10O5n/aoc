using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2023_02
{
    internal partial class Program
    {


        static string[] d2_data0 =
            """
        Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
        Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
        Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
        Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
        Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        """.Split("\r\n");
        class gs
        {
            public gs() { }
            public gs(int igame, int iroll, string iset)
            {
                gId = igame;
                rId = iroll;
                foreach (string s in iset.Split(","))
                {
                    if (s.Contains("red")) red = int.Parse(s.Trim().Split(" ")[0]);
                    else if (s.Contains("green")) green = int.Parse(s.Trim().Split(" ")[0]);
                    else if (s.Contains("blue")) blue = int.Parse(s.Trim().Split(" ")[0]);
                }
            }
            public int gId { get; set; }
            public int rId { get; set; }
            public int red { get; set; }
            public int green { get; set; }
            public int blue { get; set; }
        }

        static void day2()
        {
            var gList = new List<gs>();
            foreach (var g in d2_data)
            {
                var g1 = g.Split(":");
                var gi = int.Parse((string)g1[0].Split(" ")[1]);
                var ri = 0;
                foreach (var s in g1[1].Split(";"))
                {
                    gList.Add(new gs(gi, ri, s));
                    ri++;
                }
            }

            var badGames = gList.Where(g => g.red > 12 || g.green > 13 || g.blue > 14).Select(g => g.gId).Distinct();
            var ans1 = gList.Where(g => !badGames.Contains(g.gId)).Select(g => g.gId).Distinct().Sum();

            Console.WriteLine($"Answer1: {ans1}");


            var ans2 = gList
                .GroupBy(g => g.gId)
                .Select(gl => new gs
                {
                    gId = gl.First().gId,
                    red = gl.Max(x => x.red),
                    green = gl.Max(x => x.green),
                    blue = gl.Max(x => x.blue),
                })
                .Sum(x => x.red * x.green * x.blue);

            Console.WriteLine($"Answer2: {ans2}");
        }

    }
}
