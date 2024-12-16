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

        class c15Coord
        {
            public c15Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }
            public c15Coord(c15Coord ic) { s = ic.s;r = ic.r;c = ic.c; }
            public c15Coord Clone() => new c15Coord(s, r, c);


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }


            public c15Map ParentMap { get; set; }
            public bool IsWall => s == '#';
            public bool IsBox => s == 'O' || s == '[' || s == ']';
            public bool IsGuard => s == '@';

            c15Coord? bx2 =>
                  s == '[' ? ParentMap[r][c + 1]
                : s == ']' ? ParentMap[r][c - 1]
                : null;


            bool MoveBox(char mvDir)
            {
                if (!IsBox) return false;

                var mc = getMoveCell(mvDir);
                if (mc == null)
                {
                    return false;
                }
                else if (mc.IsWall)
                {
                    return false;
                }
                else if (mc.IsBox)
                {
                    if (mc.MoveBox(mvDir))
                    {
                        mc.s = this.s;
                        this.s = '.';
                        return true;
                    }
                    else return false;
                }
                else
                {
                    mc.s = this.s;
                    this.s = '.';

                    return true;
                }
            }
            public c15Coord? MoveGuard(char mvDir)
            {
                if (!IsGuard) return null;

                var mc =  getMoveCell(mvDir);
                if (mc == null)
                {
                    return null;
                }
                else if (mc.IsWall)
                {
                    return this;
                }
                else if (mc.IsBox)
                {
                    if (mc.MoveBox(mvDir))
                    {
                        mc.s = this.s;
                        this.s = '.';

                        return mc;
                    }
                    else
                    {
                        return this;
                    }                    
                }
                else
                {
                    mc.s = this.s;
                    this.s = '.';

                    return mc;
                }

            }



            bool MoveBox2(char mvDir, List<(c15Coord cell, char newVal, int depth)>? moves, int depth = -1)
            {
                if (bx2 == null) return false;

                var isEntryPoint = false;
                depth++;
                if (moves == null)
                {
                    moves = new List<(c15Coord cell, char newVal, int depth)>();
                    isEntryPoint = true;
                }

                var mc1 = getMoveCell(mvDir);
                var mc2 = bx2.getMoveCell(mvDir);
                if (mc1 == null || mc2 == null)
                {
                    return false;
                }
                else if (mc1.IsWall || mc2.IsWall)
                {
                    return false;
                }
                else if (mc1.IsBox && mc2.IsBox)
                {
                    if (mc1.MoveBox2(mvDir,moves,depth))
                    {
                        if (mc2.MoveBox2(mvDir,moves,depth))
                        {
                            moves.Add((mc2 , bx2.s, depth));
                            moves.Add((bx2 , '.', depth));
                            moves.Add((mc1 , this.s, depth));
                            moves.Add((this , '.', depth));

                            //Commit Moves
                            if (isEntryPoint)
                            {
                                moves.OrderByDescending(x => x.depth).ToList()
                                    .ForEach(x => x.cell.s = x.newVal);
                            }


                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else if (mc1.IsBox && !mc2.IsBox)
                {
                    if (mc1.MoveBox2(mvDir, moves,depth))
                    {
                        moves.Add((mc2, bx2.s, depth));
                        moves.Add((bx2, '.', depth));
                        moves.Add((mc1, this.s, depth));
                        moves.Add((this, '.', depth));

                        //Commit Moves
                        if (isEntryPoint)
                            moves.OrderByDescending(x => x.depth).ToList()
                                    .ForEach(x => x.cell.s = x.newVal);


                        return true;
                    }
                    else
                        return false;
                }
                else if (!mc1.IsBox && mc2.IsBox)
                {
                    if (mc2.MoveBox2(mvDir, moves,depth))
                    {
                        moves.Add((mc2, bx2.s, depth));
                        moves.Add((bx2, '.', depth));
                        moves.Add((mc1, this.s, depth));
                        moves.Add((this, '.', depth));

                        //Commit Moves
                        if (isEntryPoint)
                            moves.OrderByDescending(x => x.depth).ToList()
                                    .ForEach(x => x.cell.s = x.newVal);


                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    moves.Add((mc2, bx2.s, depth));
                    moves.Add((bx2, '.', depth));
                    moves.Add((mc1, this.s, depth));
                    moves.Add((this, '.', depth));

                    //Commit Moves
                    if (isEntryPoint)
                        moves.OrderByDescending(x => x.depth).ToList()
                                    .ForEach(x => x.cell.s = x.newVal);

                    return true;
                }
            }
            public c15Coord? MoveGuard2(char mvDir)
            {
                if (!IsGuard) return null;

                var mc = getMoveCell(mvDir);
                if (mc == null)
                {
                    return null;
                }
                else if (mc.IsWall)
                {
                    return this;
                }
                else if (mc.IsBox)
                {
                    if (mc.MoveBox2(mvDir,null))
                    {
                        mc.s = this.s;
                        this.s = '.';

                        return mc;
                    }
                    else
                    {
                        return this;
                    }
                }
                else
                {
                    mc.s = this.s;
                    this.s = '.';

                    return mc;
                }

            }

            c15Coord? getMoveCell(char mvDir)=>
                  mvDir == '^' ? CellNorth
                : mvDir == 'v' ? CellSouth
                : mvDir == '>' ? CellEast
                : mvDir == '<' ? CellWest
                : null;


            #region Distance Logic
            public double CalcDistToCoordMahatten(c15Coord ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c15Coord ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c15Coord CellNorth => NeighborCellsBase[0];
            public c15Coord CellSouth => NeighborCellsBase[1];
            public c15Coord CellEast => NeighborCellsBase[2];
            public c15Coord CellWest => NeighborCellsBase[3];
            public c15Coord CellNE => NeighborCellsBase[4];
            public c15Coord CellNW => NeighborCellsBase[5];
            public c15Coord CellSE => NeighborCellsBase[6];
            public c15Coord CellSW => NeighborCellsBase[7];
            public List<c15Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c15Coord> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c15Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c15Coord>();
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


            public bool Equals(c15Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c15Map : List<List<c15Coord>>
        {
            public c15Map(string[] iData, bool expand = false)
            {
                //build map
                var ls = iData[0].Split(Environment.NewLine);
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xcl = new List<c15Coord>();
                    var ca = ls[i];
                    if (expand)
                    {
                        ca = ls[i]
                            .Replace("#", "##")
                            .Replace(".", "..")
                            .Replace("O", "[]")
                            .Replace("@", "@.");
                    }
                    for (int j = 0; j < ca.Length; j++)
                    {
                        var nc = new c15Coord(ca[j], i, j) { ParentMap = this };
                        xcl.Add(nc);
                        //if (expand) {

                        //    if (nc.s == '#') xcl.Add(nc.Clone());
                        //    else if (nc.s == '.') xcl.Add(nc.Clone());
                        //    else if (nc.s == '@') xcl.Add(new c15Coord('.', rowCnt, -1));
                        //    else if (nc.s == 'O')
                        //    {
                        //        nc.s = '[';
                        //        xcl.Add(new c15Coord(']', rowCnt, -1));
                        //    }

                        //    if (j == ca.Length - 1)
                        //        for (int k = 0; k < xcl.Count; k++)
                        //            xcl[k].c = k;
                        //}
                    }
                    this.Add(xcl);
                }

                GuardMoves = iData[1].Replace(Environment.NewLine,"");

            }

            public string GuardMoves { get; set; }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public IEnumerable<c15Coord> Cells => this.SelectMany(x => x);
            public c15Coord? GuardCell => Cells.Where(c => c.IsGuard).FirstOrDefault();
            public List<c15Coord> BoxCells => Cells.Where(c => c.IsBox).ToList();


            public void PrintMap()
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsGuard ? c.s
                        : c.IsBox ? c.s
                        : c.IsWall ? c.s
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }


        static void day15()
        {           
            Console.WriteLine($"Answer1: {day15LogicPart1()}");
            Console.WriteLine($"Answer2: {day15LogicPart2()}");
        }
                

        static long day15LogicPart1()
        {
            var map = new c15Map(d15_data);

            map.PrintMap();

            Console.WriteLine(map.GuardMoves);

            foreach(var mv in map.GuardMoves)
            {
                map.GuardCell.MoveGuard(mv);
                //Console.WriteLine($"Move: {mv}");
                //map.PrintMap();
            }

            var ans = map.BoxCells.Sum(c=>c.r*100+c.c);

            return ans;
        }

        static long day15LogicPart2()
        {
            var map = new c15Map(d15_data, true);

            map.PrintMap();

            var i = 0;
            foreach (var mv in map.GuardMoves)
            {

                if(mv=='<' || mv=='>')
                    map.GuardCell.MoveGuard(mv);
                else
                    map.GuardCell.MoveGuard2(mv);
                i++;
                //Console.WriteLine($"Move {i}: {mv}");
                //map.PrintMap();
            }

            map.PrintMap();

            var ans = map.Cells.Where(c=>c.s=='[').Sum(c => c.r * 100 + c.c);

            return ans;
        }

        static string[] d15_data0 =
        """        
        ##########
        #..O..O.O#
        #......O.#
        #.OO..O.O#
        #..O@..O.#
        #O#..O...#
        #O..O..O.#
        #.OO.O.OO#
        #....O...#
        ##########

        <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
        vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
        ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
        <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
        ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
        ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
        >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
        <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
        ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
        v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
        """.Split(Environment.NewLine+Environment.NewLine);

        static string[] d15_data01 =
        """        
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>>>vv<v>>v<<
        """.Split(Environment.NewLine + Environment.NewLine);

        static string[] d15_data02 =
        """        
        #######
        #...#.#
        #.....#
        #..OO@#
        #..O..#
        #.....#
        #######

        <vv<<^^<<^^
        """.Split(Environment.NewLine + Environment.NewLine);

        static string[] d15_data03 =
        """        
        #######
        #...#.#
        #.....#
        #..O.@#
        #..O..#
        #.....#
        #######

        <vv<<^^<<^^
        """.Split(Environment.NewLine + Environment.NewLine);

        static string[] d15_data04 =
        """        
        #######
        #...#.#
        #..O..#
        #@OOO.#
        #..O..#
        #.....#
        #######

        >>vv>>>^vv<<^^<<^^
        """.Split(Environment.NewLine + Environment.NewLine);

    }
}
