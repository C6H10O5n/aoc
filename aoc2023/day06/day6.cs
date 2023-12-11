using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {
        class cRace
        {
            public long Time { get; set; }
            public long Distance { get; set; }

            public long Rate { get; set; } = 1;

            public List<long> PossibleWinningOptions { get; private set; } = new List<long>();

            public int CountWinningOptions => PossibleWinningOptions.Count;

            public void CalcWinningOptions()
            {
                PossibleWinningOptions.Clear();

                long rd = 0;
                for (long i = 1; i <= Time; i++)
                {
                    rd = (Time - i) * i * Rate;
                    if (rd > Distance) PossibleWinningOptions.Add(i);
                }

            }

        }

        static void day6()
        {
            var d = d6_data;

            var xt = GetSpaceDelimDigitsAsListInt(d[0].Split(":")[1]);
            var xd = GetSpaceDelimDigitsAsListInt(d[1].Split(":")[1]);

            var races = new List<cRace>();
            for (int i = 0; i < xt.Count; i++) races.Add(new cRace() { Time = xt[i], Distance = xd[i] });
            foreach (var xr in races) xr.CalcWinningOptions();

            Console.WriteLine($"Answer1: {races.Select(x => x.CountWinningOptions).Aggregate((x, y) => x * y)}");


            var race = new cRace
            {
                Time = long.Parse(d[0].Split(":")[1].Replace(" ", "")),
                Distance = long.Parse(d[1].Split(":")[1].Replace(" ", ""))
            };
            race.CalcWinningOptions();

            Console.WriteLine($"Answer2: {race.CountWinningOptions}");
        }

        static string[] d6_data0 =
        """
        Time:      7  15   30
        Distance:  9  40  200
        """.Split("\r\n");

        static string[] d6_data =
        """
        Time:        41     96     88     94
        Distance:   214   1789   1127   1055
        """.Split("\r\n");


    }
}
