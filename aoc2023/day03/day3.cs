using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoc2023_02
{
    internal partial class Program
    {
        static bool chkAround(int y, int x, string[] d)
        {
            for (int i = y - (y > 0 ? 1 : 0); i <= y + (y == d.Length - 1 ? 0 : 1); i++)
            {
                for (int j = x - (x > 0 ? 1 : 0); j <= x + (x == d[y].Length -1  ? 0 : 1); j++)
                {
                    if (!Char.IsAsciiDigit(d[i][j]) && d[i][j]!='.') return true;
                }
            }
            return false;
        }

        static (int y,int x) chkAroundForStars(int y, int x, string[] d)
        {
            for (int i = y - (y > 0 ? 1 : 0); i <= y + (y == d.Length - 1 ? 0 : 1); i++)
            {
                for (int j = x - (x > 0 ? 1 : 0); j <= x + (x == d[y].Length - 1 ? 0 : 1); j++)
                {
                    if (d[i][j] == '*') return (i,j);
                }
            }
            return (-1,-1);
        }

        static void day3()
        {
            var result = new List<(int num,string starid)>();
            var d = d3_data;
            for (int i = 0; i< d.Length; i++)
            {
                var lx = d[i];
                var nx = "";
                var ns = false;
                var starId = (-1, -1);
                for (int j = 0; j < lx.Length+1; j++)
                {
                    if (j < lx.Length && Char.IsDigit(lx[j]))
                    {
                        nx += lx[j];
                        if(!ns) ns = chkAround(i, j, d);
                        if(starId==(-1,-1)) starId = chkAroundForStars(i, j, d);
                    }
                    else if (nx != "") //checked number
                    {   //
                        if (ns) result.Add((int.Parse(nx), $"{starId.Item1}|{starId.Item2}"));
                        nx = ""; 
                        ns = false;
                        starId = (-1, -1);
                    }
                }
            }

            Console.WriteLine($"Answer1: {result.Sum(x=>x.num)}");

            var gear = result
                .Where(x => x.starid != ("-1|-1"))
                .GroupBy(x => x.starid)
                .Select(z => new
                {
                    starid = z.First().starid,
                    count = z.Count(),
                    numMin = z.Min(x => x.num),
                    numMax = z.Max(x => x.num),
                })
                .Where(x => x.count == 2);

            Console.WriteLine($"Answer2: {gear.Sum(x => x.numMin*x.numMax)}");
        }

        static string[] d3_data0 =
        """
        467..114..
        ...*......
        ..35..633.
        ......#...
        617*......
        .....+.58.
        ..592.....
        ......755.
        ...$.*....
        .664.598..
        """.Split("\r\n");


    }
}
