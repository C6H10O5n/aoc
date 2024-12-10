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

        static void day9()
        {           
            Console.WriteLine($"Answer1: {day9LogicPart1()}");
            Console.WriteLine($"Answer2: {day9LogicPart2()}");
        }
                

        static long day9LogicPart1()
        {
            //var fx = d9_data0[0].ToCharArray().Select(c => c - '0').ToList();
            var fxd = d9_data[0].ToCharArray().Where((c, i) => i % 2 == 0).Select(c => c - '0').ToList();
            var fxs = d9_data[0].ToCharArray().Where((c, i) => i % 2 != 0).Select(c => c - '0').ToList();

            var fxdSum = fxd.Sum();
            var fxdCnt = fxd.Count;
            var rx = new List<int>();
            for(int i=0; i< fxdCnt; i++)
            {
                for(int j=0; j < fxd[i]; j++)
                {
                    rx.Add(i);
                    if (rx.Count == fxdSum)
                        break;
                }
                if (rx.Count == fxdSum)
                    break;
                for (int j = 0; j < fxs[i]; j++)
                {
                    rx.Add(fxd.Count-1);
                    if (rx.Count == fxdSum)
                        break;
                    fxd[fxd.Count - 1] -= 1;
                    if (fxd[fxd.Count - 1] == 0)
                        fxd.RemoveAt(fxd.Count - 1);
                }
                if (rx.Count == fxdSum)
                    break;
            }

            return rx.Select((r,idx)=>(long)(r*idx)).Sum();
        }

        static long day9LogicPart2()
        {
            //6368144863485 to high -- need to stop searching foward: added [ && j<i] in line 82 if statement
            //6362722604045

            //var fx = d9_data0[0].ToCharArray().Select(c => c - '0').ToList();
            var fxd = d9_data[0].ToCharArray().Where((c, i) => i % 2 == 0).Select(c => c - '0').ToList();
            var fxs = d9_data[0].ToCharArray().Where((c, i) => i % 2 != 0).Select(c => c - '0').ToList();
            fxs.Add(0);


            var rx = fxd.Select((x, i) => (x, i))
                    .Zip(fxs.Select((y) => y), (a, b) => Enumerable.Range(0, a.x).Select(_ => a.i).Concat(Enumerable.Range(0, b).Select(_ => -1))
                    .ToList()).ToList();

            //Console.WriteLine("Input String: "+d9_data0[0]);
            for (int i = fxd.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine((new string(i.ToString()[0],fxd[i])+"     ").Substring(0,5)+string.Join("",rx.SelectMany(x=>x).Select(x=>x==-1?".":x==-2?"-":x.ToString())));

                for (int j = 0; j < fxs.Count; j++)
                {

                    if (fxd[i] <= fxs[j] && j<i)
                    {
                        var sp = rx[j].Count() - fxs[j];
                        for (int k = 0; k < fxd[i]; k++)
                        {
                            rx[j][k + sp] = i;
                            rx[i][k] = -2;
                        }
                        fxs[j] -= fxd[i];
                        break;
                    }
                }
            }

            return rx.SelectMany(x => x).Select((xd, xi) => (xd, xi)).Where(x=>x.xd>0).Sum(x => (long)(x.xd*x.xi));
        }

        static string[] d9_data0 =
        """        
        2333133121414131402
        """.Split(Environment.NewLine);

    }
}
