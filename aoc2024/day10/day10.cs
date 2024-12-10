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

        class c10Coord
        {
            public c10Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;

                if (IsTrailHead)
                    trailHeadScoreList = new List<c10Coord>();
            }


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c10Map ParentMap { get; set; }

            private List<c10Coord> trailHeadScoreList = null;
            public int TrailHeadScore => trailHeadScoreList == null? -1: trailHeadScoreList.Count;

            public bool IsTrailHead => s == '0';
            public bool IsTrailEnd => s == '9';
            public bool IsImpassable => s == '.';

            public int FindTrails(bool paths)
            {
                //must be two valid antenna towers
                if (!IsTrailHead) return 0;

                FindTrail(this,paths);                

                return TrailHeadScore;
            }

            public void FindTrail(c10Coord thn, bool paths)
            {
                foreach (var cell in NeighborCells.Where(x => x.isOneStepUp(this)))
                {
                    if (cell.IsTrailEnd)
                    {
                        if (paths)
                        {
                            thn.trailHeadScoreList.Add(thn);
                        }
                        else
                        {
                            if (!thn.trailHeadScoreList.Contains(cell))
                                thn.trailHeadScoreList.Add(cell);
                        }
                    }
                    else
                    {
                        cell.FindTrail(thn, paths);
                    }
                }
            }

            bool isOneStepUp(c10Coord ic)
            {
                return !ic.IsImpassable && s-ic.s==1;
            }

            public double CalcDistToCoordMahatten(c10Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c10Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }


            c10Coord CellNorth => NeighborCellsBase[0];
            c10Coord CellSouth => NeighborCellsBase[1];
            c10Coord CellEast => NeighborCellsBase[2];
            c10Coord CellWest => NeighborCellsBase[3];
            c10Coord CellNE => NeighborCellsBase[4];
            c10Coord CellNW => NeighborCellsBase[5];
            c10Coord CellSE => NeighborCellsBase[6];
            c10Coord CellSW => NeighborCellsBase[7];
            public List<c10Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c10Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c10Coord>();
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

            public bool Equals(c10Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c10Map : List<List<c10Coord>>
        {
            public c10Map(string[] ls)
            {
                for (int i = 0; i < ls.Count(); i++)
                {
                    var ca = ls[i].ToCharArray();
                    var xcl = new List<c10Coord>();
                    for (int j = 0; j < ca.Length; j++)
                    {
                        xcl.Add(new c10Coord(ca[j], i, j) { ParentMap = this });
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;


            public int MaxRoutes => Cells.Count(x=>x.IsTrailEnd);


            public int FindTrails(bool path = false)
            {
                //PrintMap();

                Cells.Where(x=>x.IsTrailHead).ToList().ForEach(x => x.FindTrails(path));
                
                //PrintMap();

                return Cells.Where(x => x.IsTrailHead).Sum(x => x.TrailHeadScore);
            }

            public IEnumerable<c10Coord> Cells => this.SelectMany(x => x);

            public void PrintMap()
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write( c.s
                        //  c.IsAntinode && c.IsTrailHead ? '@'
                        //: c.IsAntinode ? '#'
                        //: c.s
                        );
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }


        static void day10()
        {           
            Console.WriteLine($"Answer1: {day10LogicPart1()}");
            Console.WriteLine($"Answer2: {day10LogicPart2()}");
        }
                

        static long day10LogicPart1()
        {
            var map = new c10Map(d10_data);

            return map.FindTrails();
        }

        static long day10LogicPart2()
        {
            var map = new c10Map(d10_data);

            return map.FindTrails(true);
        }

        static string[] d10_data0 =
        """  
        89010123
        78121874
        87430965
        96549874
        45678903
        32019012
        01329801
        10456732
        """.Split(Environment.NewLine);

    }
}
