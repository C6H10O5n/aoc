﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace aoc2024
{
    internal partial class Program
    {

        static void day13()
        {           
            Console.WriteLine($"Answer1: {day13LogicPart1()}");
            Console.WriteLine($"Answer2: {day13LogicPart2()}");
        }

        class d13Button
        {
            public long Y { get; set; }
            public long X { get; set; }
            public override string ToString() => $"[{X},{Y}]";
        }

        class d13Machine
        {
            public d13Machine(string input, long prizeMultiplier = 1) 
            {
                foreach(string s in input.Split(Environment.NewLine))
                {
                    var sd = s.Split(":")[1].Trim()
                        .Replace("X","").Replace("Y", "")
                        .Replace("+", "").Replace("=", "")
                        .Replace(" ", "").Split(",")
                        .Select(x=>int.Parse(x)).ToList();
                    
                    if(s.StartsWith("Button A"))
                    {
                        A = new d13Button() { X = sd[0], Y = sd[1] };
                    }
                    else if (s.StartsWith("Button B"))
                    {
                        B = new d13Button() { X = sd[0], Y = sd[1] };
                    }
                    else if (s.StartsWith("Prize"))
                    {
                        PrizeLocation = new d13Button() { X = sd[0] * prizeMultiplier, Y = sd[1] * prizeMultiplier };
                    }
                }
            }

            public d13Button A { get; set; }
            public d13Button B { get; set; }
            public d13Button PrizeLocation { get; set; }

            public List<(long x1, long x2)> GetWinPaths(bool isX)
            {
                var pv = isX ? PrizeLocation.X : PrizeLocation.Y;
                var av = isX ? A.X : A.Y;
                var bv = isX ? B.X : B.Y;

                var xx1 = CreateRange(0, pv / av).ToList()
                    .Where(an => (pv - an * av) % bv == 0)
                    .Select(an => (an, (pv - an * av) / bv) )
                    .Where(x => x.Item1 * av + x.Item2 * bv == pv)
                    .ToList();

                var xx2 = CreateRange(0, pv / bv).ToList()
                    .Where(bn => (pv - bn * bv) % av == 0)
                    .Select(bn => ((pv - bn * bv) / av, bn))
                    .Where(x => x.Item1 * av + x.Item2 * bv == pv)
                    .ToList();

                return xx1.Union(xx2).Distinct().ToList();
            }


            public override string ToString() => $"A:{A} B:{B} Prize{PrizeLocation}";

        }

        static IEnumerable<long> CreateRange(long start, long count)
        {
            var limit = start + count;

            while (start < limit)
            {
                yield return start;
                start++;
            }
        }

        static long day13LogicPart1()
        {
            var mrd = d13_data.Select(x => new d13Machine(x)).ToList();


            var sol = new List<(long x1, long x2)>();

            foreach (var mch in mrd)
            { 

                var xx = mch.GetWinPaths(true);
                var yy = mch.GetWinPaths(false);

                var ss = xx.Intersect(yy).ToList();

                if(ss != null && ss.Count > 1)
                {
                    var dd = 1;
                }

                if(ss!=null) sol.AddRange(ss);
            }

            return sol.Sum(x=>x.x1*3+x.x2*1);
        }

        static long day13LogicPart2()
        {

            //26735 too low
            var mrd = d13_data0.Select(x => new d13Machine(x, 10000000000000)).ToList();


            var sol = new List<(long x1, long x2)>();

            foreach (var mch in mrd)
            {

                var xx = mch.GetWinPaths(true);
                var yy = mch.GetWinPaths(false);

                var ss = xx.Intersect(yy).ToList();

                if (ss != null && ss.Count > 1)
                {
                    var dd = 1;
                }

                if (ss != null) sol.AddRange(ss);
            }

            return sol.Sum(x => x.x1 * 3 + x.x2 * 1);
        }

        static string[] d13_data0 =
        """        
        Button A: X+94, Y+34
        Button B: X+22, Y+67
        Prize: X=8400, Y=5400

        Button A: X+26, Y+66
        Button B: X+67, Y+21
        Prize: X=12748, Y=12176

        Button A: X+17, Y+86
        Button B: X+84, Y+37
        Prize: X=7870, Y=6450

        Button A: X+69, Y+23
        Button B: X+27, Y+71
        Prize: X=18641, Y=10279
        """.Split(Environment.NewLine+ Environment.NewLine);

    }
}
