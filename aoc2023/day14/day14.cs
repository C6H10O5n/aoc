using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
        class c14Coord
        {
            public c14Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
                
                this.s_orig = s;
            }

            public c14Coord Clone()
            {
                return new c14Coord(s, r, c)
                {
                    ParentMap = this.ParentMap,
                    ParentRow = this.ParentRow,
                    ParentCol = this.ParentCol,
                };
            }

            public void SwapSymbols(c14Coord ic)
            {
                var ics = ic.s;


                ic.s = s;

                s = ics;
            }
                

            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public char s_orig { get; private set; }

            public c14Map ParentMap { get; set; }
            public c14CoordList ParentRow { get; set; }
            public c14CoordList ParentCol { get; set; }

            c14Coord CellNorth => NeighborCellsBase[0];
            c14Coord CellSouth => NeighborCellsBase[1];
            c14Coord CellEast => NeighborCellsBase[2];
            c14Coord CellWest => NeighborCellsBase[3];
            c14Coord CellNE => NeighborCellsBase[4];
            c14Coord CellNW => NeighborCellsBase[5];
            c14Coord CellSE => NeighborCellsBase[6];
            c14Coord CellSW => NeighborCellsBase[7];
            public List<c14Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c14Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c14Coord>();

                        var rc = ParentMap.RowCnt - 1;
                        var cc = ParentMap.ColCnt - 1;

                        if (r > 0) nc.Add(ParentRow.Prev[c + 0]); else nc.Add(null);//N
                        if (r < rc) nc.Add(ParentRow.Next[c + 0]); else nc.Add(null);//S
                        if (c < cc) nc.Add(ParentRow[c + 1]); else nc.Add(null);//E
                        if (c > 0) nc.Add(ParentRow[c - 1]); else nc.Add(null);//W


                        if (r > 0 && c < cc) nc.Add(ParentRow.Prev[c + 1]); else nc.Add(null);//NE
                        if (r > 0 && c > 0) nc.Add(ParentRow.Prev[c - 1]); else nc.Add(null);//NW
                        if (r < rc && c < cc) nc.Add(ParentRow.Next[c + 1]); else nc.Add(null);//SE
                        if (r < rc && c > 0) nc.Add(ParentRow.Next[c - 1]); else nc.Add(null);//SW

                        return nc;
                    }
                }
            }



            public bool Equals(c14Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }

        class c14CoordList : List<c14Coord>
        {
            public c14CoordList(int idx, string s, c14Map map, c14CoordListCollection iCollection_rows_or_cols)
            {
                Id = idx;
                ParentMap = map;
                ParentCollection = iCollection_rows_or_cols;
                if (s != null)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        var xx = new c14Coord(s[i], idx, i)
                        {
                            ParentMap = map,
                            ParentRow = this
                        };
                        this.Add(xx);
                    }
                }
            }

            public int Id { get; private set; }
            public c14Map ParentMap { get; private set; }
            public c14CoordListCollection ParentCollection  { get; private set; }

            public int CountOs => this.Sum(x => x.s == 'O' ? 1 : 0);
            public int CountOsTimesIndexInverse => CountOs * (ParentCollection.Count - Id);

        public c14CoordList Prev => ParentMap != null && this.Id > 0 ? ParentCollection[Id - 1] : null;
            public c14CoordList Next => ParentMap != null && this.Id < ParentCollection.Last().Id ? ParentMap.Cols[Id + 1] : null;

            public string GetCollAsString => new string(this.Select(x => x.s).ToArray());


            public override string ToString() => $"[{GetCollAsString}]";
        }

        class c14CoordListCollection: List<c14CoordList>
        {
           public List<string> SummaryView => this.Select(x=>x.ToString()).ToList();
        }


        class c14Map
        {
            public c14Map(List<string> ls, bool findSmudge = false)
            {

                Rows = new c14CoordListCollection();
                Cols = new c14CoordListCollection();


                //Load Row data
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xr = new c14CoordList(i, ls[i], this, Rows);
                    Rows.Add(xr);

                    //Load col data
                    for(int j=0; j<xr.Count; j++)
                    {
                        if(i==0) Cols.Add(new c14CoordList(j, null, this, Cols));
                        xr[j].ParentCol = Cols[j];
                        Cols[j].Add(xr[j]);
                    }

                }


            }

            public c14CoordListCollection Rows {  get; private set; }
            public c14CoordListCollection Cols { get; private set; }


            public int ColCnt => Cols.Count;
            public int RowCnt => Rows.Count;
            public int CellCnt => ColCnt * RowCnt;


            public void TiltMapN() => TiltMapNE(Cols);
            public void TiltMapW() => TiltMapNE(Rows);
            public void TiltMapS() => TiltMapSW(Cols);
            public void TiltMapE() => TiltMapSW(Rows);


            public void Cycle(int iter1 = 1 )
            {
                for (int i=0; i<iter1; i++)
                {
                    TiltMapN();
                    TiltMapW();
                    TiltMapS();
                    TiltMapE();
                }
            }

            void TiltMapNE(c14CoordListCollection ccx)
            {
                for(int i=0; i< ccx.Count; i++)
                {
                    var dc = ccx[i];
                    var lx0 = -1;
                    for(int j=0; j < dc.Count; j++)
                    {
                        if (dc[j].s == '.')
                        {
                            int k = j;
                            while (k < Cols.Count-1 && dc[k].s == '.') k++;
                            if (dc[k].s == 'O') dc[j].SwapSymbols(dc[k]); //move to base
                            else if (dc[k].s == '#') k = k; //do nothing
                            else j = k; //end of col reached
                        }
                    }
                }
            }

            void TiltMapSW(c14CoordListCollection ccx)
            {
                for (int i = 0; i < ccx.Count; i++)
                {
                    var dc = ccx[i];
                    for (int j = dc.Count - 1; j >= 0; j--)
                    {
                        if (dc[j].s == '.')
                        {
                            int k = j;
                            while (k > 0 && dc[k].s == '.') k--;
                            if (dc[k].s == 'O') dc[j].SwapSymbols(dc[k]); //move to base
                            else if (dc[k].s == '#') k = k; //do nothing
                            else j = k; //end of col reached
                        }
                    }
                }
            }

            public IEnumerable<c14Coord> Cells => this.Rows.SelectMany(x => x); 


        }


        static void day14()
        {
            var d = d14_data;

            var map = new c14Map(d);

            //map.TiltMapN();

            Console.WriteLine($"Answer1: {map.Rows.Sum(r=>r.CountOsTimesIndexInverse)}");

            var ans2 = 0;
            var repeats = new Dictionary<string, int>();
            for (int i = 0; i < 1000000000; i++)
            {
                //map.Cycle();
                var wts = new List<string>();
                Console.Write($"Cycle: {i}");
                map.TiltMapN();
                wts.Add(map.Rows.Sum(r => r.CountOsTimesIndexInverse).ToString().PadLeft(4, ' '));
                map.TiltMapW();
                wts.Add(map.Rows.Sum(r => r.CountOsTimesIndexInverse).ToString().PadLeft(4, ' '));
                map.TiltMapS();
                wts.Add(map.Rows.Sum(r => r.CountOsTimesIndexInverse).ToString().PadLeft(4, ' '));
                map.TiltMapE();
                wts.Add(map.Rows.Sum(r => r.CountOsTimesIndexInverse).ToString().PadLeft(4, ' '));
                
                Console.WriteLine($"  >N:{wts[0]} W:{wts[1]} S:{wts[2]} E{wts[3]}");

                var xkey = string.Join(',', wts);
                if (repeats.ContainsKey(xkey))
                {
                    var remainingIderations = (999999999-i);
                    var cycleLength = i - repeats[xkey];
                    var cycIdx = remainingIderations % cycleLength;
                    map.Cycle(cycIdx);
                    ans2 = map.Rows.Sum(r => r.CountOsTimesIndexInverse);

                    Console.WriteLine($"cycLen = {cycleLength}; cycIdx={cycIdx}; ans2={ans2}");

                    break;
                }
                else {
                    repeats.Add(xkey,i);
                }

            }

            Console.WriteLine($"Answer2: {ans2}");



            //Print Map
            var m = map;
            Console.Write($"  |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(2, ' ')[0]);
            Console.WriteLine("|");
            Console.Write($"  |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(2, ' ')[1]);
            Console.WriteLine("|");
            Console.WriteLine(new string('-', m.Cols.Count + 6));
            foreach (var r in m.Rows)
            {
                Console.Write(r.Id.ToString().PadLeft(2, ' ') + "|");
                foreach (var c in r)
                    Console.Write($"{c.s}");
                Console.Write("|"+r.Id.ToString().PadLeft(2, ' ') + $"|{r.CountOsTimesIndexInverse}" + "\n");
            }
            Console.WriteLine(new string('-', m.Cols.Count + 6));
            Console.Write($"  |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(2, ' ')[1]);
            Console.WriteLine("|");
            Console.Write($"  |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(2, ' ')[0]);
            Console.WriteLine("|");
            Console.WriteLine($"CountOsTimesIndexInverse: {map.Rows.Sum(r=>r.CountOsTimesIndexInverse)}");
            Console.WriteLine($"\n\n");


        }


        static List<string> d14_data0 =
        """
        O....#....
        O.OO#....#
        .....##...
        OO.#O....O
        .O.....O#.
        O.#..O.#.#
        ..O..#O..O
        .......O..
        #....###..
        #OO..#....
        """
        .Split("\r\n").ToList();


    }
}
