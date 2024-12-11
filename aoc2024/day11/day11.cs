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
            //Console.WriteLine($"Answer1: {day11LogicPart1()}");
            Console.WriteLine($"Answer2: {day11LogicPart2()}");
        }
        static (long, long) split(long num)
        {
            var sn = num.ToString();
            var pn = sn.Length / 2;
            var r1 = (long.Parse(sn.Substring(0, pn)));
            var r2 = (long.Parse(sn.Substring(pn, pn)));
            return (r1, r2);
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

            //var l0 = GetSpaceDelimDigitsAsListLong(d11_data[0]);
            //var l0 = new List<List<long>>() { new List<long>() { 2024 } };
            var l0 = new List<List<long>>() { //8435 234 928434 14 0 7 92446 8992692
                 new List<long>() { 8435 }
                ,new List<long>() { 234 }
                ,new List<long>() { 928434 }
                ,new List<long>() { 14 }
                ,new List<long>() { 0 }
                ,new List<long>() { 7 }
                ,new List<long>() { 92446 }
                ,new List<long>() { 8992692 }
            };

            var lstSz = 100000;

            for (var i = 0; i < 75; i++)
            {
                //for(var j=0; j<l0.Count; j++)
                //{
                //    l0[j] = blinkList(l0[j]);
                //}
                Parallel.For(0, l0.Count, j =>
                {
                    l0[j] = blinkList(l0[j]);
                });
                for (var j = 0; j < l0.Count; j++)
                {
                    if (l0[j].Count > lstSz)
                    {
                        l0.Add(l0[j].Take(lstSz/2).ToList());
                        l0[j] = l0[j].Skip(lstSz/2).ToList();
                    }
                }
                Console.WriteLine($"  blink[{i}][lst#={l0.Count}][maxVal{l0.Max(x=>(long)x.Max())}][distinctVals={l0.SelectMany(x=>x.Distinct()).Distinct().Count()}]: {l0.Sum(x=>(long)x.Count)}");
            }

            return l0.Count();
        }

        static string[] d11_data0 =
        """        
        125 17
        """.Split(Environment.NewLine);

    }
}

