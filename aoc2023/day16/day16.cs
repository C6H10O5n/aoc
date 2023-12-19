using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Dynamic;
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
        class c16Coord
        {
            public enum emDir {N,S,E,W,X }
            Dictionary<emDir,char> dir2char = new Dictionary<emDir, char> { { emDir.N,'^'}, { emDir.S, 'v' }, { emDir.E, '>' }, { emDir.W, '<' }, { emDir.X, '?' } };

            public c16Coord(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }

            public c16Coord Clone()
            {
                return new c16Coord(s, r, c)
                {
                    ParentMap = this.ParentMap,
                    ParentRow = this.ParentRow,
                    ParentCol = this.ParentCol,
                };
            }
                        
            public int LightCount { get; set; }
            public emDir lastLightDir { get; set; } = emDir.X;
            public char LastLightDirChar => dir2char[lastLightDir];
            public bool MoveDirectionIsDifferent(c16Coord pc) =>lastLightDir != GetNextCellWhenComingFrom(pc).dr;

            public List<c16Coord> MoveLight(c16Coord fc, emDir iDir)
            {

                //Init move to cell and direction
                var tcd =
                      fc == null && iDir==emDir.X ? (tc:CellEast,dr:emDir.E) //default start direction
                    : fc == null && iDir == emDir.N ? (tc: CellNorth, dr: emDir.N) //default start direction N
                    : fc == null && iDir == emDir.N ? (tc: CellSouth, dr: emDir.S) //default start direction S
                    : fc == null && iDir == emDir.N ? (tc: CellEast, dr: emDir.E) //default start direction E
                    : fc == null && iDir == emDir.N ? (tc: CellWest, dr: emDir.W) //default start direction W
                    : GetNextCellWhenComingFrom(fc);
                
                //Increment light count for current cell
                this.LightCount++;
                this.lastLightDir = tcd.dr;
                
                
                var moveTo = new List<c16Coord>();

                if (s == '.')
                {
                    moveTo.Add(tcd.tc);
                }
                else if (s == '|')
                {
                    if (tcd.dr == emDir.N || tcd.dr == emDir.S)
                    {
                        moveTo.Add(tcd.tc);
                    }
                    else
                    {
                        lastLightDir = emDir.X;
                        if (CellNorth != null) moveTo.Add(CellNorth);
                        if (CellSouth != null) moveTo.Add(CellSouth);
                    }
                }
                else if (s == '-')
                {
                    if (tcd.dr == emDir.E || tcd.dr == emDir.W)
                    {
                        moveTo.Add(tcd.tc);
                    }
                    else
                    {
                        lastLightDir = emDir.X;
                        if (CellEast != null) moveTo.Add(CellEast);
                        if (CellWest != null) moveTo.Add(CellWest);
                    }
                }
                else if (s == '/')
                {
                    lastLightDir = emDir.X;
                    switch (tcd.dr)
                    {
                        case emDir.N: if (CellEast != null) moveTo.Add(CellEast); break;
                        case emDir.S: if (CellWest != null) moveTo.Add(CellWest); break;
                        case emDir.E: if (CellNorth != null) moveTo.Add(CellNorth); break;
                        case emDir.W: if (CellSouth != null) moveTo.Add(CellSouth); break;
                    }
                }
                else if (s == '\\')
                {
                    lastLightDir = emDir.X;
                    switch (tcd.dr)
                    {
                        case emDir.N: if (CellWest != null) moveTo.Add(CellWest); break;
                        case emDir.S: if (CellEast != null) moveTo.Add(CellEast); break;
                        case emDir.E: if (CellSouth != null) moveTo.Add(CellSouth); break;
                        case emDir.W: if (CellNorth != null) moveTo.Add(CellNorth); break;
                    }
                }

                return moveTo;
            }




            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }

            public c16Map ParentMap { get; set; }
            public c16CoordList ParentRow { get; set; }
            public c16CoordList ParentCol { get; set; }

            c16Coord CellNorth => NeighborCellsBase[0];
            c16Coord CellSouth => NeighborCellsBase[1];
            c16Coord CellEast => NeighborCellsBase[2];
            c16Coord CellWest => NeighborCellsBase[3];
            c16Coord CellNE => NeighborCellsBase[4];
            c16Coord CellNW => NeighborCellsBase[5];
            c16Coord CellSE => NeighborCellsBase[6];
            c16Coord CellSW => NeighborCellsBase[7];
            public List<c16Coord> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c16Coord> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c16Coord>();

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


            (c16Coord tc, emDir dr) GetNextCellWhenComingFrom(c16Coord fc)
            {
                if (fc == CellEast) return (CellWest,emDir.W);
                else if (fc == CellWest) return (CellEast,emDir.E);
                else if (fc == CellNorth) return (CellSouth,emDir.S);
                else if (fc == CellSouth) return (CellNorth, emDir.N);
                else return (null,emDir.X);
            }


            public bool Equals(c16Coord ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c && s == ic.s) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }

        class c16CoordList : List<c16Coord>
        {
            public c16CoordList(int idx, string s, c16Map map, c16CoordListCollection iCollection_rows_or_cols)
            {
                Id = idx;
                ParentMap = map;
                ParentCollection = iCollection_rows_or_cols;
                if (s != null)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        var xx = new c16Coord(s[i], idx, i)
                        {
                            ParentMap = map,
                            ParentRow = this
                        };
                        this.Add(xx);
                    }
                }
            }

            public int Id { get; private set; }
            public c16Map ParentMap { get; private set; }
            public c16CoordListCollection ParentCollection  { get; private set; }

            public int CountOfLight => this.Sum(x => x.LightCount > 0 ? 1 : 0);

            public c16CoordList Prev => ParentMap != null && this.Id > 0 ? ParentCollection[Id - 1] : null;
            public c16CoordList Next => ParentMap != null && this.Id < ParentCollection.Last().Id ? ParentCollection[Id + 1] : null;

            public string GetCollAsString => new string(this.Select(x => x.s).ToArray());


            public override string ToString() => $"[{GetCollAsString}]";
        }

        class c16CoordListCollection: List<c16CoordList>
        {
           public List<string> SummaryView => this.Select(x=>x.ToString()).ToList();
        }


        class c16Map
        {
            public c16Map(List<string> ls)
            {

                Rows = new c16CoordListCollection();
                Cols = new c16CoordListCollection();


                //Load Row data
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xr = new c16CoordList(i, ls[i], this, Rows);
                    Rows.Add(xr);

                    //Load col data
                    for(int j=0; j<xr.Count; j++)
                    {
                        if(i==0) Cols.Add(new c16CoordList(j, null, this, Cols));
                        xr[j].ParentCol = Cols[j];
                        Cols[j].Add(xr[j]);
                    }

                }


            }

            public c16CoordListCollection Rows {  get; private set; }
            public c16CoordListCollection Cols { get; private set; }


            public int MoveLightCellCount=> Cells.Sum(x => x.LightCount > 0 ? 1 : 0);
            void moveLight(c16Coord pc, c16Coord cc, c16Coord.emDir iDir = c16Coord.emDir.X)
            {
                if (cc != null && ( pc==null || cc.MoveDirectionIsDifferent(pc)))
                {
                    var ccl = cc.MoveLight(pc,iDir);
                    //PrintMap();
                    pc = cc;
                    moveLightList(pc, ccl);
                }
            }
            void moveLightList(c16Coord pc, List<c16Coord> ccl) { foreach (var cc in ccl) moveLight(pc,cc); }
            public int MoveLightBeam(c16Coord sc, c16Coord.emDir iDir)
            {
                //reset cells light beam counts
                foreach(var cc in Cells)
                {
                    cc.LightCount = 0;
                    cc.lastLightDir = c16Coord.emDir.X;
                }


                moveLight(null,sc, iDir);
                return MoveLightCellCount;
            }


            public int ColCnt => Cols.Count;
            public int RowCnt => Rows.Count;
            public int CellCnt => ColCnt * RowCnt;


            public IEnumerable<c16Coord> Cells => this.Rows.SelectMany(x => x); 


            public void PrintMap()
            {
                var m = this;

                Console.WriteLine($"\n\n");
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[0]);
                Console.WriteLine("|");
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[1]);
                Console.WriteLine("|");
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[2]);
                Console.WriteLine("|");
                Console.WriteLine(new string('-', m.Cols.Count + 8));
                foreach (var r in m.Rows)
                {
                    Console.Write(r.Id.ToString().PadLeft(3, ' ') + "|");
                    foreach (var c in r)
                    {
                        Console.Write($"{(
                              //c.LightCount > 0 ? '#'
                              c.s != '.' ? c.s
                            : c.LightCount == 1 ? c.LastLightDirChar
                            : c.LightCount > 1 ? c.LightCount.ToString().Last()
                            : c.s
                            )}");
                    }
                    Console.Write("|" + r.Id.ToString().PadLeft(2, ' ') + $"|{r.CountOfLight}" + "\n");
                }
                Console.WriteLine(new string('-', m.Cols.Count + 8));
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[2]);
                Console.WriteLine("|");
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[1]);
                Console.WriteLine("|");
                Console.Write($"   |"); foreach (var c in m.Cols) Console.Write($"{c.Id}".PadLeft(3, ' ')[0]);
                Console.WriteLine("|");
                Console.WriteLine($"CountLightCells: {m.Rows.Sum(r => r.CountOfLight)}");
                Console.WriteLine($"\n\n");
            }

        }


        static void day16()
        {
            var d = d16_data;

            var map = new c16Map(d);

            var ans1 = map.MoveLightBeam(map.Rows[0][0],c16Coord.emDir.E);

            Console.WriteLine($"Answer1: {ans1}");

            var ans2 = 0;
            foreach (var sc in map.Cols.First()) { var lcnt = map.MoveLightBeam(sc, c16Coord.emDir.E); if (lcnt > ans2) ans2 = lcnt; }
            foreach (var sc in map.Cols.Last()) { var lcnt = map.MoveLightBeam(sc, c16Coord.emDir.W); if (lcnt > ans2) ans2 = lcnt; }
            foreach (var sc in map.Rows.First()) { var lcnt = map.MoveLightBeam(sc, c16Coord.emDir.S); if (lcnt > ans2) ans2 = lcnt; }
            foreach (var sc in map.Rows.Last()) { var lcnt = map.MoveLightBeam(sc, c16Coord.emDir.N); if (lcnt > ans2) ans2 = lcnt; }


            Console.WriteLine($"Answer2: {ans2}");



            //Print Map
            map.PrintMap();


        }


        static List<string> d16_data0 =
        """
        .|...\....
        |.-.\.....
        .....|-...
        ........|.
        ..........
        .........\
        ..../.\\..
        .-.-/..|..
        .|....-|.\
        ..//.|....
        """
        .Split("\r\n").ToList();


    }
}
