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

        class c6Coord
        {
            public c6Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;

                if(IsGuard)
                    origionalGuardDir = isc;
            }


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public readonly char origionalGuardDir;
            public bool IsOrigionalGuardCell => "^v<>".Any(x => x == origionalGuardDir);

            public string WasVisitedDirection = "";
            public bool WasVisited=>WasVisitedDirection!="";
            public bool IsRepeatedMove;

            public c6Map ParentMap { get; set; }

            public bool IsGuard => "^v<>".Any(x=>x==s);
            public bool IsObstacle => s == '#' || s == 'X';

            public void Reset()
            {
                if (IsOrigionalGuardCell)
                    s = origionalGuardDir;
                else 
                    s = '.';

                WasVisitedDirection = "";
                IsRepeatedMove = false;

            }

            void rotateGuardDirectionClockWise() 
            {                
                if (IsGuard) { 
                  s = s == '^' ? '>'
                    : s == '>' ? 'v'
                    : s == 'v' ? '<'
                    : s == '<' ? '^'
                    : s ;
                }
            }

            public c6Coord GuardNextMoveCell()
            {
                if (!IsGuard) return null;

                WasVisitedDirection +=s;

                var mc =  s == '^' ? CellNorth
                        : s == 'v' ? CellSouth
                        : s == '>' ? CellEast
                        : s == '<' ? CellWest
                        : null;
                if (mc == null)
                {
                    return null;
                }
                else if(mc.IsObstacle)  
                {
                    rotateGuardDirectionClockWise();
                    return this;
                }
                else
                {                    
                    mc.s = this.s;
                    if(!mc.WasVisitedDirection.Any(x=>x==mc.s)) 
                        mc.WasVisitedDirection+=mc.s;
                    else
                        mc.IsRepeatedMove = true;

                    this.s = '.';

                    return mc;
                }

            }


            c6Coord CellNorth => NeighborCellsBase[0];
            c6Coord CellSouth => NeighborCellsBase[1];
            c6Coord CellEast => NeighborCellsBase[2];
            c6Coord CellWest => NeighborCellsBase[3];
            c6Coord CellNE => NeighborCellsBase[4];
            c6Coord CellNW => NeighborCellsBase[5];
            c6Coord CellSE => NeighborCellsBase[6];
            c6Coord CellSW => NeighborCellsBase[7];
            public List<c6Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c6Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c6Coord>();
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

            public bool Equals(c6Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s && WasVisitedDirection==ic.WasVisitedDirection) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }


        class c6Map : List<List<c6Coord>>
        {
            public c6Map(string[] ls)
            {
                for (int i = 0; i < ls.Count(); i++)
                {
                    var ca = ls[i].ToCharArray();
                    var xcl = new List<c6Coord>();
                    for (int j = 0; j < ca.Length; j++)
                    {
                        xcl.Add(new c6Coord(ca[j], i, j) { ParentMap = this});
                    }
                    this.Add(xcl);
                }

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;


            public void Reset()
            {
                foreach (var cell in Cells.Where(c => c.WasVisited))
                {
                    cell.Reset();
                }
            }

            public int MoveGaurdTillOffMap()
            {
                //PrintMap();
                var mg = GuardPos.GuardNextMoveCell();
                while (mg != null && !mg.IsRepeatedMove)
                {
                    mg = mg.GuardNextMoveCell();
                    //PrintMap();
                }

                if (mg != null && mg.IsRepeatedMove)
                    return -9;
                else
                    return Cells.Count(c => c.WasVisited);
            }

            c6Coord GuardPos => Cells.Where(x => x.IsGuard).FirstOrDefault();
            c6Coord GuardPosOrigional => Cells.Where(x => x.IsOrigionalGuardCell).FirstOrDefault();


            public IEnumerable<c6Coord> Cells => this.SelectMany(x => x);

            public void PrintMap()
            {
                Console.WriteLine($"\n\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsOrigionalGuardCell ? '@'
                        : c.WasVisited ? 'O'
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }

        }



        static void day6()
        {           
            Console.WriteLine($"Answer1: {day6LogicPart1()}");
            Console.WriteLine($"Answer2: {day6LogicPart2()}");
        }
                

        static int day6LogicPart1() 
        {
            var map = new c6Map(d6_data);

            return map.MoveGaurdTillOffMap();
        }

        static int day6LogicPart2()
        {
            var cnt = 0;
            var map = new c6Map(d6_data);
            //map.PrintMap();
            map.MoveGaurdTillOffMap();
            var chkNewObs = map.Cells.Where(c=>c.WasVisited && !c.IsOrigionalGuardCell).ToList();
            //map.PrintMap();
            for(int i = 0; i<chkNewObs.Count ; i++)
            {
                var cell = chkNewObs[i];
                map.Reset();
                //map.PrintMap();

                //test new obstacle
                cell.s = 'X';

                var mx = map.MoveGaurdTillOffMap();
                //map.PrintMap(); 
                if (mx == -9)
                {
                    cnt++;
                    //map.PrintMap();
                }

                cell.s = '.';
            }

            return cnt;
        }

        static string[] d6_data0 =
        """        
        ....#.....
        .........#
        ..........
        ..#.......
        .......#..
        ..........
        .#..^.....
        ........#.
        #.........
        ......#...
        """.Split(Environment.NewLine);

    }
}
