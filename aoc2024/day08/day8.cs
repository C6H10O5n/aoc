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

        class c8Coord
        {
            public c8Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }


            public string AntiNodeList = "";
            public bool IsAntinode => AntiNodeList != "";

            public c8Map ParentMap { get; set; }

            public bool IsAntenna => s != '.';

            public List<c8Coord> FindAntinodes(c8Coord ic, bool inResonance = false)
            {
                //must be two valid antenna towers
                if (!IsAntenna || !ic.IsAntenna) return null;

                List<c8Coord> ac1,ac1x;
                int cnt = 0;
                if (inResonance)
                {
                    ac1 = new List<c8Coord>();
                    do
                    {
                        cnt += 1;

                        ac1x = ParentMap.Cells.Where(x =>
                           (x.r == r + (r - ic.r)*cnt && x.c == c + (c - ic.c)*cnt)
                        || (x.r == ic.r + (ic.r - r)*cnt && x.c == ic.c + (ic.c - c)*cnt)
                        ).ToList();

                        if (ac1x.Count > 0)
                            ac1.AddRange(ac1x);

                    } while (ac1x.Count>0);
                }
                else
                {
                    ac1 = ParentMap.Cells.Where(x =>
                       (x.r == r + (r - ic.r) && x.c == c + (c - ic.c))
                    || (x.r == ic.r + (ic.r - r) && x.c == ic.c + (ic.c - c))
                    ).ToList();
                }

                ac1.ForEach(x => x.AntiNodeList+=s);

                return ac1;
            }

            public double CalcDistToCoordMahatten(c8Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c8Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }


            c8Coord CellNorth => NeighborCellsBase[0];
            c8Coord CellSouth => NeighborCellsBase[1];
            c8Coord CellEast => NeighborCellsBase[2];
            c8Coord CellWest => NeighborCellsBase[3];
            c8Coord CellNE => NeighborCellsBase[4];
            c8Coord CellNW => NeighborCellsBase[5];
            c8Coord CellSE => NeighborCellsBase[6];
            c8Coord CellSW => NeighborCellsBase[7];
            public List<c8Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c8Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c8Coord>();
                        var rc = ParentMap.rowCnt - 1;
                        var cc = ParentMap.colCnt - 1;
                        if (r > 0) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//N
                        if (r < rc) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//S
                        if (c < cc) nc.Add(ParentMap[r + 0][c + 1]); else nc.Add(null);//E
                        if (c > 0) nc.Add(ParentMap[r + 0][c - 1]); else nc.Add(null);//W


                        if (r > 0 && c < cc) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//NE
                        if (r > 0 && c > 0) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//NW
                        if (r < rc && c < cc) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//SE
                        if (r < rc && c > 0) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//SW

                        return nc;
                    }
                }
            }

            public bool Equals(c8Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s && AntiNodeList == ic.AntiNodeList) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c8Map : List<List<c8Coord>>
        {
            public c8Map(string[] ls)
            {
                for (int i = 0; i < ls.Count(); i++)
                {
                    var ca = ls[i].ToCharArray();
                    var xcl = new List<c8Coord>();
                    for (int j = 0; j < ca.Length; j++)
                    {
                        xcl.Add(new c8Coord(ca[j], i, j) { ParentMap = this });
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;


            public int FindAntiNodes()
            {
                PrintMap();

                foreach(var ax in  Cells.Where(x => x.IsAntenna).Select(x => x.s).Distinct())
                {
                    foreach(var ap in Cells.Where(x=>x.s==ax).DifferentCombinations(2).Select(x => x.ToArray()).ToArray())
                    {
                        ap.First().FindAntinodes(ap.Last());
                    }
                }

                PrintMap();

                return Cells.Count(x=>x.IsAntinode);
            }

            public int FindResonanceAntiNodes()
            {
                PrintMap();

                foreach (var ax in Cells.Where(x => x.IsAntenna).Select(x => x.s).Distinct())
                {
                    foreach (var ap in Cells.Where(x => x.s == ax).DifferentCombinations(2).Select(x => x.ToArray()).ToArray())
                    {
                        ap.First().FindAntinodes(ap.Last(), true);
                    }
                }

                PrintMap();

                return Cells.Count(x => x.IsAntinode || x.IsAntenna);
            }

            public IEnumerable<c8Coord> Cells => this.SelectMany(x => x);

            public void PrintMap()
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsAntinode && c.IsAntenna ? '@'
                        : c.IsAntinode ? '#'
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }

        static void day8()
        {           
            Console.WriteLine($"Answer1: {day8LogicPart1()}");
            Console.WriteLine($"Answer2: {day8LogicPart2()}");
        }
                

        static long day8LogicPart1() 
        {
            var map = new c8Map(d8_data);

            return map.FindAntiNodes();
        }

        static long day8LogicPart2()
        {

            var map = new c8Map(d8_data);

            return map.FindResonanceAntiNodes();
        }

        static string[] d8_data0 =
        """        
        ............
        ........0...
        .....0......
        .......0....
        ....0.......
        ......A.....
        ............
        ............
        ........A...
        .........A..
        ............
        ............
        """.Split(Environment.NewLine);

        static string[] d8_data01 =
        """        
        ..........
        ..........
        ..........
        ....a.....
        ..........
        .....a....
        ..........
        ..........
        ..........
        ..........
        """.Split(Environment.NewLine);

        static string[] d8_data02 =
        """        
        T.........
        ...T......
        .T........
        ..........
        ..........
        ..........
        ..........
        ..........
        ..........
        ..........
        """.Split(Environment.NewLine);

    }
}
