using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        static void day17()
        {           
            Console.WriteLine($"Answer1: {day17LogicPart1()}");
            Console.WriteLine($"Answer2: {day17LogicPart2()}");
        }        

        static string day17LogicPart1()
        {
            var id = d17_data;

            var rA = int.Parse(id[0].Split(':')[1]);
            var rB = int.Parse(id[1].Split(':')[1]);
            var rC = int.Parse(id[2].Split(':')[1]);
            var il = GetCommaDelimDigitsAsListInt(id[4].Split(':')[1]).Chunk(2).ToList();

            var result = new List<int>();

            int cop(int io)=>
               io switch
                {
                    <= 3 => io,
                    4 => rA,
                    5 => rB,
                    6 => rC
                };

            for ( var i = 0;i<il.Count; i++)
            {
                switch (il[i][0]) {
                    case 0:
                        rA /= (int)Math.Pow(2, cop(il[i][1]));
                        break;
                    case 1:
                        rB ^= il[i][1];
                        break;
                    case 2:
                        rB = cop(il[i][1]) % 8;
                        break;
                    case 3:
                        i = rA > 0 ? il[i][1]-1 : i;
                        break;
                    case 4:
                        rB ^= rC;
                        break;
                    case 5:
                        result.Add(cop(il[i][1])%8);
                        break;
                    case 6:
                        rB = rA / (int)Math.Pow(2, cop(il[i][1]));
                        break;
                    case 7:
                        rC = rA / (int)Math.Pow(2, cop(il[i][1]));
                        break;
                }


            }

            return string.Join(',',result);
        }
        static string day17LogicPart2()
        {

            var id = d17_data;

            var rA = long.Parse(id[0].Split(':')[1]);
            var rB = long.Parse(id[1].Split(':')[1]);
            var rC = long.Parse(id[2].Split(':')[1]);
            var il = GetCommaDelimDigitsAsListLong(id[4].Split(':')[1]).Chunk(2).ToList();

            var result = new List<long>();

            //rA = 117440;


            long cop(long io) =>
               io switch
               {
                   <= 3 => io,
                   4 => rA,
                   5 => rB,
                   6 => rC
               };

            //2,4,1,1,7,5,1,5,4,3,5,5,0,3,3,0

            bool IsCorrect (long rA, List<long> res)
            {
                int i = 0;
                do
                {


                    var rB = rA & 0b111;    //  1111 & 0111 = 111
                    rB ^= 1;            //
                    var rC = rA >> (int)rB; //
                    rB ^= 5;            //
                    rB ^= rC;           //
                    var ans = rB % 8;   //10
                    if (ans != res[i])
                    {
                        return false;
                    }
                    rA >>= 3;           // makes ra smaller
                    i++;
                } while (rA > 0);
                return true;
            }

            var xr = il.SelectMany(x => x).ToList();

            long findIns(int depth, long ss)
            {
                if (depth == xr.Count) return ss;
                ss <<= 3;
                for (int i=0; i<8; i++)
                {
                    List<long> arr = xr.TakeLast(depth + 1).ToList();
                    if (IsCorrect(ss | i , arr))
                    {
                        var s = findIns(depth + 1, ss | i);
                        if (s != -1)
                        {
                            return s; 
                        }
                    }
    
                }
                return -1;
            }

            Console.WriteLine(findIns(0, 0)); 



            //for (int i = 0; i < il.Count; i++)
            //{
            //    switch (il[i][0])
            //    {
            //        case 0:
            //            rA /= (long)Math.Pow(2, cop(il[i][1]));
            //            break;
            //        case 1:
            //            rB ^= il[i][1];
            //            break;
            //        case 2:
            //            rB = cop(il[i][1]) % 8;
            //            break;
            //        case 3:
            //            i = rA > 0 ? (int)il[i][1] - 1 : i;
            //            break;
            //        case 4:
            //            rB ^= rC;
            //            break;
            //        case 5:
            //            result.Add(cop(il[i][1]) % 8);
            //            break;
            //        case 6:
            //            rB = rA / (long)Math.Pow(2, cop(il[i][1]));
            //            break;
            //        case 7:
            //            rC = rA / (long)Math.Pow(2, cop(il[i][1]));
            //            break;
            //    }


            //}

            return string.Join(',', result);
        }

        static string[] d17_data0 =
        """        
        Register A: 729
        Register B: 0
        Register C: 0

        Program: 0,1,5,4,3,0
        """.Split(Environment.NewLine);


        static string[] d17_data02 =
        """        
        Register A: 2024
        Register B: 0
        Register C: 0

        Program: 0,3,5,4,3,0
        """.Split(Environment.NewLine);


    }
}
