using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
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

            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public c10Map ParentMap { get; set; }


            public int PathId { get; set; } = -1;
            public bool IsOnPath => PathId >= 0;
            public bool IsInsidePath { get; set; }
            public bool IsRightOfPath { get; set; }
            public bool IsLeftOfPath { get; set; }
            public bool IsUnassigned => !IsLeftOfPath && !IsRightOfPath && !IsOnPath;


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
                var dcells = new List<c10Coord>();


                switch (s)
                {
                    case 'F': //NW-SE
                        switch (directionIn)
                        {
                            case 'S':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellNW != null) dcells.Add(CellNW); //NW
                                break;
                            case 'E':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellSE != null) dcells.Add(CellSE); //SE
                                break;
                        }
                        break;
                    case '7': //NE-SW
                        switch (directionIn)
                        {
                            case 'S':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellSW != null) dcells.Add(CellSW); //SW
                                break;
                            case 'W':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellNE != null) dcells.Add(CellNE); //NE
                                break;
                        }
                        break;
                    case 'L': //
                        switch (directionIn)
                        {
                            case 'N':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellNE != null) dcells.Add(CellNE); //NE
                                break;
                            case 'E':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellSW != null) dcells.Add(CellSW); //SW
                                break;
                        }
                        break;
                    case 'J':
                        switch (directionIn)
                        {
                            case 'N':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellSE != null) dcells.Add(CellSE); //SE
                                break;
                            case 'W':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellNW != null) dcells.Add(CellNW); //NW
                                break;
                        }
                        break;
                    case '-':
                        switch (directionIn)
                        {
                            case 'E':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                break;
                            case 'W':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                break;
                        }
                        break;
                    case '|':
                        switch (directionIn)
                        {
                            case 'N':
                                if (CellEast != null) dcells.Add(CellEast); //E
                                break;
                            case 'S':
                                if (CellWest != null) dcells.Add(CellWest); //W
                                break;
                        }
                        break;
                }



                return dcells;
            }

            public List<c10Coord> GetCellsRightOfDirection(int rCnt, int cCnt)
            {
                var dcells = new List<c10Coord>();


                switch (s)
                {
                    case 'F': //NW-SE
                        switch (directionIn)
                        {
                            case 'E':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellNW != null) dcells.Add(CellNW); //NW
                                break;
                            case 'S':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellSE != null) dcells.Add(CellSE); //SE
                                break;
                        }
                        break;
                    case '7': //NE-SW
                        switch (directionIn)
                        {
                            case 'W':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellSW != null) dcells.Add(CellSW); //SW
                                break;
                            case 'S':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellNE != null) dcells.Add(CellNE); //NE
                                break;
                        }
                        break;
                    case 'L': //
                        switch (directionIn)
                        {
                            case 'E':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellNE != null) dcells.Add(CellNE); //NE
                                break;
                            case 'N':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellSW != null) dcells.Add(CellSW); //SW
                                break;
                        }
                        break;
                    case 'J':
                        switch (directionIn)
                        {
                            case 'W':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                if (CellEast != null) dcells.Add(CellEast); //E
                                if (CellSE != null) dcells.Add(CellSE); //SE
                                break;
                            case 'N':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                if (CellWest != null) dcells.Add(CellWest); //W
                                if (CellNW != null) dcells.Add(CellNW); //NW
                                break;
                        }
                        break;
                    case '-':
                        switch (directionIn)
                        {
                            case 'W':
                                if (CellSouth != null) dcells.Add(CellSouth); //S
                                break;
                            case 'E':
                                if (CellNorth != null) dcells.Add(CellNorth); //N
                                break;
                        }
                        break;
                    case '|':
                        switch (directionIn)
                        {
                            case 'S':
                                if (CellEast != null) dcells.Add(CellEast); //E
                                break;
                            case 'N':
                                if (CellWest != null) dcells.Add(CellWest); //W
                                break;
                        }
                        break;
                }




                return dcells;
            }


            c10Coord CellNorth => NeighborCellsBase[0];
            c10Coord CellSouth => NeighborCellsBase[1];
            c10Coord CellEast  => NeighborCellsBase[2];
            c10Coord CellWest  => NeighborCellsBase[3];
            c10Coord CellNE    => NeighborCellsBase[4];
            c10Coord CellNW    => NeighborCellsBase[5];
            c10Coord CellSE    => NeighborCellsBase[6];
            c10Coord CellSW    => NeighborCellsBase[7];
            public List<c10Coord> NeighborCells => NeighborCellsBase.Where((x,index) => x != null && index < 4).ToList();
            public List<c10Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c10Coord>();
                        var rc = ParentMap.rowCnt - 1;
                        var cc = ParentMap.colCnt - 1;
                        if (r >  0) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//N
                        if (r < rc) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//S
                        if (c < cc) nc.Add(ParentMap[r + 0][c + 1]); else nc.Add(null);//E
                        if (c >  0) nc.Add(ParentMap[r + 0][c - 1]); else nc.Add(null);//W


                        if (r >  0 && c < cc) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//NE
                        if (r >  0 && c >  0) nc.Add(ParentMap[r - 1][c + 0]); else nc.Add(null);//NW
                        if (r < rc && c < cc) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//SE
                        if (r < rc && c >  0) nc.Add(ParentMap[r + 1][c + 0]); else nc.Add(null);//SW

                        return nc;
                    }
                }
            }


            public c10Coord MoveNext(c10Coord pc)
            {
                if (!isMoveCell) return null;
                PathId = pc.PathId+1;

                switch (s)
                {
                    case 'F': return pc == CellSouth ? CellEast: CellSouth;
                    case 'L': return pc == CellNorth ? CellEast: CellNorth;
                    case 'J': return pc == CellNorth ? CellWest: CellNorth;
                    case '7': return pc == CellSouth ? CellWest: CellSouth;
                    case '-': return pc == CellWest  ? CellEast: CellWest;
                    case '|': return pc == CellNorth ? CellSouth: CellNorth;
                    default: return null;
                        //case 'S': return c > ic.c || r > ic.r;
                }


                //return NeighborCells.Where(c => c.isMoveCell && !c.IsOnPath).Where(c => this.isVaildMoveTo(c)).FirstOrDefault();

            }
            public c10Coord MoveNext(int idx)
            {
                if (!isMoveCell) return null;
                PathId=idx;


                return NeighborCells.Where(c => c.isMoveCell && !c.IsOnPath).Where(c=>this.isVaildMoveTo(c)).FirstOrDefault();

            }

            public bool isMoveCell => mv.Keys.Contains(s);
            public List<c10Coord> MoveCells => isMoveCell ? mv[s] : null;

            public bool isVaildMoveTo(c10Coord ic)
            {
                if (!isMoveCell) return false;
                switch (ic.s)
                {
                    case 'F': return c > ic.c || r > ic.r;
                    case 'L': return c > ic.c || r < ic.r;
                    case 'J': return c < ic.c || r < ic.r;
                    case '7': return c < ic.c || r > ic.r;
                    case '-': return c != ic.c || r == ic.r;
                    case '|': return c == ic.c || r != ic.r;
                  //case 'S': return c > ic.c || r > ic.r;
                }
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


        class c10Map : List<List<c10Coord>>
        {
            public c10Map(string[] ls)
            {
                for (int i = 0; i < ls.Count(); i++)
                {
                    var ca = ls[i].ToCharArray();
                    var xc = new List<c10Coord>();
                    for (int j = 0; j < ca.Length; j++)
                    {
                       xc.Add(new c10Coord(ca[j], i, j) { ParentMap = this});
                    }
                    this.Add(xc);
                }
            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;


            //find start S
            int StrartRow() => StratCoord.r;
            int StrartCol() => StratCoord.c;
            public c10Coord StratCoord => this.SelectMany(x => x).Where(c=>c.s=='S').FirstOrDefault();


            // search for path
            public List<c10Coord> Path = null;
            public void BuildPath()
            {
                var cc = StratCoord;
                var pc = cc;
                cc = cc.MoveNext(0);
                while (true)
                {
                    var tc=cc.MoveNext(pc);
                    pc = cc;
                    cc = tc;
                    if (cc.s == 'S') break;
                }
                Path = this.SelectMany(x=>x).Where(x=>x.PathId>=0).OrderBy(x=>x.PathId).ToList();
                AssignDirectionToPath();
                BuildCellsLeftOfPath();
                BuildCellsRightOfPath();
                FillUnassignedCells();

            }


            //Assign Direction to Path
            private void AssignDirectionToPath()
            {
                if (Path == null || Path.Count<2) return;

                for (var i = 0;i<Path.Count-1; i++)
                {
                    Path[i].setDirectionOut(Path[i+1]);
                }
                Path.Last().setDirectionOut(Path.First());
            }


            //Find cells Left of Path
            public List<c10Coord> CellsLeftOfPath = null;
            private void BuildCellsLeftOfPath()
            {
                if (Path == null || Path.Count < 2) return;


                for (var i = 0; i < Path.Count - 1; i++)
                {
                    foreach (var lc in Path[i].GetCellsLeftOfDirection(rowCnt, colCnt))
                    {
                        if (!lc.IsOnPath && !lc.IsLeftOfPath)
                        {
                            lc.IsLeftOfPath = true;
                        }
                    }
                }
                CellsLeftOfPath = this.SelectMany(x => x).Where(x => x.IsLeftOfPath).ToList();
            }


            //Find cells Right of Path
            public List<c10Coord> CellsRightOfPath = null;
            private void BuildCellsRightOfPath()
            {
                if (Path == null || Path.Count < 2) return;

                for (var i = 0; i < Path.Count - 1; i++)
                {
                    foreach (var lc in Path[i].GetCellsRightOfDirection(rowCnt, colCnt))
                    {
                        if (!lc.IsOnPath && !lc.IsRightOfPath)
                        {
                            lc.IsRightOfPath = true;
                        }
                    }
                }
                CellsRightOfPath = this.SelectMany(x => x).Where(x => x.IsRightOfPath).ToList();
            }


            //Fill Unassigned Cells
            private void FillUnassignedCells()
            {
                if (Path == null || Path.Count < 2) return;
                for (int r = 0; r < this.Count(); r++)
                {
                    for (int c = 0; c < this[0].Count(); c++)
                    {
                        var me = this[r][c];
                        if (me.IsRightOfPath) {
                            me.NeighborCells.ForEach(c => { if (c.IsUnassigned) c.IsRightOfPath = true; });
                        }
                        if (me.IsLeftOfPath)
                        {
                            me.NeighborCells.ForEach(c => { if (c.IsUnassigned) c.IsLeftOfPath = true; });
                        }
                    }
                }
                for (int r = this.Count() - 1; r >= 0; r--)
                {
                    for (int c = this[0].Count() - 1; c >= 0; c--)
                    {
                        var me = this[r][c];
                        if (me.IsRightOfPath)
                        {
                            me.NeighborCells.ForEach(c => { if (c.IsUnassigned) c.IsRightOfPath = true; });
                        }
                        if (me.IsLeftOfPath)
                        {
                            me.NeighborCells.ForEach(c => { if (c.IsUnassigned) c.IsLeftOfPath = true; });
                        }
                    }
                }

            }

        }


        static void day10()
        {
            var map = new c10Map(d10_data);

            //follow pipe
            map.BuildPath();

            Console.WriteLine($"map size=: rows: {map.rowCnt}: columns: {map.colCnt}: cells: {map.cellCnt}");
            Console.WriteLine($"cells in path= {map.Path.Count}");
            Console.WriteLine($"\n\n");


            Console.WriteLine($"Answer1: steps={map.Path.Count}: furthest(steps/2)={map.Path.Count / 2}");

            Console.WriteLine($"\n\nPart2....:");
            Console.WriteLine($"    -CellsLeftOfPath: {map.CellsLeftOfPath.Count()}: After Fill: {map.SelectMany(x=>x).Count(x => x.IsLeftOfPath)}");
            Console.WriteLine($"    -CellsRightOfPath: {map.CellsRightOfPath.Count()}: After Fill: {map.SelectMany(x => x).Count(x => x.IsRightOfPath)}");


            //Print Map
            Console.WriteLine($"\n\n");
            foreach (var r in map) 
            {
                foreach (var c in r)
                    Console.Write(
                          c.IsOnPath?c.s
                        : c.IsLeftOfPath ? 'I'
                        : c.IsRightOfPath ? 'O'
                        : '.');
                Console.Write("\n");
            }


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
        7-F7-
        .FJ|7
        SJLL7
        |F--J
        LJ.LJ
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
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
        .|IIIII||II777|.
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
