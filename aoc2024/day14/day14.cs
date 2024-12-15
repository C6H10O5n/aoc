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
        class c14Guard
        {
            public c14Guard(string iData, c14Map iMap)
            {

                Id = iMap.GuardCount;
                Map = iMap;

                var ic = int.Parse(iData.Split(" v=")[0].Split(",")[0][2..]);
                var ir = int.Parse(iData.Split(" v=")[0].Split(",")[1]);

                ParentCell = iMap.Cells.Where(c => c.c == ic && c.r == ir).FirstOrDefault();
                ParentCell.Guards.Add(this);


                cStep = int.Parse(iData.Split(" v=")[1].Split(",")[0]);
                rStep = int.Parse(iData.Split(" v=")[1].Split(",")[1]);

                iMap.Guards.Add(this);


            }


            public int Id { get; set; }
            public int cStep { get; set; }
            public int rStep { get; set; }
            public c14Coord ParentCell { get; set; }
            public c14Map Map { get; set; }



            public c14Coord Move()
            {
                var nr = ParentCell.r + rStep;
                var nc = ParentCell.c + cStep;

                if (nr >= Map.rowCnt) nr = (nr - Map.rowCnt);
                if (nc >= Map.colCnt) nc = (nc - Map.colCnt);


                if (nr < 0) nr = (nr + Map.rowCnt);
                if (nc < 0) nc = (nc + Map.colCnt);

                ParentCell.Guards.Remove(this);
                ParentCell = ParentCell.ParentMap.Cells.Where(c => c.c == nc && c.r == nr).FirstOrDefault();
                ParentCell.Guards.Add(this);

                return ParentCell;
            }
        }    


        class c14Coord
        {
            public c14Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }


            public c14Map ParentMap { get; set; }

            public List<c14Guard> Guards { get; set; } = new List<c14Guard>();
            public bool HasGuard => Guards.Count>0;


            public int Quadrant =>
                  (r < ParentMap.rowCnt / 2 && c < ParentMap.colCnt / 2) ? 1
                : (r > ParentMap.rowCnt / 2 && c < ParentMap.colCnt / 2) ? 2
                : (r < ParentMap.rowCnt / 2 && c > ParentMap.colCnt / 2) ? 3
                : (r > ParentMap.rowCnt / 2 && c > ParentMap.colCnt / 2) ? 4
                : 0;



            public long RegionId { get; set; } = -1;
            public long BuildRegion(long iRegion = -1)
            {
                long cnt = 1;
                if (iRegion < 0)
                    iRegion = ParentMap.Cells.Select(x => x.RegionId).Distinct().Max() + 1;

                RegionId = iRegion;
                //ParentMap.PrintMap(true);
                foreach (var nb in NeighborCellsBase.Where(x => x != null && x.HasGuard == HasGuard && x.RegionId == -1))
                {
                    cnt += nb.BuildRegion(iRegion);
                }

                return cnt;
            }

            #region Distance Logic
            public double CalcDistToCoordMahatten(c14Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c14Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c14Coord CellNorth => NeighborCellsBase[0];
            public c14Coord CellSouth => NeighborCellsBase[1];
            public c14Coord CellEast => NeighborCellsBase[2];
            public c14Coord CellWest => NeighborCellsBase[3];
            public c14Coord CellNE => NeighborCellsBase[4];
            public c14Coord CellNW => NeighborCellsBase[5];
            public c14Coord CellSE => NeighborCellsBase[6];
            public c14Coord CellSW => NeighborCellsBase[7];
            public List<c14Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c14Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c14Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c14Coord>();
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
            #endregion


            public bool Equals(c14Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]|g{Guards.Count}";

        }


        class c14Map : List<List<c14Coord>>
        {
            public c14Map(int iRows, int iCols)
            {
                for (int i = 0; i < iRows; i++)
                {
                    var xcl = new List<c14Coord>();
                    for (int j = 0; j < iCols; j++)
                    {
                        xcl.Add(new c14Coord('.', i, j) { ParentMap = this });
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;


            public int GuardCount => Cells.Sum(x => x.Guards.Count);
            public c14Guard AddGuard(string iData) => new c14Guard(iData, this);
            public List<c14Guard> Guards { get; set; } = new List<c14Guard>();


            public bool LooksLikeTree()
            {

                var midCol = Cells.Where(c => c.c == colCnt / 2);

                if (midCol.Count() - midCol.Where(c=>c.HasGuard).Count()<80)
                {

                    return true;
                }


                return false;
            }

            public long BuildRegions()
            {
                foreach (var cell in Cells) cell.RegionId = -1;

                long regCnt = 0;
                foreach (var cell in Cells)
                {
                    if (cell.RegionId < 0)
                    {
                        if (cell.BuildRegion() > 0)
                            regCnt++;
                    }
                }

                return regCnt;
            }

            public IEnumerable<c14Coord> Cells => this.SelectMany(x => x);

            public void PrintMap()
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.HasGuard ? c.Guards.Count().ToString()
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }


        static void day14()
        {           
            //Console.WriteLine($"Answer1: {day14LogicPart1()}");
            Console.WriteLine($"Answer2: {day14LogicPart2()}");
        }
                

        static long day14LogicPart1()
        {
            var map = new c14Map(103,101); // (7,11);

            foreach(var gd in d14_data)
            {
                map.AddGuard(gd);
            }

            map.PrintMap();
            for (int i=0; i<100; i++)
            {
                foreach(var g in map.Guards)
                {
                    g.Move();
                }
                //map.PrintMap();
            }

            map.PrintMap();

            var ans = map.Cells
                .Where(c => c.HasGuard && c.Quadrant>0)
                .GroupBy(c => c.Quadrant)
                .Select(g => g.Sum(c => c.Guards.Count))
                .Aggregate((a,v)=>a*v);

            return ans;
        }

        static long day14LogicPart2()
        {
            var map = new c14Map(103, 101);

            foreach (var gd in d14_data)
            {
                map.AddGuard(gd);
            }

            map.PrintMap();
            long minrc = 10000;
            var ans = -1;
            for (int i = 0; i < 1000000000000; i++)
            {
                foreach (var g in map.Guards)
                {
                    g.Move();
                }

                var rc = map.BuildRegions();
                if (rc < minrc)
                {
                    minrc = rc;
                    map.PrintMap();
                    Console.WriteLine("");
                }
                else
                {
                    Console.Write($"\x1b[100D{(i == 0 ? Environment.NewLine : "")}{i}: {minrc}");
                }
                                

                if (rc < 300)
                {
                    map.PrintMap();
                    ans = i + 1;
                }
            }


            return ans;
        }

        static string[] d14_data0 =
        """        
        p=0,4 v=3,-3
        p=6,3 v=-1,-3
        p=10,3 v=-1,2
        p=2,0 v=2,-1
        p=0,0 v=1,3
        p=3,0 v=-2,-2
        p=7,6 v=-1,-3
        p=3,0 v=-1,-2
        p=9,3 v=2,3
        p=7,3 v=-1,2
        p=2,4 v=2,-3
        p=9,5 v=-3,-3
        """.Split(Environment.NewLine);

        static string[] d14_data01 =
        """
        p=2,4 v=2,-3
        """.Split(Environment.NewLine);

    }
}
