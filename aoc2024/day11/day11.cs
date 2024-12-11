using System;
using System.Collections.Generic;
using System.Data;
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

        static void day11()
        {           
            Console.WriteLine($"Answer1: {day11LogicPart1()}");
            Console.WriteLine($"Answer2: {day11LogicPart2()}");
        }
        static List<long> blinkList(List<long> l0)
        {
            var l = new List<long>();
            foreach (var num in l0)
            {
                if (num == 0)
                {
                    l.Add(1);
                }
                else if (num.ToString().Length % 2 == 0)
                {
                    var sn = num.ToString();
                    var pn = sn.Length / 2;
                    l.Add(long.Parse(sn.Substring(0, pn)));
                    l.Add(long.Parse(sn.Substring(pn, pn)));
                }
                else
                {
                    l.Add(num * 2024);
                }
            }
            return l;
        }

        class d11Node
        {
            public long Num;
            public long Count;
            public bool IsActive;

            public d11Node(long num, long count, bool active)
            {
                Num = num;
                Count = count;
                IsActive = active;
            }

            public d11Node Clone() => new d11Node(Num, Count, IsActive);
            public d11Node CloneAndReset() => new d11Node(Num, 0, false);

            public void AddOrIncNum(long num, long count, Dictionary<long, d11Node> l, Dictionary<long, d11Node> l0)
            {
                if (l.ContainsKey(num))
                {
                    if (l[num].IsActive)
                    {
                        l[num].Count+=count;
                    }
                    else
                    {
                        l[num].IsActive = true;
                        l[num].Count = count;
                    }
                }
                else
                {
                    l.Add(num, new(num, count, true));
                }
            }

            public override string ToString() => $"{Num}:{IsActive}:{Count}";


        }


        static Dictionary<long, d11Node> blink(Dictionary<long, d11Node> l0)
        {
            var l = l0.Values.ToDictionary(x=>x.Num,x=>x.CloneAndReset());
            foreach (var dx in l0.Values.Where(x => x.IsActive).ToList())
            {
                if (dx.Num == 0)
                {
                    l[dx.Num].AddOrIncNum(1,dx.Count, l,l0);
                }
                else if (dx.Num.ToString().Length % 2 == 0)
                {
                    var sn = dx.Num.ToString();
                    var pn = sn.Length / 2;
                    var n1 = long.Parse(sn.Substring(0, pn));
                    var n2 = long.Parse(sn.Substring(pn, pn));

                    l[dx.Num].AddOrIncNum(n1, dx.Count, l,l0);
                    l[dx.Num].AddOrIncNum(n2, dx.Count, l,l0);

                }
                else
                {
                    dx.AddOrIncNum(dx.Num * 2024, dx.Count, l, l0);
                }
            }

            return l;
        }

        static long day11LogicPart1()
        {
            var l0 = GetSpaceDelimDigitsAsListLong(d11_data[0]);
            
            for (var i = 0;i<25; i++)
            {
                l0 = blinkList(l0);
            }

            return l0.Count();
        }

        static long day11LogicPart2()
        {
            var l0 = GetSpaceDelimDigitsAsListLong(d11_data[0]).ToDictionary(k=>k,v=>new d11Node(v,1,true));

            for (var i = 0; i < 75; i++)
            {
                l0 = blink(l0);
                //Console.WriteLine($"  blink[{i}][maxActiveVal{l0.Values.Where(x => x.IsActive).Max(x=>x.Num)}][ValsAll={l0.Values.Count()}][ValsActive={l0.Values.Count(x => x.IsActive)}]: {l0.Values.Where(x=>x.IsActive).Sum(x=>x.Count)}");
            }

            return l0.Values.Where(x => x.IsActive).Sum(x => x.Count);
        }

        static string[] d11_data0 =
        """        
        125 17
        """.Split(Environment.NewLine);

    }
}

