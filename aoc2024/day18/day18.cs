using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace aoc2024
{
    internal partial class Program
    {
        class c18State
        {
            public c18State(c18Coord cell, char? pathInDirection)
            {
                Cell = cell;
                DirIn = pathInDirection;
                EstCost = Math.Abs(cell.r - cell.ParentMap.EndCell.r) + Math.Abs(cell.c - cell.ParentMap.EndCell.c);
                PthCost = int.MaxValue - EstCost * 2;
            }

            public c18Coord Cell { get; set; }
            public List<c18State> PStates { get; set; } = new List<c18State>();
            public char? DirIn { get; set; } = null;
            public int PthCost { get; set; } // Calculated Cost from start to current node
            public int EstCost { get; set; } //estimated cost from current cell to end
            public int TotCost => PthCost + EstCost;
            public bool IsInBestPath { get; set; }
        }
        class c18Coord
        {
            public c18Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }
            public c18Coord(c18Coord ic) { s = ic.s; r = ic.r; c = ic.c; ; ParentMap = ic.ParentMap; }
            public c18Coord Clone() => new c18Coord(s, r, c) { ParentMap = this.ParentMap };


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c18Map ParentMap { get; set; }

            public bool IsWall => s == '#';
            public bool IsStart => s == 'S';
            public bool IsEnd => s == 'E';




            public char? GetPathDirectionTo(c18Coord ic) =>
                  ic == CellNorth ? '^'
                : ic == CellSouth ? 'v'
                : ic == CellEast ? '>'
                : ic == CellWest ? '<'
                : null;


            #region Distance Logic
            public double CalcDistToCoordMahatten(c18Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c18Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c18Coord CellNorth => NeighborCellsBase[0];
            public c18Coord CellSouth => NeighborCellsBase[1];
            public c18Coord CellEast => NeighborCellsBase[2];
            public c18Coord CellWest => NeighborCellsBase[3];
            public c18Coord CellNE => NeighborCellsBase[4];
            public c18Coord CellNW => NeighborCellsBase[5];
            public c18Coord CellSE => NeighborCellsBase[6];
            public c18Coord CellSW => NeighborCellsBase[7];
            public List<c18Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c18Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c18Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c18Coord>();
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


            public bool Equals(c18Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c18Map : List<List<c18Coord>>
        {
            public c18Map(string[] ls, int lcnt, int r, int c)
            {
                //Set Wall Locations
                var xw = ls.Select(GetCommaDelimDigitsAsListInt).Take(lcnt).ToList();

                //build map
                for (int i = 0; i <= r; i++)
                {
                    var xcl = new List<c18Coord>();
                    for (int j = 0; j <= c; j++)
                    {
                        var nc = new c18Coord(xw.Any(x => x[0] == j && x[1] == i) ? '#' : '.', i, j) { ParentMap = this };
                        xcl.Add(nc);
                    }
                    this.Add(xcl);
                }

                //init start/end cells
                Cells.Where(x => x.r == 0 && x.c == 0).First().s = 'S';
                Cells.Where(x => x.r == r && x.c == c).First().s = 'E';


            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public IEnumerable<c18Coord> Cells => this.SelectMany(x => x);
            public c18Coord? StartCell => Cells.Where(c => c.IsStart).FirstOrDefault();
            public c18Coord? EndCell => Cells.Where(c => c.IsEnd).FirstOrDefault();
            public List<c18Coord> WallCells => Cells.Where(c => c.IsWall).ToList();


            //Find Least Cost Path: A Star Method
            void _extractStatesInPath(c18State st)
            {
                st.IsInBestPath = true;
                st.PStates.ForEach(_extractStatesInPath);
            }
            public List<c18State>? PathStates { get; set; }
            public bool BuildPath(int turnCost, bool findAll)
            {
                var startState = new c18State(StartCell, '>') { PthCost = 0 };
                PathStates = new List<c18State>() { startState };
                foreach (var cell in Cells.Where(x => x != StartCell && !x.IsWall))
                {
                    foreach (var nc in cell.NeighborCells.Where(x => !x.IsWall))
                    {
                        PathStates.Add(new c18State(cell, nc.GetPathDirectionTo(cell)));
                    }
                }
                var ol = PathStates.ToList();

                var i = 0;
                while (ol.Count > 0)
                {
                    i++;

                    // Get the node with the lowest cost (use a* if looking for single path)
                    var cur = ol.OrderBy(x => findAll ? x.PthCost : x.TotCost).FirstOrDefault();
                    ol.Remove(cur);

                    //Break if looking for single best path
                    if (!findAll && cur.Cell == EndCell)
                        break;

                    // Explore neighbors
                    foreach (var neighbor in cur.Cell.NeighborCells.Where(c => !c.IsWall))
                    {
                        var st = PathStates.FirstOrDefault(x => x.Cell == neighbor && x.DirIn == cur.Cell.GetPathDirectionTo(neighbor));

                        if (st == null || !ol.Contains(st))
                            continue;

                        var tmpTurnCost = st.DirIn == cur.DirIn ? 0 : turnCost;

                        var tmpPathCost = cur.PthCost + 1 + tmpTurnCost;


                        if ((findAll  && tmpPathCost <= st.PthCost) || tmpPathCost < st.PthCost)
                        {
                            st.PthCost = tmpPathCost;
                            if (tmpPathCost < st.PthCost)
                                st.PStates.Clear();
                            st.PStates.Add(cur);
                        }

                    }
                }

                if(PathStates.Where(x => x.Cell == EndCell).Min(x => x.TotCost) < 1000000)
                {
                    _extractStatesInPath(PathStates.Where(x => x.Cell == EndCell).MinBy(x => x.PthCost));
                    //Console.WriteLine($"  -> solver iderations: {i}");
                    return true;

                }
                else return false;
            }

            public void PrintMap(c18Map m = null)
            {
                Console.WriteLine($"\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsStart ? c.s
                        : c.IsEnd ? c.s
                        : c.IsWall ? c.s
                        : PathStates != null && m!=null && PathStates.Any(x => x.Cell == c && x.IsInBestPath) && m.WallCells.Any(x=> x.r==c.r && x.c==c.c) ? '@'
                        : PathStates != null && PathStates.Any(x=>x.Cell==c && x.IsInBestPath) ? 'O'
                        : m != null && m.WallCells.Any(x => x.r == c.r && x.c==c.c) ? '+'
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n");
            }

        }


        static void day18()
        {           
            //Console.WriteLine($"Answer1: {day18LogicPart1()}");
            Console.WriteLine($"Answer2: {day18LogicPart2()}");
        }
                

        static long day18LogicPart1()
        {
            var map = new c18Map(d18_data, 1024, 70, 70);


            map.PrintMap();

            map.BuildPath(0, false);

            map.PrintMap();

            return map.PathStates.Count(x=>x.IsInBestPath)-1;
        }

        static long day18LogicPart2()
        {
            var map0 = new c18Map(d18_data, 10240, 70, 70);
            var map = new c18Map(d18_data, 1024, 70, 70);
            map.PrintMap();


            var xw = d18_data.Select(GetCommaDelimDigitsAsListInt).ToList();

            var high = xw.Count - 1;
            var low = 1024;
            while (low <= high)
            {

                int mid = (low + high) / 2;
                map0 = new c18Map(d18_data, mid, 70, 70);

                if (!map0.BuildPath(0, false))
                    high = mid - 1;
                else
                    low = mid + 1;

                Console.WriteLine($"  -> solver iderations: {mid}");
            }


            //int i = xw.Count-1;
            //while (!map0.BuildPath(0, false))
            //{
            //    Console.WriteLine($"  -> solver iderations: {xw.Count-i-1}");
            //    map0[xw[i][1]][xw[i][0]].s = '.';
            //    i--;

            //}

            map0.PrintMap();


            Console.WriteLine($"  -> solver anser: {xw[high][0]} : {xw[high][1]} ");

            return xw[high][0] * 100000 + xw[high][1];
        }

        static string[] d18_data0 =
        """        
        5,4
        4,2
        4,5
        3,0
        2,1
        6,3
        2,4
        1,5
        0,6
        3,3
        2,6
        5,1
        1,2
        5,5
        2,5
        6,5
        1,4
        0,4
        6,4
        1,1
        6,1
        1,0
        0,5
        1,6
        2,0
        """.Split(Environment.NewLine);

    }
}
