using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {


        class c15Lens
        {
            public c15Lens(string s)
            {
                if (s.Contains('='))
                {
                    Operator = '=';
                    s = s.Replace("=",",");
                }
                else if (s.Contains('-'))
                {
                    Operator = '-';
                    s = s.Replace("-", ",");
                }
                var sl = s.Split(',');
                Code = sl[0];
                Value = sl.Length>1 && sl[1].Length>0 ? int.Parse(sl[1]) : 0;
                Index = makeHash(Code);
            }
            public string Code { get; private set; }
            public int Index { get; private set; }
            public int Value { get; set; }
            public char Operator { get; set; }  
        }

        static int makeHash(string s)
        {
            var hx = 0;
            foreach (char c in s)
            {
                hx += c;
                hx *= 17;
                hx %= 256;
            }

            return hx;
        }
        static void day15()
        {
            var d = d15_data;


            var ans1 = d[0].Split(',').Select(makeHash).Sum();
            Console.WriteLine($"Answer1: {ans1}");


            var lensMap = new Dictionary<int, List<c15Lens>>();
            for (int i = 0; i < 256; i++) lensMap.Add(i, new List<c15Lens>());
            foreach(var s in d[0].Split(','))
            {
                var lens = new c15Lens(s);
                var lensBox = lensMap[lens.Index];
                var li = lensBox.Where(x => x.Code == lens.Code).FirstOrDefault();
                if (lens.Operator == '=')
                {
                    if (li!=null)
                    {
                        li.Value = lens.Value;
                        li.Operator = lens.Operator;
                    }
                    else
                    {
                        lensBox.Add(lens);
                    }
                }
                else if (lens.Operator == '-')
                {
                    if (li != null)
                    {
                        lensBox.Remove(li);
                    }
                }

            }

            var ans2 = lensMap.Values.Select((x,i)=>x.Select((x2,i2)=> (i+1)*(i2+1)*x2.Value).Sum()).Sum();


            Console.WriteLine($"Answer2: {ans2}");


        }


        static List<string> d15_data0 =
        """
        rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
        """
        .Split("\r\n").ToList();


    }
}
