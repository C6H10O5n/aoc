using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {
        class c10Coord
        {

            //direction Lib
            private static Dictionary<char, List<c10Coord>> mv = new Dictionary<char, List<c10Coord>>{
                { '|', new List<c10Coord>{ new c10Coord('|', 1, 0), new c10Coord('|',-1, 0)} },
                { '-', new List<c10Coord>{ new c10Coord('-', 0, 1), new c10Coord('-', 0,-1)} },
                { 'L', new List<c10Coord>{ new c10Coord('L',-1, 0), new c10Coord('L', 0, 1)} },
                { 'J', new List<c10Coord>{ new c10Coord('J',-1, 0), new c10Coord('J', 0,-1)} },
                { '7', new List<c10Coord>{ new c10Coord('7', 1, 0), new c10Coord('7', 0,-1)} },
                { 'F', new List<c10Coord>{ new c10Coord('F', 1, 0), new c10Coord('F', 0, 1)} },
                { 'S', new List<c10Coord>{ new c10Coord('S', 1, 0), new c10Coord('S', 0, 1), new c10Coord('S', -1, 0), new c10Coord('S', 0, -1)} }
            };

            public c10Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }
            public c10Coord(int ir, int ic) : this(' ', ir, ic) { }

            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public char directionOut { get; private set; } = '?';
            public void setDirectionOut(c10Coord toCoord)
            {
                directionOut =
                      r < toCoord.r ? 'S' //r increases
                    : r > toCoord.r ? 'N' //r decreases
                    : c < toCoord.c ? 'E' //c increases
                    : c > toCoord.c ? 'W' //c decreases
                    : '?';


                directionIn = 
                    s == 'F' && directionOut == 'E' ? 'S' :
                    s == 'F' && directionOut == 'S' ? 'E' :

                    s == '7' && directionOut == 'W' ? 'S' :
                    s == '7' && directionOut == 'S' ? 'W' :

                    s == 'L' && directionOut == 'E' ? 'N' :
                    s == 'L' && directionOut == 'N' ? 'E' :

                    s == 'J' && directionOut == 'W' ? 'N' :
                    s == 'J' && directionOut == 'N' ? 'W' :

                    s == '-' && directionOut == 'E' ? 'W' :
                    s == '-' && directionOut == 'W' ? 'E' :

                    s == '|' && directionOut == 'N' ? 'S' :
                    s == '|' && directionOut == 'S' ? 'N' :

                    directionOut;

            }
            public char directionIn { get; private set; }
            public List<c10Coord> GetCellsLeftOfDirection(int rCnt, int cCnt)
            {
                var leftCells = new List<c10Coord>();


                switch (s)
                {
                    case 'F':
                        switch (directionIn)
                        {
                            case 'S':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r > 0) leftCells.Add(new c10Coord(r - 1, c - 1)); //NW
                                break;
                            case 'E':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c + 1)); //SE
                                break;
                        }
                        break;
                    case '7':
                        switch (directionIn)
                        {
                            case 'S':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c - 1)); //SW
                                break;
                            case 'W':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r > 0) leftCells.Add(new c10Coord(r - 1, c + 1)); //NE
                                break;
                        }
                        break;
                    case 'L': 
                        switch (directionIn)
                        {
                            case 'N':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r > 0) leftCells.Add(new c10Coord(r - 1, c + 1)); //NE
                                break;
                            case 'W':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c + 1)); //SE
                                break;
                        }
                        break;
                    case 'J':
                        switch (directionIn)
                        {
                            case 'N':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c - 1)); //SW
                                break;
                            case 'E':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r > 0) leftCells.Add(new c10Coord(r - 1, c - 1)); //NW
                                break;
                        }
                        break;
                    case '-':
                        switch (directionIn)
                        {
                            case 'E':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                break;
                            case 'W':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                break;
                        }
                        break;
                    case '|':
                        switch (directionIn)
                        {
                            case 'N':
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                break;
                            case 'S':
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                break;
                        }
                        break;
                }


                

                return leftCells;
            }

            public List<c10Coord> GetCellsRightOfDirection(int rCnt, int cCnt)
            {
                var leftCells = new List<c10Coord>();


                switch (s)
                {
                    case 'F':
                        switch (directionIn)
                        {
                            case 'E':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r > 0) leftCells.Add(new c10Coord(r - 1, c - 1)); //NW
                                break;
                            case 'S':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c + 1)); //SE
                                break;
                        }
                        break;
                    case '7':
                        switch (directionIn)
                        {
                            case 'W':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c - 1)); //SW
                                break;
                            case 'S':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r > 0) leftCells.Add(new c10Coord(r - 1, c + 1)); //NE
                                break;
                        }
                        break;
                    case 'L':
                        switch (directionIn)
                        {
                            case 'W':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r > 0) leftCells.Add(new c10Coord(r - 1, c + 1)); //NE
                                break;
                            case 'N':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                if (c < cCnt - 1 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c + 1)); //SE
                                break;
                        }
                        break;
                    case 'J':
                        switch (directionIn)
                        {
                            case 'E':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c - 1)); //SW
                                break;
                            case 'N':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                if (c > 0 && r > 0) leftCells.Add(new c10Coord(r - 1, c - 1)); //NW
                                break;
                        }
                        break;
                    case '-':
                        switch (directionIn)
                        {
                            case 'W':
                                if (r < rCnt - 1) leftCells.Add(new c10Coord(r + 1, c)); //S
                                break;
                            case 'E':
                                if (r > 0) leftCells.Add(new c10Coord(r - 1, c)); //N
                                break;
                        }
                        break;
                    case '|':
                        switch (directionIn)
                        {
                            case 'S':
                                if (c < cCnt - 1) leftCells.Add(new c10Coord(r, c + 1)); //E
                                break;
                            case 'N':
                                if (c > 0) leftCells.Add(new c10Coord(r, c - 1)); //W
                                break;
                        }
                        break;
                }




                return leftCells;
            }


            public double X => r + .05;
            public double Y => c + .05;

            public bool isMoveCell => mv.Keys.Contains(s);
            public List<c10Coord> MoveCells => isMoveCell ? mv[s] : null;
            public bool isVaildMove(c10Coord ic)
            {
                if (!isMoveCell) return false;
                if (MoveCells.Any(mc => r + mc.r == ic.r && c + mc.c == ic.c)) return true;
                return false;
            }

            public bool Equals(c10Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]{directionIn}";

        }


        class c10Map : List<string>
        {
            public c10Map(string[] ls) => this.AddRange(ls);

            //helper methods
            char getcv(c10Coord c) => this[c.r][c.c];
            bool cInRng(c10Coord c) => c.r >= 0 && c.c >= 0 && c.r < this.Count && c.c < this.Last().Length;


            //find start S
            int GetStrartRow() => this.TakeWhile(x => !x.Contains('S')).Count();
            int GetStrartCol() => this[GetStrartRow()].IndexOf('S');
            public c10Coord GetStratCoord() => new c10Coord('S', GetStrartRow(), GetStrartCol());



            //get neighbouring cell coordinates
            public List<c10Coord> getCells(int r, int c) => new List<c10Coord> {
                                           new c10Coord(r - 1, c + 0),
                new c10Coord(r + 0, c - 1)                           ,new c10Coord(r + 0, c + 1),
                                           new c10Coord(r + 1, c + 0)
            }.Where(c => cInRng(c)).ToList();
            public List<c10Coord> getCellsDiag(int r, int c) => new List<c10Coord> {
                new c10Coord(r - 1, c - 1),new c10Coord(r - 1, c + 0),new c10Coord(r - 1, c + 1),
                new c10Coord(r + 0, c - 1)                           ,new c10Coord(r + 0, c + 1),
                new c10Coord(r + 1, c - 1),new c10Coord(r + 1, c + 0),new c10Coord(r + 1, c + 1)
            }.Where(c => cInRng(c)).ToList();
            public List<c10Coord> GetValidMoveCells(c10Coord ic) => getCells(ic.r, ic.c)
              .Select(c => new c10Coord(getcv(c), c.r, c.c))
             .Where(c => ic.isVaildMove(c) && c.isVaildMove(ic))
             .ToList();



            // search for path
            public List<c10Coord> Path = null; 
            public List<c10Coord> rSearchPath(c10Coord cc = null, c10Coord pc = null, List<c10Coord> path = null)
            {
                if (cc != null && cc.s == 'S')
                {
                    path.Add(cc);
                    return path;
                }
                if (cc == null) cc = GetStratCoord();
                if (pc == null) pc = cc;
                if (path == null) path = new List<c10Coord>();
                path.Add(cc);
                var mvs = GetValidMoveCells(cc).Where(c => !c.Equals(pc)).ToList();

                if (!mvs.Any())
                {
                    path.Remove(cc);
                    return path;
                }
                else
                {
                    foreach (var cci in mvs)
                    {
                        var xp = rSearchPath(cci, cc, path);
                        if (xp.Last().s == 'S')
                        {
                            return path;
                        }
                        else
                        {
                            path.Remove(cci);
                        }
                    };
                }

                return path;

            }
            public List<c10Coord> BuildPath()
            {
                var sc = GetStratCoord();
                var path = new List<c10Coord>();
                path.Add(sc);

                var cc = sc;
                var pc = sc;
                var mvs = GetValidMoveCells(cc);
                while (mvs.Any())
                {
                    cc = mvs.FirstOrDefault();
                    path.Add(cc);

                    if (cc.s == 'S') break;

                    mvs = GetValidMoveCells(cc);
                    mvs = mvs.Where(c => !c.Equals(pc)).ToList();
                    pc = cc;
                }

                Path = path;
                AssignDirectionToPath();
                BuildCellsLeftOfPath();
                BuildCellsRightOfPath();
                return path;

            }


            //Assign Direction to Path
            private void AssignDirectionToPath()
            {
                if (Path == null || Path.Count<2) return;

                for (var i = 0;i<Path.Count-1; i++)
                {
                    Path[i].setDirectionOut(Path[i+1]);
                }
                Path.Last().setDirectionOut(Path[1]); //repeat of start cell
            }

            //Find cells Left of Path
            public List<c10Coord> CellsLeftOfPath = null;
            private void BuildCellsLeftOfPath()
            {
                if (Path == null || Path.Count < 2) return;

                CellsLeftOfPath = new List<c10Coord>();

                for (var i = 0; i < Path.Count - 1; i++)
                {
                    if (i == 80)
                    {
                        i = i;
                    }

                    foreach (var lc in Path[i].GetCellsLeftOfDirection(Count, this[0].Length))
                    {
                        if(!Path.Any(p => p.r == lc.r && p.c == lc.c))
                            if(!CellsLeftOfPath.Any(p => p.r == lc.r && p.c == lc.c))
                                CellsLeftOfPath.Add(lc);
                    }
                }
            }


            //Find cells Right of Path
            public List<c10Coord> CellsRightOfPath = null;
            private void BuildCellsRightOfPath()
            {
                if (Path == null || Path.Count < 2) return;

                CellsRightOfPath = new List<c10Coord>();

                for (var i = 0; i < Path.Count - 1; i++)
                {
                    if (i == 80)
                    {
                        i = i;
                    }

                    foreach (var lc in Path[i].GetCellsRightOfDirection(Count, this[0].Length))
                    {
                        if (!Path.Any(p => p.r == lc.r && p.c == lc.c))
                            if (!CellsRightOfPath.Any(p => p.r == lc.r && p.c == lc.c))
                                CellsRightOfPath.Add(lc);
                    }
                }
            }

        }


        static void day10()
        {
            var map = new c10Map(d10_data0a3b);

            //follow pipe
            var path = map.BuildPath();

            Console.WriteLine($"map size=: rows: {map.Count}: columns: {map[0].Length}: cells: {map.Count*map[0].Length}");
            Console.WriteLine($"cells in path= {map.Path.Count - 1}");
            Console.WriteLine($"cells in left/right adjacent to Path= {map.CellsLeftOfPath.Select(c=>$"[{c.r},{c.c}]").Union(map.CellsRightOfPath.Select(c => $"[{c.r},{c.c}]")).Distinct().Count()}");
            Console.WriteLine($"\n\n");


            Console.WriteLine($"Answer1: steps={path.Count - 1}: furthest(steps/2)={(path.Count - 1) / 2}");
            


            Console.WriteLine($"Part2....: \n    -CellsLeftOfPath={map.CellsLeftOfPath.Count}\n    -CellsRightOfPath={map.CellsRightOfPath.Count}");

            var ipl = new List<c10Coord>();
            for (var i = 0; i < map.Count; i++)
            {
                char[] ck = new char[map[i].Length];
                var cks = "";
                for (int j = 0; j < ck.Length; j++)
                {
                    if(map.CellsLeftOfPath.Any(p => p.r == i && p.c == j))
                    {
                        while(j<ck.Length-1 
                            && !map.Path.Any(p => p.r == i && p.c == j + 1)
                            && !map.CellsLeftOfPath.Any(p => p.r == i && p.c == j + 1)
                            && !map.CellsRightOfPath.Any(p => p.r == i && p.c == j + 1)
                        )
                        {
                            j++;
                            ipl.Add(new c10Coord(i, j));
                        }
                    }
                }
                cks = new string(ck);
            }


            Console.WriteLine($"\n    -Left span cells={ipl.Count}"); 



             var ipr = new List<c10Coord>();
            for (var i = 0; i < map.Count; i++)
            {
                char[] ck = new char[map[i].Length];
                var cks = "";
                for (int j = ck.Length -1; j >= 0 ; j--)
                {
                    if (map.CellsRightOfPath.Any(p => p.r == i && p.c == j))
                    {
                        while (j > 0
                            && !map.Path.Any(p => p.r == i && p.c == j - 1)
                            && !map.CellsLeftOfPath.Any(p => p.r == i && p.c == j - 1)
                            && !map.CellsRightOfPath.Any(p => p.r == i && p.c == j - 1)
                        )
                        {
                            j--;
                            ipr.Add(new c10Coord(i, j));
                        }
                    }
                }
                cks = new string(ck);
            }


            Console.WriteLine($"    -Right span cells={ipr.Count}"); 



            Console.WriteLine($"\n    -LeftTotal={ipr.Count+ map.CellsLeftOfPath.Count}"); // 262 > ans < 842 (!289; !521)
            Console.WriteLine($"    -RightTotal={ipr.Count + map.CellsRightOfPath.Count}"); // 262 > ans < 842 (!289; !521)



        }

        static string[] d10_data0a1 =
        """
        .....
        .S-7.
        .|.|.
        .L-J.
        .....
        """.Split("\r\n");

        static string[] d10_data0a2 =
        """
        -L|F7
        7S-7|
        L|7||
        -L-J|
        L|-JF
        """.Split("\r\n");

        static string[] d10_data0a3 =
        """
        ..........
        .S------7.
        .|F----7|.
        .||OOOO||.
        .||OOOO||.
        .|L-7F-J|.
        .|II||II|.
        .L--JL--J.
        ..........
        """.Split("\r\n");

        static string[] d10_data0a3b =
        """
        ................
        .S------------7.
        .|F----------7|.
        .||OOOOOOOOOO||.
        .||OOOOOOOOOO||.
        .|L----7F----J|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .|IIIII||IIIII|.
        .L-----JL-----J.
        ................
        """.Split("\r\n");

        static string[] d10_data0a4 =
        """
        .F----7F7F7F7F-7....
        .|F--7||||||||FJ....
        .||.FJ||||||||L7....
        FJL7L7LJLJ||LJ.L-7..
        L--J.L7...LJS7F-7L7.
        ....F-J..F7FJ|L7L7L7
        ....L7.F7||L7|.L7L7|
        .....|FJLJ|FJ|F7|.LJ
        ....FJL-7.||.||||...
        ....L---J.LJ.LJLJ...
        """.Split("\r\n");

        static string[] d10_data0a4b =
        """
        .F----7F7F7F7F-7....
        .|F--7||||||||FJ....
        .||.FJ||||||||L7....
        FJL7L7LJLJLJLJ.L-7..
        |..|.|...........|..
        |..|.|...........|..
        |..|.|...........|..
        L--J.L7.....S7F-7L7.
        ....F-J..F7FJ|L7L7L7
        ....L7.F7||L7|.L7L7|
        .....|FJLJ|FJ|F7|.LJ
        ....FJL-7.||.||||...
        ....L---J.LJ.LJLJ...
        """.Split("\r\n");

        static string[] d10_data0a5 =
        """
        FF7FSF7F7F7F7F7F---7
        L|LJ||||||||||||F--J
        FL-7LJLJ||||||LJL-77
        F--JF--7||LJLJ7F7FJ-
        L---JF-JLJ.||-FJLJJ7
        |F|F-JF---7F7-L7L|7|
        |FFJF7L7F-JF7|JL---7
        7-L-JL7||F7|L7F-7F7|
        L.L7LFJ|||||FJL7||LJ
        L7JLJL-JLJLJL--JLJ.L
        """.Split("\r\n");

        static string[] d10_data0b1 =
        """
        ..F7.
        .FJ|.
        SJ.L7
        |F--J
        LJ...
        """.Split("\r\n");

        static string[] d10_data0b2 =
        """
        7-F7-
        .FJ|7
        SJLL7
        |F--J
        LJ.LJ
        """.Split("\r\n");


    }
}
