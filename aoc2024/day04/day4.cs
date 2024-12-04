using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc2024
{
    internal partial class Program
    {
        static void day4()
        {           
            //Console.WriteLine($"Answer1: {day4LogicPart1()}");
            Console.WriteLine($"Answer2: {day4LogicPart2()}");
        }


        static int day4LogicPart1() 
        {

            var rx_p1 = new Regex("XMAS");
            var rx_p2 = new Regex("SAMX");

            var mx = d4_data.ToList();

            var w1 = "XMAS";
            var cnt = 0;

            //Cols
            for (int j = 0; j < mx[0].Length; j++)
            {
                var rs1 = string.Join("", mx.Select(x => x[j]));
                cnt += rx_p1.Count(rs1);
                cnt += rx_p2.Count(rs1);
            }

            //rows
            foreach (var r in mx)
            {
                cnt += rx_p1.Count(r);
                cnt += rx_p2.Count(r);
            }

            //diagionals
            for (int i=0; i<mx.Count-(w1.Length-1); i++)
            {
                for (int j = 0; j < mx[i].Length - (w1.Length-1); j++)
                {
                    var rs1 = new string(new char[] { mx[i][j], mx[i + 1][j + 1], mx[i + 2][j + 2], mx[i + 3][j + 3] });
                    cnt += rx_p1.Count(rs1);
                    cnt += rx_p2.Count(rs1);
                    var rs2 = new string(new char[] { mx[i][j+3], mx[i + 1][j + 2], mx[i + 2][j + 1], mx[i + 3][j] });
                    cnt += rx_p1.Count(rs2);
                    cnt += rx_p2.Count(rs2);
                }
            }

            return cnt;
        }

        static int day4LogicPart2()
        {


            var rx_p1 = new Regex("MAS");
            var rx_p2 = new Regex("SAM");

            var mx = d4_data.ToList();

            var w1 = "MAS";
            var cnt = 0;

            //diagionals
            for (int i = 0; i < mx.Count - (w1.Length - 1); i++)
            {
                for (int j = 0; j < mx[i].Length - (w1.Length - 1); j++)
                {
                    var rs1 = new string(new char[] { mx[i][j], mx[i + 1][j + 1], mx[i + 2][j + 2] });
                    var rs2 = new string(new char[] { mx[i][j + 2], mx[i + 1][j + 1], mx[i + 2][j] });
                    if((rx_p1.IsMatch(rs1) || rx_p2.IsMatch(rs1)) && (rx_p1.IsMatch(rs2) || rx_p2.IsMatch(rs2)))
                        cnt += 1;
                }
            }

            return cnt;
        }

        static string[] d4_data0 =
        """        
        MMMSXXMASM
        MSAMXMSMSA
        AMXSXMAAMM
        MSAMASMSMX
        XMASAMXAMM
        XXAMMXXAMA
        SMSMSASXSS
        SAXAMASAAA
        MAMMMXMMMM
        MXMXAXMASX
        """.Split(Environment.NewLine);

        static string[] d4_data01 =
        """        
        ....XXMAS.
        .SAMXMS...
        ...S..A...
        ..A.A.MS.X
        XMASAMX.MM
        X.....XA.A
        S.S.S.S.SS
        .A.A.A.A.A
        ..M.M.M.MM
        .X.X.XMASX
        """.Split(Environment.NewLine);

        static string[] d4_data02 =
        """        
        .M.S......
        ..A..MSMS.
        .M.S.MAA..
        ..A.ASMSM.
        .M.S.M....
        ..........
        S.S.S.S.S.
        .A.A.A.A..
        M.M.M.M.M.
        ..........
        """.Split(Environment.NewLine);

    }
}
