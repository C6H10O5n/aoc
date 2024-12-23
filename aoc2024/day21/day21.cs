using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        class c21Button
        {
            public c21Button(char isc, int ir, int ic)
            {
                this.s = isc;
                this.r = ir;
                this.c = ic;
            }

            public c21Button(c21Button ic) { s = ic.s; r = ic.r; c = ic.c; ; ParentPad = ic.ParentPad; }
            public c21Button Clone() => new c21Button(s, r, c) { ParentPad = this.ParentPad };


            public char s { get; set; }
            public int r { get; set; }
            public int c { get; set; }
            public c21Pad ParentPad { get; set; }

            public bool IsA => s == 'A';
            public bool IsEmpty => s == '!';


            c21Button getButton(char ic) => ParentPad.GetButton(ic);

            List<string> pathPermute(string ipath, List<string> xpaths = null)
            {
                var pPaths = ipath.ToCharArray()
                    .Permute()
                    .Select(x => new string(x.ToArray()))
                    .Distinct()
                    .ToList();

                if (xpaths != null)
                {
                    pPaths = pPaths.Where(x => !xpaths.Any(y => x.StartsWith(y))).ToList();
                }
                pPaths = pPaths.OrderBy(x => x.Zip(x.Skip(1)).Count(x => x.First != x.Second)).ToList();

                var px = new List<string>();
                px.Add(pPaths[0]);
                if(pPaths.Count>1 && new string(pPaths[0].ToCharArray().Reverse().ToArray())==pPaths[1])
                    px.Add(pPaths[1]);

                return px.Select(x=>x+(xpaths==null?"":"A")).ToList();
            }

            void BuildPathsNumeric()
            {
                //789
                //456
                //123
                // 0A
                var xp = new List<string>();
                switch (s)
                {
                    case 'A':
                        xp = pathPermute("<<");
                        PathTo.Add(getButton('A'), new List<string> { "A" });
                        PathTo.Add(getButton('0'), pathPermute("<", xp));
                        PathTo.Add(getButton('1'), pathPermute("^<<", xp));
                        PathTo.Add(getButton('2'), pathPermute("^<", xp));
                        PathTo.Add(getButton('3'), pathPermute("^", xp));
                        PathTo.Add(getButton('4'), pathPermute("^^<<", xp));
                        PathTo.Add(getButton('5'), pathPermute("^^<", xp));
                        PathTo.Add(getButton('6'), pathPermute("^^", xp));
                        PathTo.Add(getButton('7'), pathPermute("^^^<<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^^^<", xp));
                        PathTo.Add(getButton('9'), pathPermute("^^^", xp));
                        break;
                    case '0':
                        xp = pathPermute("<");
                        PathTo.Add(getButton('A'), pathPermute(">", xp));
                        PathTo.Add(getButton('0'), new List<string> { "A" });
                        PathTo.Add(getButton('1'), pathPermute("^<", xp));
                        PathTo.Add(getButton('2'), pathPermute("^", xp));
                        PathTo.Add(getButton('3'), pathPermute("^>", xp));
                        PathTo.Add(getButton('4'), pathPermute("^^<", xp));
                        PathTo.Add(getButton('5'), pathPermute("^^", xp));
                        PathTo.Add(getButton('6'), pathPermute("^^>", xp));
                        PathTo.Add(getButton('7'), pathPermute("^^^<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^^^", xp));
                        PathTo.Add(getButton('9'), pathPermute("^^^>", xp));
                        break;
                    case '1':
                        xp = pathPermute("v");
                        PathTo.Add(getButton('A'), pathPermute("v>>", xp));
                        PathTo.Add(getButton('0'), pathPermute("v>", xp));
                        PathTo.Add(getButton('1'), new List<string> { "A" });
                        PathTo.Add(getButton('2'), pathPermute(">", xp));
                        PathTo.Add(getButton('3'), pathPermute(">>", xp));
                        PathTo.Add(getButton('4'), pathPermute("^", xp));
                        PathTo.Add(getButton('5'), pathPermute("^>", xp));
                        PathTo.Add(getButton('6'), pathPermute("^>>", xp));
                        PathTo.Add(getButton('7'), pathPermute("^^", xp));
                        PathTo.Add(getButton('8'), pathPermute("^^>", xp));
                        PathTo.Add(getButton('9'), pathPermute("^^>>", xp));
                        break;
                    case '2':
                        xp = pathPermute("v<");
                        PathTo.Add(getButton('A'), pathPermute("v>", xp));
                        PathTo.Add(getButton('0'), pathPermute("v", xp));
                        PathTo.Add(getButton('1'), pathPermute("<", xp));
                        PathTo.Add(getButton('2'), new List<string> { "A" });
                        PathTo.Add(getButton('3'), pathPermute(">", xp));
                        PathTo.Add(getButton('4'), pathPermute("^", xp));
                        PathTo.Add(getButton('5'), pathPermute("^<", xp));
                        PathTo.Add(getButton('6'), pathPermute("^", xp));
                        PathTo.Add(getButton('7'), pathPermute("^^<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^^", xp));
                        PathTo.Add(getButton('9'), pathPermute("^^>", xp));
                        break;
                    case '3':
                        xp = pathPermute("v<<");
                        PathTo.Add(getButton('A'), pathPermute("v", xp));
                        PathTo.Add(getButton('0'), pathPermute("v<", xp));
                        PathTo.Add(getButton('1'), pathPermute("<<", xp));
                        PathTo.Add(getButton('2'), pathPermute("<", xp));
                        PathTo.Add(getButton('3'), new List<string> { "A" });
                        PathTo.Add(getButton('4'), pathPermute("^<<", xp));
                        PathTo.Add(getButton('5'), pathPermute("^<", xp));
                        PathTo.Add(getButton('6'), pathPermute("^", xp));
                        PathTo.Add(getButton('7'), pathPermute("^^<<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^^<", xp));
                        PathTo.Add(getButton('9'), pathPermute("^^", xp));
                        break;
                    case '4':
                        xp = pathPermute("vv");
                        PathTo.Add(getButton('A'), pathPermute("vv>>", xp));
                        PathTo.Add(getButton('0'), pathPermute("vv>", xp));
                        PathTo.Add(getButton('1'), pathPermute("v", xp));
                        PathTo.Add(getButton('2'), pathPermute("v>", xp));
                        PathTo.Add(getButton('3'), pathPermute("v>>", xp));
                        PathTo.Add(getButton('4'), new List<string> { "A" });
                        PathTo.Add(getButton('5'), pathPermute(">", xp));
                        PathTo.Add(getButton('6'), pathPermute(">>", xp));
                        PathTo.Add(getButton('7'), pathPermute("^", xp));
                        PathTo.Add(getButton('8'), pathPermute("^>", xp));
                        PathTo.Add(getButton('9'), pathPermute("^>>", xp));
                        break;
                    case '5':
                        xp = pathPermute("vv<");
                        PathTo.Add(getButton('A'), pathPermute("vv>", xp));
                        PathTo.Add(getButton('0'), pathPermute("vv", xp));
                        PathTo.Add(getButton('1'), pathPermute("v<", xp));
                        PathTo.Add(getButton('2'), pathPermute("v", xp));
                        PathTo.Add(getButton('3'), pathPermute("v>", xp));
                        PathTo.Add(getButton('4'), pathPermute("<", xp));
                        PathTo.Add(getButton('5'), new List<string> { "A" });
                        PathTo.Add(getButton('6'), pathPermute(">", xp));
                        PathTo.Add(getButton('7'), pathPermute("^<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^", xp));
                        PathTo.Add(getButton('9'), pathPermute("^>", xp));
                        break;
                    case '6':
                        xp = pathPermute("vv<<");
                        PathTo.Add(getButton('A'), pathPermute("vv", xp));
                        PathTo.Add(getButton('0'), pathPermute("vv<", xp));
                        PathTo.Add(getButton('1'), pathPermute("v<<", xp));
                        PathTo.Add(getButton('2'), pathPermute("v<", xp));
                        PathTo.Add(getButton('3'), pathPermute("v", xp));
                        PathTo.Add(getButton('4'), pathPermute("<<", xp));
                        PathTo.Add(getButton('5'), pathPermute("<", xp));
                        PathTo.Add(getButton('6'), new List<string> { "A" });
                        PathTo.Add(getButton('7'), pathPermute("^<<", xp));
                        PathTo.Add(getButton('8'), pathPermute("^<", xp));
                        PathTo.Add(getButton('9'), pathPermute("^", xp));
                        break;
                    case '7':
                        xp = pathPermute("vvv");
                        PathTo.Add(getButton('A'), pathPermute("vvv>>", xp));
                        PathTo.Add(getButton('0'), pathPermute("vvv>", xp));
                        PathTo.Add(getButton('1'), pathPermute("vv", xp));
                        PathTo.Add(getButton('2'), pathPermute("vv>", xp));
                        PathTo.Add(getButton('3'), pathPermute("vv>>", xp));
                        PathTo.Add(getButton('4'), pathPermute("v", xp));
                        PathTo.Add(getButton('5'), pathPermute("v>", xp));
                        PathTo.Add(getButton('6'), pathPermute("v>>", xp));
                        PathTo.Add(getButton('7'), new List<string> { "A" });
                        PathTo.Add(getButton('8'), pathPermute(">", xp));
                        PathTo.Add(getButton('9'), pathPermute(">>", xp));
                        break;
                    case '8':
                        xp = pathPermute("vvv<");
                        PathTo.Add(getButton('A'), pathPermute("vvv>", xp));
                        PathTo.Add(getButton('0'), pathPermute("vvv", xp));
                        PathTo.Add(getButton('1'), pathPermute("vv<", xp));
                        PathTo.Add(getButton('2'), pathPermute("vv", xp));
                        PathTo.Add(getButton('3'), pathPermute("vv>", xp));
                        PathTo.Add(getButton('4'), pathPermute("v<", xp));
                        PathTo.Add(getButton('5'), pathPermute("v", xp));
                        PathTo.Add(getButton('6'), pathPermute("v>", xp));
                        PathTo.Add(getButton('7'), pathPermute("<", xp));
                        PathTo.Add(getButton('8'), new List<string> { "A" });
                        PathTo.Add(getButton('9'), pathPermute(">", xp));
                        break;
                    case '9':
                        xp = pathPermute("vvv<<");
                        PathTo.Add(getButton('A'), pathPermute("vvv", xp));
                        PathTo.Add(getButton('0'), pathPermute("vvv<", xp));
                        PathTo.Add(getButton('1'), pathPermute("vv<<", xp));
                        PathTo.Add(getButton('2'), pathPermute("vv<", xp));
                        PathTo.Add(getButton('3'), pathPermute("vv", xp));
                        PathTo.Add(getButton('4'), pathPermute("v<<", xp));
                        PathTo.Add(getButton('5'), pathPermute("v<", xp));
                        PathTo.Add(getButton('6'), pathPermute("v", xp));
                        PathTo.Add(getButton('7'), pathPermute("<<", xp));
                        PathTo.Add(getButton('8'), pathPermute("<", xp));
                        PathTo.Add(getButton('9'), new List<string> { "A" });
                        break;
                }
            }
            void BuildPathsDirectional()
            {
                // ^A
                //<v>
                var xp = new List<string>();
                switch (s)
                {
                    case 'A':
                        xp = pathPermute("<<");
                        PathTo.Add(getButton('A'), new List<string> { "A" });
                        PathTo.Add(getButton('^'), pathPermute("<", xp));
                        PathTo.Add(getButton('<'), pathPermute("v<<", xp));
                        PathTo.Add(getButton('v'), pathPermute("v<", xp));
                        PathTo.Add(getButton('>'), pathPermute("v", xp));
                        break;
                    case '^':
                        xp = pathPermute("<");
                        PathTo.Add(getButton('A'), pathPermute(">", xp));
                        PathTo.Add(getButton('^'), new List<string> { "A" });
                        PathTo.Add(getButton('<'), pathPermute("v<", xp));
                        PathTo.Add(getButton('v'), pathPermute("v", xp));
                        PathTo.Add(getButton('>'), pathPermute("v>", xp));
                        break;
                    case '<':
                        xp = pathPermute("^");
                        PathTo.Add(getButton('A'), pathPermute("^>>", xp));
                        PathTo.Add(getButton('^'), pathPermute("^>", xp));
                        PathTo.Add(getButton('<'), new List<string> { "A" });
                        PathTo.Add(getButton('v'), pathPermute(">", xp));
                        PathTo.Add(getButton('>'), pathPermute(">>", xp));
                        break;
                    case 'v':
                        xp = pathPermute("^<");
                        PathTo.Add(getButton('A'), pathPermute("^>", xp));
                        PathTo.Add(getButton('^'), pathPermute("^", xp));
                        PathTo.Add(getButton('<'), pathPermute("<", xp));
                        PathTo.Add(getButton('v'), new List<string> { "A" });
                        PathTo.Add(getButton('>'), pathPermute(">", xp));
                        break;
                    case '>':
                        xp = pathPermute("^<<");
                        PathTo.Add(getButton('A'), pathPermute("^", xp));
                        PathTo.Add(getButton('^'), pathPermute("^<", xp));
                        PathTo.Add(getButton('<'), pathPermute("<<", xp));
                        PathTo.Add(getButton('v'), pathPermute("<", xp));
                        PathTo.Add(getButton('>'), new List<string> { "A" });
                        break;
                }

            }
            public void BuildPaths() { if (ParentPad.IsNumeric) BuildPathsNumeric(); else BuildPathsDirectional(); }
            public Dictionary<c21Button, List<string>> PathTo { get; private set; } = new Dictionary<c21Button, List<string>>();

            public char? GetPathDirectionTo(c21Button ic) =>
                  ic == CellNorth ? '^'
                : ic == CellSouth ? 'v'
                : ic == CellEast ? '>'
                : ic == CellWest ? '<'
                : null;


            #region Distance Logic
            public double CalcDistToCoordMahatten(c21Button ic)
            {
                int cMax, cMin, rMax, rMin;
                if (ic.c >= c) { cMax = ic.c; cMin = c; } else { cMax = c; cMin = ic.c; };
                if (ic.r >= r) { rMax = ic.r; rMin = r; } else { rMax = r; rMin = ic.r; };

                return (cMax - cMin) + (rMax - rMin);
            }
            public double CalcDistToCoordEculid(c21Button ic)
            {
                return Math.Sqrt((ic.c - c) ^ 2 + (ic.r - r) ^ 2) / Math.Sqrt(2.0);
            }
            #endregion


            #region NeighborLogic
            public c21Button CellNorth => NeighborCellsBase[0];
            public c21Button CellSouth => NeighborCellsBase[1];
            public c21Button CellEast => NeighborCellsBase[2];
            public c21Button CellWest => NeighborCellsBase[3];
            public c21Button CellNE => NeighborCellsBase[4];
            public c21Button CellNW => NeighborCellsBase[5];
            public c21Button CellSE => NeighborCellsBase[6];
            public c21Button CellSW => NeighborCellsBase[7];
            public List<c21Button> NeighborCells => NeighborCellsBase.Where((x, index) => x != null && index < 4).ToList();
            public List<c21Button> NeighborCellsNS => NeighborCellsBase.Where((x, index) => index < 2).ToList();
            public List<c21Button> NeighborCellsEW => NeighborCellsBase.Where((x, index) => index >= 2 && index < 4).ToList();
            public List<c21Button> NeighborCellsNSEW => NeighborCellsBase.Where((x, index) => index < 4).ToList();
            public List<c21Button> NeighborCellsBase
            {
                get
                {
                    {
                        var nc = new List<c21Button>();
                        var rc = ParentPad.rowCnt - 1;
                        var cc = ParentPad.colCnt - 1;
                        if (r > 0) nc.Add(ParentPad[r - 1][c + 0]); else nc.Add(null);//N
                        if (r < rc) nc.Add(ParentPad[r + 1][c + 0]); else nc.Add(null);//S
                        if (c < cc) nc.Add(ParentPad[r + 0][c + 1]); else nc.Add(null);//E
                        if (c > 0) nc.Add(ParentPad[r + 0][c - 1]); else nc.Add(null);//W


                        if (r > 0 && c < cc) nc.Add(ParentPad[r - 1][c + 0]); else nc.Add(null);//NE
                        if (r > 0 && c > 0) nc.Add(ParentPad[r - 1][c + 0]); else nc.Add(null);//NW
                        if (r < rc && c < cc) nc.Add(ParentPad[r + 1][c + 0]); else nc.Add(null);//SE
                        if (r < rc && c > 0) nc.Add(ParentPad[r + 1][c + 0]); else nc.Add(null);//SW

                        return nc;
                    }
                }
            }
            #endregion


            public bool Equals(c21Button ic)
            {
                if (ic is null) return false;
                if (r == ic.r && c == ic.c) return true;
                return false;
            }

            public override string ToString() => $"{s}[{r},{c}]";

        }

        class c21Pad : List<List<c21Button?>>
        {
            public c21Pad(string[] ls)
            {
                //build
                for (int i = 0; i < ls.Count(); i++)
                {
                    var xcl = new List<c21Button>();
                    var ca = ls[i];
                    for (int j = 0; j < ca.Length; j++)
                    {
                        var nc = new c21Button(ca[j], i, j) { ParentPad = this };
                        xcl.Add(nc);
                    }
                    this.Add(xcl);
                }

                //init button paths
                foreach (var bn in this.Buttons)
                    bn.BuildPaths();

                //init start button
                ActiveButton = AButton;

            }

            public int colCnt => this[0].Count;
            public int rowCnt => Count;
            public int cellCnt => colCnt * rowCnt;

            public c21Button AButton => Buttons.First(x => x.IsA);
            public c21Button GetButton(char ic) => Buttons.Where(x => x.s == ic).First();
            public c21Button ActiveButton { get; set; }

            public IEnumerable<c21Button> Buttons => this.SelectMany(x => x);
            public bool IsNumeric => this[0][0].s == '7';
            public bool IsDirection => this[0][0].s == '!';
            public c21Pad ParentPad { get; set; }
            public c21Pad ChildPad { get; set; }


            public Dictionary<string, long> ButtonCache = new Dictionary<string, long>();

            public long PressNumericButtonLength(string iCode)
            {

                long hx(c21Pad ip, string ic)
                {
                    
                    if (ip == null)
                        return ic.Length;

                    if (ip.ButtonCache.ContainsKey(ic))
                        return ip.ButtonCache[ic];

                    var ax = 0L;
                    Debug.Assert(ip.ActiveButton == ip.AButton);
                    foreach (var c in ic)
                    {
                        ax += ip.ActiveButton.PathTo[ip.GetButton(c)].Select(x => hx(ip.ParentPad, x)).Min();
                        ip.ActiveButton = ip.GetButton(c);
                    }

                    //Console.WriteLine(ax);
                    ip.ButtonCache.Add(ic,ax);
                    return ax;
                }

                return hx(this, iCode);
            }
            public string PressNumericButton(string  iCode)
            {

                string hx(c21Pad ip, string ic)
                {
                    if (ip == null)
                    {
                        //Console.WriteLine(ic);
                        return ic;
                    }
                    var ax = "";
                    Debug.Assert(ip.ActiveButton == ip.AButton);
                    foreach (var c in ic)
                    {
                        ax += ip.ActiveButton.PathTo[ip.GetButton(c)].Select(x => hx(ip.ParentPad, x)).MinBy(x => x.Length);
                        //ax += ip.ActiveButton.PathTo[ip.GetButton(c)].Select(x => hx(ip.ParentPad, x)).First();
                        ip.ActiveButton = ip.GetButton(c);
                    }

                    //Console.WriteLine(ax);
                    return ax;
                }

                return hx(this , iCode);
            }
            public string PressNumericButton(char ib)
            {

                string gx(c21Pad ip, char ic)
                {
                    if (ip.ChildPad != null)
                    {
                        var sx = gx(ip.ChildPad, ic).Split('|');
                        var iniActiveBn = ip.ActiveButton;
                        var s1 = "";
                        var s2 = "";
                        foreach (char c in sx[0])
                        {
                            var tb = ip.GetButton(c);
                            s1 += ip.ActiveButton.PathTo[tb][0] + "A";
                            ip.ActiveButton = tb;
                        }
                        if (sx.Length > 1)
                        {
                            ip.ActiveButton = iniActiveBn;
                            foreach (char c in sx[1])
                            {
                                var tb = ip.GetButton(c);
                                s2 += ip.ActiveButton.PathTo[tb][0] + "A";
                                ip.ActiveButton = tb;
                            }
                        }

                        var s = s2.Length > s1.Length ? s2 : s1;
                        Console.WriteLine(s);
                        return s;
                    }
                    else
                    {
                        var tb = ip.GetButton(ic);
                        var s = ip.ActiveButton.PathTo[tb][0] + "A";
                        if (ip.ActiveButton.PathTo[tb].Count > 1)
                        {
                            s += "|" + ip.ActiveButton.PathTo[tb][1] + "A";
                        }
                        ip.ActiveButton = tb;


                        Console.WriteLine(s);
                        return s;
                    }
                };

                var ps = gx(this, ib);

                return ps;
            }


            string getDirFromActiveTo(char ib)
            {

                return "";
            }


            public void PrintMap()
            {
                Console.WriteLine($"\n");
                foreach (var r in this)
                {
                    foreach (var c in r)
                        Console.Write(
                          c.IsEmpty ? ' '
                        : c.IsA ? c.s
                        : c.s);
                    Console.Write("\n");
                }
                Console.WriteLine($"\n");
            }

        }


        static void day21()
        {
            Console.WriteLine($"Answer1: {day21LogicPart1()}");
            Console.WriteLine($"Answer2: {day21LogicPart2()}");
        }


        static long day21LogicPart1()
        {
            var pn1 = new c21Pad(d21_dataPadNumeric);
            var pd1 = new c21Pad(d21_dataPadDirectional) { ChildPad = pn1 };
            var pd2 = new c21Pad(d21_dataPadDirectional) { ChildPad = pd1 };

            pn1.ParentPad = pd1;
            pd1.ParentPad = pd2;

            var ans = 0;
            foreach (var code in d21_data)
            {
                //var code = "179A";
                var ax = pn1.PressNumericButton(code);
                Console.WriteLine($"{code}: {ax.Length} * {code[..3]} = {ax.Length * int.Parse(code[..3])}");
                ans += ax.Length * int.Parse(code[..3]);
            }

            return ans;
        }

        static long day21LogicPart2()
        {
            var pn = new c21Pad(d21_dataPadNumeric);
            var pr = new List<c21Pad>();
            for(int i = 0; i< 25; i++)
            {
                pr.Add(new c21Pad(d21_dataPadDirectional));
                pr[i].ChildPad = i == 0 ? pn : pr[i - 1];

                if (i == 0) pn.ParentPad = pr[i];
                if (i > 0) pr[i - 1].ParentPad = pr[i];
            }
            var ph = new c21Pad(d21_dataPadDirectional) { ChildPad = pr.Last() };

            var ans = 0L;
            foreach (var code in d21_data)
            {
                //var code = "179A";
                var ax = pn.PressNumericButtonLength(code);
                Console.WriteLine($"{code}: {ax} * {code[..3]} = {ax * int.Parse(code[..3])}");
                ans += ax * long.Parse(code[..3]);
            }
            //66587428950800 too low
            return ans;
        }

        static string[] d21_dataPadNumeric =
        """        
        789
        456
        123
        !0A
        """.Split(Environment.NewLine);

        static string[] d21_dataPadDirectional =
        """        
        !^A
        <v>
        """.Split(Environment.NewLine);

        static string[] d21_data0 =
        """        
        029A
        980A
        179A
        456A
        379A
        """.Split(Environment.NewLine);

    }
}
