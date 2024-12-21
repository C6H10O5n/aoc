using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        class c20State
        {
            public c20State(c20Coord cell, char? pathInDirection)
            {
                Cell = cell;
                DirIn = pathInDirection;
                EstCost = Math.Abs(cell.r - cell.ParentMap.EndCell.r) + Math.Abs(cell.c - cell.ParentMap.EndCell.c);
                PthCost = int.MaxValue - EstCost * 2;
            }

            public c20Coord Cell { get; set; }
            public List<c20State> PStates { get; set; } = new List<c20State>();
            public char? DirIn { get; set; } = null;
            public int PthCost { get; set; } // Calculated Cost from start to current node
            public int EstCost { get; set; } //estimated cost from current cell to end
            public int TotCost => PthCost + EstCost;
            public bool IsInBestPath { get; set; }
        }
        class c20Coord
        {
            public c20Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }
            public c20Coord(c20Coord ic) { s = ic.s; r = ic.r; c = ic.c; ; ParentMap = ic.ParentMap; }
            public c20Coord Clone() => new c20Coord(s, r, c) { ParentMap = this.ParentMap };


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c20Map ParentMap { get; set; }

            public bool IsWall => s == '#';
            public bool IsStart => s == 'S';
            public bool IsEnd => s == 'E';

            public bool IsWalkable => ".ES".Contains(s);
            public bool IsCheatOptionNS => CellNorth!=null && CellSouth != null &&  CellNorth.IsWalkable && CellSouth.IsWalkable;
            public bool IsCheatOptionEW => CellEast!=null && CellWest!=null && CellEast.IsWalkable && CellWest.IsWalkable;
            public bool IsCheatOption => IsCheatOptionEW || IsCheatOptionNS;
            public (c20Coord c1, c20Coord c2)? CheatPairsNS => IsCheatOptionNS ? (CellNorth, CellSouth) : null;
            public (c20Coord c1, c20Coord c2)? CheatPairsEW => IsCheatOptionEW ? (CellEast, CellWest) : null;
            public List<(c20Coord c1, c20Coord c2)?> CheatPairs => new List<(c20Coord c1, c20Coord c2)?>() {CheatPairsNS,CheatPairsEW }.Where(x=>x!=null).ToList();
            
            public List<c20Coord> GetWalkableCheatSet(int SearchDistance)=> 
                ParentMap.Cells.Where(x => x.IsWalkable && x!=this && x.CalcDistToCoordMahatten(this) <= SearchDistance).ToList();
            


            public char? GetPathDirectionTo(c20Coord ic) =>
                  ic == CellNorth ? '^'
                : ic == CellSouth ? 'v'
                : ic == CellEast ? '>'
                : ic == CellWest ? '<'
                : null;


            #region Distance Logic
            public double CalcDistToCoordMahatten(c20Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c20Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c20Coord CellNorth => NeighborCellsBase[0];
            public c20Coord CellSouth => NeighborCellsBase[1];
            public c20Coord CellEast => NeighborCellsBase[2];
            public c20Coord CellWest => NeighborCellsBase[3];
            public c20Coord CellNE => NeighborCellsBase[4];
            public c20Coord CellNW => NeighborCellsBase[5];
            public c20Coord CellSE => NeighborCellsBase[6];
            public c20Coord CellSW => NeighborCellsBase[7];
            public List<c20Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c20Coord> NeighborCellsNS => NeighborCellsBase.Where((x, index) => index < 2).ToList();
            public List<c20Coord> NeighborCellsEW => NeighborCellsBase.Where((x, index) => index >=2 && index < 4).ToList();
            public List<c20Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c20Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c20Coord>();
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


            public bool Equals(c20Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }

        class c20Map : List<List<c20Coord>>
        {
            public c20Map(string[] ls, bool expand = false)
            {
                //build map
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xcl = new List<c20Coord>();
                    var ca = ls[i];
                    for (int j = 0; j < ca.Length; j++)
                    {
                        var nc = new c20Coord(ca[j], i, j) { ParentMap = this };
                        xcl.Add(nc);
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public IEnumerable<c20Coord> Cells => this.SelectMany(x => x);
            public c20Coord? StartCell => Cells.Where(c => c.IsStart).FirstOrDefault();
            public c20Coord? EndCell => Cells.Where(c => c.IsEnd).FirstOrDefault();
            public List<c20Coord> WallCells => Cells.Where(c => c.IsWall).ToList();
            public List<c20Coord> WalkableCells => Cells.Where(c => c.IsWalkable).ToList();


            //Find Least Cost Path: A Star Method
            void _extractStatesInPath(c20State st)
            {
                st.IsInBestPath = true;
                st.PStates.ForEach(_extractStatesInPath);
            }
            public List<c20State>? PathsSelected => PathStates.Where(x => x.IsInBestPath).ToList();
            public int pathStateCost(c20Coord ic) => PathStates.Where(x => x.Cell == ic).MinBy(x=>x.PthCost).PthCost;
            public List<c20State>? PathStates { get; set; }
            public bool BuildPath(int turnCost, bool findAll, bool useAstar=false)
            {
                var startState = new c20State(StartCell, '>') { PthCost = 0 };
                PathStates = new List<c20State>() { startState };
                foreach (var cell in Cells.Where(x => x != StartCell && !x.IsWall))
                {
                    foreach (var nc in cell.NeighborCells.Where(x => !x.IsWall))
                    {
                        PathStates.Add(new c20State(cell, nc.GetPathDirectionTo(cell)));
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
                    if (!findAll && useAstar && cur.Cell == EndCell)
                        break;

                    // Explore neighbors
                    foreach (var neighbor in cur.Cell.NeighborCells.Where(c => !c.IsWall))
                    {
                        var st = PathStates.FirstOrDefault(x => x.Cell == neighbor && x.DirIn == cur.Cell.GetPathDirectionTo(neighbor));

                        if (st == null || !ol.Contains(st))
                            continue;

                        var tmpTurnCost = st.DirIn == cur.DirIn ? 0 : turnCost;

                        var tmpPathCost = cur.PthCost + 1 + tmpTurnCost;


                        if ((findAll && tmpPathCost <= st.PthCost) || tmpPathCost < st.PthCost)
                        {
                            st.PthCost = tmpPathCost;
                            if (tmpPathCost < st.PthCost)
                                st.PStates.Clear();
                            st.PStates.Add(cur);
                        }

                    }
                }

                if (PathStates.Where(x => x.Cell == EndCell).Min(x => x.TotCost) < 1000000)
                {
                    _extractStatesInPath(PathStates.Where(x => x.Cell == EndCell).MinBy(x => x.PthCost));
                    //Console.WriteLine($"  -> solver iderations: {i}");
                    return true;

                }
                else return false;
            }

            public void PrintMap(c20Map m = null)
            {
                Console.WriteLine($"\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsStart ? c.s
                        : c.IsEnd ? c.s
                        : c.IsWall ? c.s
                        : PathStates != null && m != null && PathStates.Any(x => x.Cell == c && x.IsInBestPath) && m.WallCells.Any(x => x.r == c.r && x.c == c.c) ? '@'
                        : PathStates != null && PathStates.Any(x => x.Cell == c && x.IsInBestPath) ? 'O'
                        : m != null && m.WallCells.Any(x => x.r == c.r && x.c == c.c) ? '+'
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n");
            }

        }


        static void day20()
        {           
            //Console.WriteLine($"Answer1: {day20LogicPart1()}");
            Console.WriteLine($"Answer2: {day20LogicPart2()}");
        }
                

        static long day20LogicPart1()
        {
            var map = new c20Map(d20_data);

            map.PrintMap();

            map.BuildPath(0, false);
            Console.WriteLine($"steps: {map.PathStates.Count(x => x.IsInBestPath)-1}");


            var cx = map.WallCells
                .Where(x=>x.IsCheatOption)
                .Select(x=>x.CheatPairs)
                .SelectMany(x=>x)
                .Select(x=>(map.pathStateCost(x.Value.c1), map.pathStateCost(x.Value.c2)));

            //map.WallCells.Where(x => x.IsCheatOption).Select(x => x.CheatPairs).SelectMany(x => x)

            var ans = cx.Select(x => Math.Abs(x.Item1 - x.Item2)).GroupBy(x => x).Select(x=>(x.Key-2,x.Count())).OrderBy(x=>x.Item1).ToList();

            //map.PrintMap();

            //1428 too high
            return ans.Where(x=>x.Item1>= 100).Sum(x=>x.Item2);
        }

        static long day20LogicPart2()
        {

            var map = new c20Map(d20_data);

            map.PrintMap();

            map.BuildPath(0, false);
            Console.WriteLine($"steps: {map.PathStates.Count(x => x.IsInBestPath) - 1}");

            var cnt = 0;
            foreach(var wc in map.WalkableCells)//.Where(x=>x== map.EndCell))
            {
                foreach(var cc in wc.GetWalkableCheatSet(20))
                {
                    if (map.pathStateCost(cc) - (map.pathStateCost(wc)+wc.CalcDistToCoordMahatten(cc))>=100)
                    {
                        cnt++;
                        if(cnt%10000==0) Console.WriteLine(cnt);
                    }
                }
            }


            //1014684 too high
            //1014056 too low
            //1014370 too low
            //1014398 incorrect
            return cnt;
        }

        static string[] d20_data0 =
        """        
        ###############
        #...#...#.....#
        #.#.#.#.#.###.#
        #S#...#.#.#...#
        #######.#.#.###
        #######.#.#...#
        #######.#.###.#
        ###..E#...#...#
        ###.#######.###
        #...###...#...#
        #.#####.#.###.#
        #.#...#.#.#...#
        #.#.#.#.#.#.###
        #...#...#...###
        ###############
        """.Split(Environment.NewLine);

    }
}