/*
 * 
 

Day 11 Problem:
  blink[0][lst#=1][maxVal24]: 2
  blink[1][lst#=1][maxVal4]: 4
  blink[2][lst#=1][maxVal8096]: 4
  blink[3][lst#=1][maxVal2024]: 7
  blink[4][lst#=1][maxVal24]: 14
  blink[5][lst#=1][maxVal18216]: 16
  blink[6][lst#=1][maxVal36869184]: 20
  blink[7][lst#=1][maxVal9456]: 39
  blink[8][lst#=1][maxVal18216]: 62
  blink[9][lst#=1][maxVal36869184]: 81
  blink[10][lst#=1][maxVal36869184]: 110
  blink[11][lst#=1][maxVal36869184]: 200
  blink[12][lst#=1][maxVal18216]: 328
  blink[13][lst#=1][maxVal36869184]: 418
  blink[14][lst#=1][maxVal36869184]: 667
  blink[15][lst#=1][maxVal36869184]: 1059
  blink[16][lst#=1][maxVal36869184]: 1546
  blink[17][lst#=1][maxVal36869184]: 2377
  blink[18][lst#=1][maxVal36869184]: 3572
  blink[19][lst#=1][maxVal36869184]: 5602
  blink[20][lst#=1][maxVal36869184]: 8268
  blink[21][lst#=1][maxVal36869184]: 12343
  blink[22][lst#=1][maxVal36869184]: 19778
  blink[23][lst#=1][maxVal36869184]: 29165



Day 11 Problem:
  blink[0][lst#=8]: 11
  blink[1][lst#=8]: 14
  blink[2][lst#=8]: 21
  blink[3][lst#=8]: 34
  blink[4][lst#=8]: 47
  blink[5][lst#=8]: 64
  blink[6][lst#=8]: 103
  blink[7][lst#=8]: 155
  blink[8][lst#=8]: 225
  blink[9][lst#=8]: 359
  blink[10][lst#=8]: 537
  blink[11][lst#=8]: 801
  blink[12][lst#=8]: 1174
  blink[13][lst#=8]: 1810
  blink[14][lst#=8]: 2901
  blink[15][lst#=8]: 4189
  blink[16][lst#=8]: 6279
  blink[17][lst#=8]: 9890
  blink[18][lst#=8]: 14730
  blink[19][lst#=8]: 22477
  blink[20][lst#=8]: 34025
  blink[21][lst#=8]: 51693
  blink[22][lst#=8]: 79208
  blink[23][lst#=8]: 118031
  blink[24][lst#=8]: 182081
  blink[25][lst#=8]: 277154
  blink[26][lst#=9]: 414824
  blink[27][lst#=10]: 637128
  blink[28][lst#=16]: 964027
  blink[29][lst#=23]: 1466235
  blink[30][lst#=33]: 2229727
  blink[31][lst#=52]: 3366152
  blink[32][lst#=76]: 5154934
  blink[33][lst#=113]: 7793740
  blink[34][lst#=174]: 11819352
  blink[35][lst#=255]: 18052606
  blink[36][lst#=396]: 27278105
  blink[37][lst#=600]: 41535881
  blink[38][lst#=902]: 63083615
  blink[39][lst#=1387]: 95665503
  blink[40][lst#=2096]: 145765473
  blink[41][lst#=3160]: 220651024
  blink[42][lst#=4833]: 335672938
  blink[43][lst#=7238]: 510419017
  blink[44][lst#=11082]: 773324028
  blink[45][lst#=16694]: 1177345751
  blink[46][lst#=25312]: 1786196766
  blink[47][lst#=38664]: 2712820361
  blink[48][lst#=58115]: 4125149793
  blink[49][lst#=88517]: 6255737488



2024:
  blink[0][lst#=1]: 2
  blink[1][lst#=1]: 4
  blink[2][lst#=1]: 4
  blink[3][lst#=1]: 7
  blink[4][lst#=1]: 14
  blink[5][lst#=1]: 16
  blink[6][lst#=1]: 20
  blink[7][lst#=1]: 39
  blink[8][lst#=1]: 62
  blink[9][lst#=1]: 81
  blink[10][lst#=1]: 110
  blink[11][lst#=1]: 200
  blink[12][lst#=1]: 328
  blink[13][lst#=1]: 418
  blink[14][lst#=1]: 667
  blink[15][lst#=1]: 1059
  blink[16][lst#=1]: 1546
  blink[17][lst#=1]: 2377
  blink[18][lst#=1]: 3572
  blink[19][lst#=1]: 5602
  blink[20][lst#=1]: 8268
  blink[21][lst#=1]: 12343
  blink[22][lst#=1]: 19778
  blink[23][lst#=1]: 29165
  blink[24][lst#=1]: 43726
  blink[25][lst#=1]: 67724
  blink[26][lst#=2]: 102131
  blink[27][lst#=2]: 156451
  blink[28][lst#=4]: 234511
  blink[29][lst#=5]: 357632
  blink[30][lst#=8]: 549949
  blink[31][lst#=12]: 819967
  blink[32][lst#=18]: 1258125
  blink[33][lst#=27]: 1916299
  blink[34][lst#=43]: 2886408
  blink[35][lst#=63]: 4414216
  blink[36][lst#=96]: 6669768
  blink[37][lst#=147]: 10174278
  blink[38][lst#=219]: 15458147
  blink[39][lst#=332]: 23333796
  blink[40][lst#=509]: 35712308
  blink[41][lst#=763]: 54046805
  blink[42][lst#=1176]: 81997335
  blink[43][lst#=1745]: 125001266
  blink[44][lst#=2704]: 189148778
  blink[45][lst#=4076]: 288114305
  blink[46][lst#=6147]: 437102505
  blink[47][lst#=9387]: 663251546
  blink[48][lst#=14161]: 1010392024
  blink[49][lst#=21567]: 1529921658
  blink[50][lst#=32668]: 2327142660
  blink[51][lst#=49205]: 3537156082
  blink[52][lst#=75319]: 5362947711
  blink[53][lst#=113293]: 8161193535



 */