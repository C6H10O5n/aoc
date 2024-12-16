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


            public c16Coord? PathParent { get; set; } = null;
            public char? PathInDirection { get; set; } = null;
            public int PthCost { get; set; } // Calculated Cost from start to current node
            public int EstCost { get; set; } // Estimated cost from current node to the goal
            public int TotCost => PthCost + EstCost; // Total cost

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
            List<c16Coord>? path;
            public List<c16Coord>? Path => path;
            public List<c16Coord>? BuildPath(int turnCost)
            {
                var openList = new List<c16Coord>();
                var closedList = new HashSet<c16Coord>();

                if (StartCell == null || EndCell == null) 
                    return null;

                openList.Add(StartCell);

                while (openList.Count > 0)
                {
                    // Get the node with the lowest F score
                    openList.Sort((a, b) => a.TotCost.CompareTo(b.TotCost));
                    var current = openList[0];

                    if (current.Equals(EndCell))
                    {
                        // Path found; reconstruct the path
                        path = new List<c16Coord>();
                        while (current != null)
                        {
                            path.Add(current);
                            current = current.PathParent;
                        }
                        path.Reverse();
                        return path;
                    }

                    openList.Remove(current);
                    closedList.Add(current);

                    // Explore neighbors
                    foreach (var neighbor in current.NeighborCells.Where(c=>!c.IsWall))
                    {
                        if (closedList.Contains(neighbor)) continue;

                        int tmpPathCost = current.PthCost + 1; // Base cost for moving

                        // Add turn cost if direction changes
                        if (current.PathInDirection != null && current.PathInDirection != current.GetPathDirectionTo(neighbor))
                        {
                            tmpPathCost += turnCost;
                        }

                        if (!openList.Contains(neighbor) || tmpPathCost < neighbor.PthCost)
                        {
                            neighbor.PthCost = tmpPathCost;
                            neighbor.EstCost = Math.Abs(neighbor.r - EndCell.r) + Math.Abs(neighbor.c - EndCell.c); // Manhattan distance
                            neighbor.PathParent = current;
                            neighbor.PathInDirection = current.GetPathDirectionTo(neighbor);

                            if (!openList.Contains(neighbor))
                                openList.Add(neighbor);
                        }
                    }
                }

                path = null;
                return null; // No path found
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
                        : path != null && path.Contains(c) ? c.PathInDirection
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n");
            }

        }



        static void day16()
        {           
            Console.WriteLine($"Answer1: {day16LogicPart1()}");
            Console.WriteLine($"Answer2: {day16LogicPart2()}");
        }
                

        static long day16LogicPart1()
        {
            var map = new c16Map(d16_data);

            map.PrintMap();

            map.StartCell.PathInDirection = '>';
            var path = map.BuildPath(1000);

            map.PrintMap();

            return map.EndCell.PthCost;
        }

        static long day16LogicPart2()
        {
            return -1;
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
