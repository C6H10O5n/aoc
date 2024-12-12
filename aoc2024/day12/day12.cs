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



        class c12Coord
        {
            public c12Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;

            }

            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c12Map ParentMap { get; set; }


            public long RegionId { get; set; } = -1;
            public int RegionEdgeIdN { get; set; } = -1;
            public int RegionEdgeIdS { get; set; } = -1;
            public int RegionEdgeIdE { get; set; } = -1;
            public int RegionEdgeIdW { get; set; } = -1;

            public bool IsRegionPerimeter => NeighborCellsBase.Any(x=>x==null || x.RegionId!=RegionId);
            public int PerimeterSides => !IsRegionPerimeter ? 0 : NeighborCellsNSEW.Count(x=> x == null || x.RegionId != RegionId);

            public long BuildRegion(long iRegion = -1)
            {
                long cnt = 1;
                if (iRegion < 0)
                    iRegion = ParentMap.Cells.Select(x => x.RegionId).Distinct().Max()+1;

                RegionId = iRegion;
                //ParentMap.PrintMap(true);
                foreach (var nb in NeighborCellsBase.Where(x => x!=null && x.s == s && x.RegionId==-1))
                {
                    cnt += nb.BuildRegion(iRegion);
                }

                return cnt;
            }


            #region Distance Logic
            public double CalcDistToCoordMahatten(c12Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c12Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c12Coord CellNorth => NeighborCellsBase[0];
            public c12Coord CellSouth => NeighborCellsBase[1];
            public c12Coord CellEast => NeighborCellsBase[2];
            public c12Coord CellWest => NeighborCellsBase[3];
            public c12Coord CellNE => NeighborCellsBase[4];
            public c12Coord CellNW => NeighborCellsBase[5];
            public c12Coord CellSE => NeighborCellsBase[6];
            public c12Coord CellSW => NeighborCellsBase[7];
            public List<c12Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c12Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c12Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c12Coord>();
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

            public bool Equals(c12Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c12Map : List<List<c12Coord>>
        {
            public c12Map(string[] ls)
            {
                for (int i = 0; i < ls.Count(); i++)
                {
                    var ca = ls[i].ToCharArray();
                    var xcl = new List<c12Coord>();
                    for (int j = 0; j < ca.Length; j++)
                    {
                        xcl.Add(new c12Coord(ca[j], i, j) { ParentMap = this });
                    }
                    this.Add(xcl);
                }

            }
            public c12Map(List<c12Coord> iCoords)
            {
                for(var ri = iCoords.Min(x=>x.r); ri<= iCoords.Max(x => x.r); ri++)
                {
                    var clist = new List<c12Coord>();
                    for (var ci = iCoords.Min(x => x.c); ci <= iCoords.Max(x => x.c); ci++)
                    {
                        clist.Add(iCoords.Where(x => x.r == ri && x.c == ci).FirstOrDefault());
                    }
                    this.Add(clist);
                }            
            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public IEnumerable<c12Coord> Cells => this.SelectMany(x => x);
            public IEnumerable<c12Coord> CellsNotNull => this.SelectMany(x => x).Where(x=>x!=null);

            public Dictionary<long, c12Map> RegionDict => Cells
                .GroupBy(x => x.RegionId)
                .ToDictionary(g => g.Key, g => new c12Map(g.ToList()));

            public long BuildRegions()
            {
                long regCnt = 0;
                foreach(var cell in Cells)
                {
                    if (cell.RegionId < 0)
                    {
                        if (cell.BuildRegion() > 0)
                            regCnt++;
                    }
                }                
                
                return regCnt;
            }

            public long BuildRegionEdges()
            {
                //foreach (var cell in CellsNotNull)
                //{
                //    if (cell.CellNorth == null || cell.CellNorth.RegionId != cell.RegionId)
                //    {
                //        cell.RegionEdgeIdN = 1;
                //    }
                //    if (cell.CellSouth == null || cell.CellSouth.RegionId != cell.RegionId)
                //    {
                //        cell.RegionEdgeIdS = 1;
                //    }
                //    if (cell.CellEast == null || cell.CellEast.RegionId != cell.RegionId)
                //    {
                //        cell.RegionEdgeIdE = 1;
                //    }
                //    if (cell.CellWest == null || cell.CellWest.RegionId != cell.RegionId)
                //    {
                //        cell.RegionEdgeIdW = 1;
                //    }
                //}

                var cntN = 0;
                var cntS = 0;
                foreach (var ri in CellsNotNull.Select(x => x.r).Distinct().Order())
                {
                    c12Coord prevCell = null;
                    foreach (var cell in CellsNotNull.Where(x => x.r == ri && (x.CellNorth == null || x.CellNorth.RegionId != x.RegionId)).OrderBy(x => x.c))
                    {
                        if (prevCell==null || prevCell.c + 1 != cell.c)
                        {
                            cntN++;
                        }

                        cell.RegionEdgeIdN = cntN;
                        prevCell = cell;
                    }

                    prevCell = null;
                    foreach (var cell in CellsNotNull.Where(x => x.r == ri && (x.CellSouth == null || x.CellSouth.RegionId != x.RegionId)).OrderBy(x => x.c))
                    {
                        if (prevCell == null || prevCell.c + 1 != cell.c)
                        {
                            cntS++;
                        }

                        cell.RegionEdgeIdS = cntS;
                        prevCell = cell;
                    }
                }

                var cntE = 0;
                var cntW = 0;
                foreach (var ci in CellsNotNull.Select(x => x.c).Distinct().Order())
                {
                    c12Coord prevCell = null;
                    foreach (var cell in CellsNotNull.Where(x => x.c == ci && (x.CellEast == null || x.CellEast.RegionId != x.RegionId)).OrderBy(x => x.r))
                    {
                        if (prevCell == null || prevCell.r + 1 != cell.r)
                        {
                            cntE++;
                        }

                        cell.RegionEdgeIdE = cntE;
                        prevCell = cell;
                    }

                    prevCell = null;
                    foreach (var cell in CellsNotNull.Where(x => x.c == ci && (x.CellWest == null || x.CellWest.RegionId != x.RegionId)).OrderBy(x => x.r))
                    {
                        if (prevCell == null || prevCell.r + 1 != cell.r)
                        {
                            cntW++;
                        }

                        cell.RegionEdgeIdW = cntW;
                        prevCell = cell;
                    }
                }

                return Cells.Max(x=> x == null ? 0 : x.RegionEdgeIdN)
                    + Cells.Max(x => x == null ? 0 : x.RegionEdgeIdS)
                    + Cells.Max(x => x == null ? 0 : x.RegionEdgeIdE)
                    + Cells.Max(x => x == null ? 0 : x.RegionEdgeIdW);
            }



            public void PrintMap(bool printRegion = false)
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          printRegion && c.RegionId>=0 ? c.RegionId.ToString()[0]
                        : c.s
                        );
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }



        static void day12()
        {           
            Console.WriteLine($"Answer1: {day12LogicPart1()}");
            Console.WriteLine($"Answer2: {day12LogicPart2()}");
        }


        static long day12LogicPart1()
        {
            var map = new c12Map(d12_data);

            //map.PrintMap();
            
            map.BuildRegions();


            var regCnt = map.Cells.Select(x => x.RegionId).Distinct().Count();
            var regTotalPreimeterCells = map.Cells.Where(x => x.IsRegionPerimeter).Count();

            var regData = map.Cells
               .GroupBy(x => x.RegionId)
               .ToDictionary(g => g.Key, g => g.ToList());



            var FenceCost = regData.Values.Select(x => x.Count * x.Where(c => c.IsRegionPerimeter).Select(c => c.PerimeterSides).Sum()).Sum();  

            return FenceCost;
        }

        static long day12LogicPart2()
        {
            var map = new c12Map(d12_data);

            //map.PrintMap();

            map.BuildRegions();

            foreach (var rmap in map.RegionDict.Values)
            {
                rmap.BuildRegionEdges();
            }


            var FenceCostOld = map.RegionDict.Values
                .Select(x =>
                      x.CellsNotNull.Count() 
                    * x.CellsNotNull.Where(c => c.IsRegionPerimeter)
                        .Select(c => c.PerimeterSides)
                        .Sum()
                ).Sum();

            var FenceCostNew = map.RegionDict.Values
                .Select(x =>
                      x.CellsNotNull.Count()
                    * x.BuildRegionEdges()
                ).Sum();

            return FenceCostNew;
        }

        static string[] d12_data0 =
        """        
        RRRRIICCFF
        RRRRIICCCF
        VVRRRCCFFF
        VVRCCCJFFF
        VVVVCJJCFE
        VVIVCCJJEE
        VVIIICJJEE
        MIIIIIJJEE
        MIIISIJEEE
        MMMISSJEEE
        """.Split(Environment.NewLine);

    }
}
