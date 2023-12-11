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
        class pc8Node
        {
            public pc8Node(string nx)
            {
                Id = nx.Substring(0, 3);
                Lnode = nx.Substring(7,3);
                Rnode = nx.Substring(12,3);
            }

            public string Id {  get; set; }
            public string Lnode { get; set; }
            public string Rnode { get; set; }

            public override string ToString() =>$"{Id} [{Lnode},{Rnode}]";
        }

        static long LCM(long[] numbers)
        {
            return numbers.Aggregate(lcm);
        }
        static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        static void day8()
        {
            var d = d8_data;

            var inst = d[0];
            var nodes = new Dictionary<string,pc8Node>();
            pc8Node sn = null;
            List<pc8Node> snl = new List<pc8Node>();
            for (int i = 2; i < d.Length; i++)
            {
                if (!nodes.ContainsKey(d[i].Substring(0, 3)))
                {
                    var n = new pc8Node(d[i]);
                    nodes.Add(n.Id,n);
                    if (n.Id == "AAA") sn = n;
                    if (n.Id[2] == 'A') snl.Add(n);
                }
            }

            
            var cn = sn;
            int ii = 0;
            int steps = 0;
            int instLoops = 0;
            int zi = 0;

            while (cn.Id != "ZZZ")
            {
                cn = inst[ii] == 'L' ? nodes[cn.Lnode] : nodes[cn.Rnode];
                if (ii < inst.Length - 1) ii++; else ii = 0;
                steps++;
            }

            Console.WriteLine($"Answer1: {steps}");


            //Get Individual Starting Node Cycles
            var snlCycles = new List<long>();
            foreach (pc8Node n in snl)
            {
                cn = n;
                ii = 0;
                steps = 0;
                instLoops = 0;
                while (cn.Id[2] != 'Z')
                {
                    cn = inst[ii] == 'L' ? nodes[cn.Lnode] : nodes[cn.Rnode];
                    steps = instLoops * inst.Length + ii;
                    if (ii < inst.Length - 1)
                    {
                        ii++;
                    }
                    else
                    {
                        ii = 0;
                        instLoops++;
                    }
                }
                snlCycles.Add(instLoops * inst.Length + ii);
                //Console.WriteLine($"---->: {n}: steps = {instLoops * inst.Length + ii} : {cn}");
            }

            //Get Least Commom Mulyiple of all cycles
            var lcm = LCM(snlCycles.ToArray()); // new long[] { 13939, 11309, 20777, 15517, 17621, 18673});

            Console.WriteLine($"Answer2: {lcm}");


            //looping took too long
            //var cnl = snl.ToList();
            //ii = 0; steps = 0;
            //instLoops = 0;
            //while (cnl.Any(n => n.Id[2] != 'Z'))
            //{
            //    for (int ni = 0; ni < cnl.Count; ni++) cnl[ni] = inst[ii] == 'L' ? nodes[cnl[ni].Lnode] : nodes[cnl[ni].Rnode];
            //    steps = instLoops * inst.Length + ii;
            //    if (ii < inst.Length - 1)
            //    {
            //        ii++;
            //    }
            //    else
            //    {
            //        ii = 0;
            //        instLoops++;
            //    }

            //    if (cnl.Where(n => n.Id[2] == 'Z').Count()>=4)
            //    {
            //        Console.WriteLine($"-----> ");
            //        foreach (var n in cnl.Where(n => n.Id[2] == 'Z'))
            //        Console.Write($",  {n.Id}: steps = {steps} [{steps - stepsLastZ}]");
            //        stepsLastZ = steps;
            //        zi++;
            //    }
            //}
        }

        static string[] d8_data0 =
        """
        LLR

        AAA = (BBB, BBB)
        BBB = (AAA, ZZZ)
        ZZZ = (ZZZ, ZZZ)
        """.Split("\r\n");

        static string[] d8_data0b =
        """
        LR

        11A = (11B, XXX)
        11B = (XXX, 11Z)
        11Z = (11B, XXX)
        22A = (22B, XXX)
        22B = (22C, 22C)
        22C = (22Z, 22Z)
        22Z = (22B, 22B)
        XXX = (XXX, XXX)
        """.Split("\r\n");


    }
}
