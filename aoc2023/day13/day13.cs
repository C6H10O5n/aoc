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
        class c13Coord
        {            
            public c13Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }

            public c13Coord Clone()
            {
                return new c13Coord(s, r, c) { 
                    ParentMap = this.ParentMap,
                    ParentRow = this.ParentRow,
                    ParentCol = this.ParentCol,
                };
            }

            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public c13Map ParentMap { get; set; }
            public c13Row ParentRow { get; set; }
            public c13Column ParentCol { get; set; }
            public bool IsKeyColReflection => ParentCol.IsColKeyReflection;
            public bool IsKeyRowReflection => ParentRow.IsKeyRowReflection;
            public bool IsColReflection => ParentCol.IsColReflection;
            public bool IsRowReflection => ParentRow.IsRowReflection;


            c13Coord CellNorth => NeighborCellsBase[0];
            c13Coord CellSouth => NeighborCellsBase[1];
            c13Coord CellEast  => NeighborCellsBase[2];
            c13Coord CellWest  => NeighborCellsBase[3];
            c13Coord CellNE    => NeighborCellsBase[4];
            c13Coord CellNW    => NeighborCellsBase[5];
            c13Coord CellSE    => NeighborCellsBase[6];
            c13Coord CellSW    => NeighborCellsBase[7];
            public List<c13Coord> NeighborCells => NeighborCellsBase.Where((x,index) => x != null && index < 4).ToList();
            public List<c13Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c13Coord>();

                        var rc = ParentMap.RowCnt - 1;
                        var cc = ParentMap.ColCnt - 1;

                        if (r >  0) nc.Add(ParentRow.Prev[c + 0]); else nc.Add(null);//N
                        if (r < rc) nc.Add(ParentRow.Next[c + 0]); else nc.Add(null);//S
                        if (c < cc) nc.Add(ParentRow[c + 1]); else nc.Add(null);//E
                        if (c >  0) nc.Add(ParentRow[c - 1]); else nc.Add(null);//W


                        if (r >  0 && c < cc) nc.Add(ParentRow.Prev[c + 1]); else nc.Add(null);//NE
                        if (r >  0 && c >  0) nc.Add(ParentRow.Prev[c - 1]); else nc.Add(null);//NW
                        if (r < rc && c < cc) nc.Add(ParentRow.Next[c + 1]); else nc.Add(null);//SE
                        if (r < rc && c >  0) nc.Add(ParentRow.Next[c - 1]); else nc.Add(null);//SW

                        return nc;
                    }
                }
            }



            public bool Equals(c13Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }

        class c13Row : List<c13Coord>
        {
            public c13Row(string s, c13Map map) 
            {
                ParentMap = map;
                rowId = map.Rows.Count;
                for (int i = 0; i < s.Length; i++)
                {
                    var xc = new c13Coord(s[i], rowId, i)
                    {
                        ParentMap = map,
                        ParentRow = this
                    };
                    this.Add(xc);
                }
            }

            public int rowId { get; private set; }
            public c13Map ParentMap { get; private set; }
            public bool IsKeyRowReflection => RefectionId == 1 || RefectionId == -1;
            public bool IsRowReflection => RefectionId != 0;
            public int RefectionId { get; set; }

            public c13Row Prev => ParentMap != null && this.rowId > 0 ? ParentMap.Rows[rowId - 1] : null;
            public c13Row Next => ParentMap != null && this.rowId < ParentMap.Rows.Last().rowId ? ParentMap.Rows[rowId + 1] : null;

            public string GetRowAsString => new string(this.Select(x => x.s).ToArray());


            public override string ToString() => $"[{GetRowAsString}] [{RefectionId}]";
        }

        class c13Column : List<c13Coord>
        {
            public c13Column(int colIdx, c13Map map)
            {
                colId = colIdx;
                ParentMap = map;
            }

            public int colId { get; private set; }
            public c13Map ParentMap { get; private set; }
            public List<c13Column> ParentCols => ParentMap.Cols;
            public bool IsColKeyReflection => RefectionId == 1 || RefectionId == -1;
            public bool IsColReflection => RefectionId != 0;
            public int RefectionId { get; set; }

            public c13Column Prev => ParentMap != null && this.colId > 0 ? ParentMap.Cols[colId - 1] : null;
            public c13Column Next => ParentMap != null && this.colId < ParentMap.Cols.Last().colId ? ParentMap.Cols[colId + 1] : null;

            public string GetColAsString => new string(this.Select(x => x.s).ToArray());


            public override string ToString() => $"[{GetColAsString}] [{RefectionId}]";
        }

        class c13RowList: List<c13Row>
        {
            public bool CheckReflect(int ridx, bool findSmudge = false)
            {
                if (ridx < 1) return false;
                var ru = this[ridx - 1];
                var rl = this[ridx];
                var xcnt = 0;
                if (ru == null || rl == null) return false;
                while (ru != null && rl != null)
                {                    
                    if (ru.GetRowAsString != rl.GetRowAsString)
                    {
                        if (!findSmudge)
                        {
                            return false;
                        }
                        else
                        {
                            var cd = ru.Where((x, i) => x.s != rl[i].s).Count();
                            if (cd > 1)
                                return false;
                            else if (cd == 1)
                                xcnt++;
                        }
                    }
                    ru = ru.Prev;
                    rl = rl.Next;
                }

                if (!findSmudge) return true;
                else return xcnt == 1;
            }


            public void SetUnusedReflect()
            {
                var rui = this.Where(r => r.RefectionId == 1).FirstOrDefault();
                var rli = this.Where(r => r.RefectionId == -1).FirstOrDefault();
                if (rui != null && rli != null)
                {
                    int refId = 1;
                    var ru = rui.Prev;
                    var rl = rli.Next;
                    while (ru != null && rl != null)
                    {
                        if (ru.GetRowAsString == rl.GetRowAsString)
                        {
                            refId++;
                            ru.RefectionId = refId;
                            rl.RefectionId = -refId;
                        }
                        else
                        {
                            break;
                        }
                        ru = ru.Prev;
                        rl = rl.Next;
                    }
                }
            }
            public List<string> SummaryView => this.Select(r=>r.ToString()).ToList();
        }
        class c13ColumnList : List<c13Column>
        {
            public bool CheckReflect(int cidx, bool findSmudge = false)
            {
                if (cidx < 1) return false;
                var ru = this[cidx - 1];
                var rl = this[cidx];
                var xcnt = 0;
                if (ru == null || rl == null) return false;
                while (ru != null && rl != null)
                {
                    if (ru.GetColAsString != rl.GetColAsString)
                    {
                        if (!findSmudge)
                        {
                            return false;
                        }
                        else
                        {
                            var cd = ru.Where((x, i) => x.s != rl[i].s).Count();
                            if (cd > 1)
                                return false;
                            else if (cd == 1)
                                xcnt++;
                        }
                    }
                    ru = ru.Prev;
                    rl = rl.Next;
                }

                if (!findSmudge) return true;
                else return xcnt == 1;
            }

            public void SetUnusedReflect()
            {
                var rui = this.Where(c => c.RefectionId == 1).FirstOrDefault();
                var rli = this.Where(c => c.RefectionId == -1).FirstOrDefault();
                if (rui != null && rli != null)
                {
                    int refId = 1;
                    var ru = rui.Prev;
                    var rl = rli.Next;
                    while (ru != null && rl != null)
                    {
                        if (ru.GetColAsString == rl.GetColAsString)
                        {
                            refId++;
                            ru.RefectionId = refId;
                            rl.RefectionId = -refId;
                        }
                        else
                        {
                            break;
                        }
                        ru = ru.Prev;
                        rl = rl.Next;
                    }
                }
            }

            public List<string> SummaryView => this.Select(c => c.ToString()).ToList();
        }


        class c13Map
        {
            public c13Map(List<string> ls, bool findSmudge = false)
            {

                Rows = new c13RowList();
                Cols = new c13ColumnList();

                var refRowCnt = new List<int>();
                var refColCnt = new List<int>();

                //Load Row data
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xr = new c13Row(ls[i], this);
                    Rows.Add(xr);

                    //flag potential reflect rows
                    if (xr.Prev != null)
                    {
                        if (xr.GetRowAsString == xr.Prev.GetRowAsString)
                        {
                            refRowCnt.Add(i);
                        }
                    }

                    //Load col data
                    for(int j=0; j<xr.Count; j++)
                    {
                        if(i==0) Cols.Add(new c13Column(j, this));
                        xr[j].ParentCol = Cols[j];
                        Cols[j].Add(xr[j]);
                    }

                }


                //flag potential refect cols
                for (int j = 0; j < Cols.Count; j++)
                {
                    var xc = Cols[j];
                    if (xc.Prev != null && xc.GetColAsString == xc.Prev.GetColAsString)
                        refColCnt.Add(j);
                }


                //Set key reflection row closest to center as Key reflection row

                if (findSmudge)
                {
                    foreach (var ridx in Rows.Select(r => r.rowId))
                    {
                        if (Rows.CheckReflect(ridx, findSmudge))
                        {
                            Rows[ridx].RefectionId = -1;
                            Rows[ridx - 1].RefectionId = 1;
                            break;
                        }
                    }
                }
                else
                {
                    if (refRowCnt.Count > 0)
                    {
                        foreach (var ridx in refRowCnt)
                        {
                            if (Rows.CheckReflect(ridx))
                            {
                                Rows[ridx].RefectionId = -1;
                                Rows[ridx - 1].RefectionId = 1;
                                break;
                            }
                        }
                    }
                }
                if(!findSmudge) Rows.SetUnusedReflect();


                //Set key reflection col closest to center as Key reflection col
                if (findSmudge)
                {
                    foreach (var cidx in Cols.Select(c => c.colId))
                    {
                        if (Cols.CheckReflect(cidx, findSmudge))
                        {
                            Cols[cidx].RefectionId = -1;
                            Cols[cidx - 1].RefectionId = 1;
                            break;
                        }
                    }
                }
                else
                {
                    if (refColCnt.Count > 0)
                    {
                        foreach (var cidx in refColCnt)
                        {
                            if (Cols.CheckReflect(cidx))
                            {
                                Cols[cidx].RefectionId = -1;
                                Cols[cidx - 1].RefectionId = 1;
                                break;
                            }
                        }
                    }
                }
                if (!findSmudge) Cols.SetUnusedReflect();


            }

            public c13RowList Rows {  get; private set; }
            public c13ColumnList Cols { get; private set; }


            public int ColCnt => Cols.Count;
            public int RowCnt => Rows.Count;
            public int CellCnt => ColCnt * RowCnt;

            public bool hasReflectRows => Rows.Any(x => x.RefectionId > 0);
            public bool hasReflectCols => Cols.Any(x => x.RefectionId > 0);

            public int UnusedReflectCols => hasReflectCols ? Cols.Count(x => x.RefectionId == 0): 9999;
            public int UnusedReflectRows => hasReflectRows ?  Rows.Count(x => x.RefectionId == 0): 9999;

            public bool DomReflectionIsRows => hasReflectRows;//UnusedReflectRows < UnusedReflectCols;


            public int ColsAboveReflection => DomReflectionIsRows ? 0 : Cols.Where(c => c.RefectionId == 1).FirstOrDefault()?.colId + 1 ?? 0;
            public int RowsAboveReflection => DomReflectionIsRows ? Rows.Where(r => r.RefectionId == 1).FirstOrDefault()?.rowId + 1 ?? 0 : 0;


            public IEnumerable<c13Coord> Cells => this.Rows.SelectMany(x => x); 


        }


        static void day13()
        {
            var d = d13_data;

            var maps = new List<c13Map>();

            for (var i=0; i< d.Count; i++)
            {
                //Console.WriteLine($"{i}");
                maps.Add(new c13Map(d[i]));
            }

            Console.WriteLine($"Answer1: {maps.Sum(m=>m.ColsAboveReflection) + maps.Sum(m=>m.RowsAboveReflection)*100}");


            var maps2 = new List<c13Map>();

            for (var i = 0; i < d.Count; i++)
            {
                //Console.WriteLine($"{i}");
                maps2.Add(new c13Map(d[i],true));
            }


            Console.WriteLine($"Answer2: {maps2.Sum(m => m.ColsAboveReflection) + maps2.Sum(m => m.RowsAboveReflection) * 100}");



            //Print Map
            Console.WriteLine($"\n\n");
            foreach (var m in maps2)
            {
                Console.WriteLine($"{m.RowsAboveReflection} : {m.ColsAboveReflection}");
                Console.Write($"  |  |"); foreach (var c in m.Cols) Console.Write($"{Math.Abs(c.RefectionId)}".PadLeft(2, ' '));
                Console.WriteLine("");
                Console.Write($"  |  |"); foreach (var c in m.Cols) Console.Write($"{Math.Abs(c.colId)}".PadLeft(2, ' '));
                Console.WriteLine("");
                Console.WriteLine(new string('-', m.Cols.Count * 2 + 6));
                foreach (var r in m.Rows)
                {
                    Console.Write(r.rowId.ToString().PadLeft(2, ' ') + "|" + Math.Abs(r.RefectionId).ToString().PadLeft(2, ' ') + "|");
                    foreach (var c in r)
                        Console.Write($" {c.s}");
                    Console.Write("\n");
                }
                Console.WriteLine($"\n\n");
            }


        }


        static List<List<string>> d13_data0 =
        """
        #.##..##.
        ..#.##.#.
        ##......#
        ##......#
        ..#.##.#.
        ..##..##.
        #.#.##.#.

        #...##..#
        #....#..#
        ..##..###
        #####.##.
        #####.##.
        ..##..###
        #....#..#
        """
        .Split("\r\n\r\n").Select(x=>x.Split("\r\n").ToList()).ToList();

        static List<List<string>> d13_data0b =
        """
        .#.####..##
        ..##.#.##.#
        .#.##.#..#.
        .....#....#
        ##....#..#.
        ....#......
        ...##......
        ##....#..#.
        .....#....#
        """
        .Split("\r\n\r\n").Select(x=>x.Split("\r\n").ToList()).ToList();


    }
}
