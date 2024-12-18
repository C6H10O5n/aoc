using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace aoc2024
{
    internal partial class Program
    {
        class c16State
        {
            public c16State(c16Coord cell,char? pathInDirection)
            {
                Cell = cell;
                Dir = pathInDirection;
            }

            public c16Coord Cell{ get; set; }
            public List<c16State> PStates { get; set; } = new List<c16State>();
            public char? Dir { get; set; } = null;
            public int Cost { get; set; } = int.MaxValue; // Calculated Cost from start to current node
            public bool IsInBestPath { get; set; }
        }
        class c16Coord
        {
            public c16Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }
            public c16Coord(c16Coord ic) { s = ic.s; r = ic.r; c = ic.c; ; ParentMap = ic.ParentMap; }
            public c16Coord Clone() => new c16Coord(s, r, c) { ParentMap=this.ParentMap};


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c16Map ParentMap { get; set; }

            public bool IsWall => s == '#';
            public bool IsStart => s == 'S';
            public bool IsEnd => s == 'E';




            public char? GetPathDirectionTo(c16Coord ic) =>
                  ic == CellNorth ? '^'
                : ic == CellSouth ? 'v'
                : ic == CellEast ? '>'
                : ic == CellWest ? '<'
                : null;


            #region Distance Logic
            public double CalcDistToCoordMahatten(c16Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c16Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c16Coord CellNorth => NeighborCellsBase[0];
            public c16Coord CellSouth => NeighborCellsBase[1];
            public c16Coord CellEast => NeighborCellsBase[2];
            public c16Coord CellWest => NeighborCellsBase[3];
            public c16Coord CellNE => NeighborCellsBase[4];
            public c16Coord CellNW => NeighborCellsBase[5];
            public c16Coord CellSE => NeighborCellsBase[6];
            public c16Coord CellSW => NeighborCellsBase[7];
            public List<c16Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c16Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c16Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c16Coord>();
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


            public bool Equals(c16Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c16Map : List<List<c16Coord>>
        {
            public c16Map(string[] ls, bool expand = false)
            {
                //build map
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xcl = new List<c16Coord>();
                    var ca = ls[i];
                    for (int j = 0; j < ca.Length; j++)
                    {
                        var nc = new c16Coord(ca[j], i, j) { ParentMap = this };
                        xcl.Add(nc);
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public IEnumerable<c16Coord> Cells => this.SelectMany(x => x);
            public c16Coord? StartCell => Cells.Where(c => c.IsStart).FirstOrDefault();
            public c16Coord? EndCell => Cells.Where(c => c.IsEnd).FirstOrDefault();
            public List<c16Coord> WallCells => Cells.Where(c => c.IsWall).ToList();


            //Find Least Cost Path: A Star Method
            public List<c16State>? PathStates { get; set; }
            public List<c16State>? BuildPath(int turnCost)
            {
                var stl = new List<c16State>();
                PathStates = stl;
                var ol = new List<c16State>();
                foreach (var cell in Cells.Where(x => !x.IsWall))
                {
                    foreach (var nc in "^v<>")
                    {
                        var st = new c16State(cell, nc);
                        ol.Add(st);
                        stl.Add(st);
                    }
                }

                ol.Where(x => x.Cell == StartCell && x.Dir == '>').FirstOrDefault().Cost = 0;
                while (ol.Count > 0)
                {
                    // Get the node with the lowest cost
                    var cur = ol.OrderBy(x => x.Cost).FirstOrDefault();
                    ol.Remove(cur);

                    if(cur.Cell.r==7 && cur.Cell.c == 5)
                    {
                        cur = cur;
                    }

                    // Explore neighbors
                    foreach (var neighbor in cur.Cell.NeighborCells.Where(c => !c.IsWall))
                    {
                        var st = stl.FirstOrDefault(x => x.Cell == neighbor && x.Dir == cur.Cell.GetPathDirectionTo(neighbor));
                        if (st == null || !ol.Contains(st)) continue;

                        var tmpTurnCost = st.Dir == cur.Dir ? 0 : turnCost;

                        var tmpPathCost = cur.Cost + 1 + tmpTurnCost;


                        if (tmpPathCost <= st.Cost)
                        {
                            st.Cost = tmpPathCost;
                            if (tmpPathCost < st.Cost)
                                st.PStates.Clear();
                            st.PStates.Add(cur);
                        }

                    }
                }

                void walkPath(c16State st)
                {
                    st.IsInBestPath = true;
                    st.PStates.ForEach(walkPath);
                }
                walkPath(PathStates.Where(x => x.Cell == EndCell).MinBy(x => x.Cost));

                return PathStates; // No path found
            }

            public void PrintMap()
            {
                Console.WriteLine($"\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsStart ? c.s
                        : c.IsEnd ? c.s
                        : c.IsWall ? c.s
                        //: path != null && path.Contains(c) ? c.PathInDirection
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n");
            }

        }



        static void day16()
        {           
            //Console.WriteLine($"Answer1: {day16LogicPart1()}");
            Console.WriteLine($"Answer2: {day16LogicPart2()}");
        }
                

        static long day16LogicPart1()
        {
            var map = new c16Map(d16_data);

            map.PrintMap();
                        
            var path = map.BuildPath(1000);

            map.PrintMap();

            return map.PathStates.Where(x=>x.Cell==map.EndCell).Min(x=>x.Cost);
        }

        static long day16LogicPart2()
        {

            var map = new c16Map(d16_data);

            map.PrintMap();

            var path = map.BuildPath(1000);

            map.PrintMap();

            


            return map.PathStates.Where(x => x.IsInBestPath).DistinctBy(x=>x.Cell).Count();
        }

        static string[] d16_data0 =
        """        
        ###############
        #.......#....E#
        #.#.###.#.###.#
        #.....#.#...#.#
        #.###.#####.#.#
        #.#.#.......#.#
        #.#.#####.###.#
        #...........#.#
        ###.#.#####.#.#
        #...#.....#.#.#
        #.#.#.###.#.#.#
        #.....#...#.#.#
        #.###.#.#.#.#.#
        #S..#.....#...#
        ###############
        """.Split(Environment.NewLine);

        static string[] d16_data01 =
        """        
        #################
        #...#...#...#..E#
        #.#.#.#.#.#.#.#.#
        #.#.#.#...#...#.#
        #.#.#.#.###.#.#.#
        #...#.#.#.....#.#
        #.#.#.#.#.#####.#
        #.#...#.#.#.....#
        #.#.#####.#.###.#
        #.#.#.......#...#
        #.#.###.#####.###
        #.#.#...#.....#.#
        #.#.#.#####.###.#
        #.#.#.........#.#
        #.#.#.#########.#
        #S#.............#
        #################
        """.Split(Environment.NewLine);

    }
}
