using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {

        class c12DataCell
        {
            public c12DataCell(int id, char s, c12Rep parentRep)
            {
                this.id = id;
                this.s = s;
                ParentRep = parentRep;
            }
            public int id { get; set; }
            public char s { get; set; }

            public c12Rep ParentRep;
            public List<c12DataCell> ParentData => ParentRep.data;
            public int maxId => ParentData.Last().id;

            public c12DataCell GetNext => id < maxId ? ParentData[id + 1] : null;
            public c12DataCell GetPrev => id > 0 ? ParentData[id - 1] : null;


            public bool IsDmg => s == '#';
            public bool IsDot => s == '.';
            public bool IsUnk => s == '?';

            public bool IsDmgStart => (s == '#' && id == 0) || (s == '#' && GetPrev.s == '.');
            public bool IsDmgEnd => (s == '#' && id == maxId) || (s == '#' && GetNext.s == '.');

            public int IniDmgGrp { get; set; }
            public bool IsInCompleteDmg { get; set; }
            public int CompleteDmgIdx { get; set; }
            public int UnkGrpIdx { get; set; }


            public override string ToString() => $"{s}[{id}]";
        }

        class c12Group
        {
            public List<c12DataCell> grpData { get; set; }
            public int grpId => grpData.First().IniDmgGrp;
            public c12Rep ParentRep => grpData[0].ParentRep;
            public List<c12DataCell> ParentData => grpData[0].ParentData;
            public int permutations { get; set; }

            public List<int> GrpChk { get; set; } = new List<int>();


            public bool IsAllDmg => grpData.All(x => x.IsDmg);
            public int DmgCnt => grpData.Count(x => x.IsDmg);
            public int DmgMissing => ChkCnt - DmgCnt;
            public int UnkCnt => grpData.Count(x => x.IsUnk);
            public bool IsAllUnk => grpData.All(x => x.IsUnk);
            public int UnkGrpCnt => UnkGrpData.Count();
            public List<c12Group> UnkGrpData => grpData
                .Where(u => u.UnkGrpIdx > 0).GroupBy(g => g.UnkGrpIdx)
                .Select(g => new c12Group { grpData = g.ToList() }).ToList();
            public int Count => grpData.Count();
            public int OprCnt { get; set; } = 0;
            public bool HasOpr => OprCnt > 0;
            public int ChkCnt => GrpChk.Sum();
            public int CntUsed => ChkCnt + OprCnt;
            public int CntAvail => Count - CntUsed;

            public override string ToString() =>
                $"[{new string(grpData.Select(x => x.s).ToArray())}]" +
                $" [{string.Join(',', GrpChk)}] " +
                $" | prm={permutations.ToString("#0.0")}";
        }

        class c12Rep
        {
            public string repeatString(string str, int count)
            {
                string outs = "";
                for (int i = 0; i < count; i++)
                {
                    outs += str;
                    if (i + 1 < count) outs += "?";
                }
                return outs;
            }
            public c12Rep(string s, int index, int repeat = 0, bool exclCompleteDamage = false, bool parseId = false)
            {
                if (parseId)
                {
                    id = int.Parse(s.Split(' ', 2)[0]);
                    s = s.Split(' ', 2)[1];
                }
                else
                {
                    id = index;
                }
                LineRaw = s;
                var sl = s.Split(' ');


                var ichk = sl[1].Split(',').Select(d => int.Parse(d)).ToList();

                LineOrig = sl[0];
                LineAdj = LineOrig;

                if (repeat == 0)
                {
                    chk = ichk.ToList();

                    if (exclCompleteDamage)
                    {
                        Regex regEx;
                        string cs;
                        var rchk = chk.ToList();
                        for (int i = 0; i < rchk.Count; i++)
                        {
                            if (rchk.Count(x => x == rchk[i]) == 1)
                            {
                                cs = new string('#', rchk[i]);

                                if (id == 693)
                                {
                                    i = i;
                                }

                                regEx = new Regex($"\\.{cs}\\.");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, "..", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }

                                regEx = new Regex($"^{cs}\\.");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, ".", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }

                                regEx = new Regex($"\\.{cs}$");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, ".", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                    }


                }
                else
                {
                    chk = ichk.ToList();


                    if (exclCompleteDamage)
                    {
                        Regex regEx;
                        string cs;
                        var rchk = chk.ToList();
                        for (int i = 0; i < rchk.Count; i++)
                        {
                            if (rchk.Count(x => x == rchk[i]) == 1)
                            {
                                cs = new string('#', rchk[i]);

                                if (id == 693)
                                {
                                    i = i;
                                }

                                regEx = new Regex($"\\.{cs}\\.");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, "..", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }

                                regEx = new Regex($"^{cs}\\.");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, ".", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }

                                regEx = new Regex($"\\.{cs}$");
                                if (regEx.IsMatch(LineAdj) && regEx.Replace(LineAdj, ".", 1).Count(c => c == '#') > 0)
                                {
                                    LineAdj = regEx.Replace(LineAdj, ".", 1);
                                    chk.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                    }

                    var xchk = chk.ToList();
                    for (int i = 0; i < repeat - 1; i++) chk.AddRange(xchk.ToList());

                    LineAdj = repeatString(LineAdj, repeat);



                }



                LineAdj = Regex.Replace(LineAdj, "\\.{3,}", ".");
                if (exclCompleteDamage)
                {

                    //LineAdj = Regex.Replace(LineAdj, "\\.{2,}", ".").Trim('.');
                }







                data = LineAdj.Select((x, index) => new c12DataCell(index, x, this)).ToList();





                //InitData
                //initData_ExistingCompleteDamage();
                //initData_RemoveCompleteDamage();
                //initData_SetInitialDamageGrouping();
                initData_UnknownGroups();







            }
            public int id { get; set; }
            public string LineRaw { get; set; }
            public string LineOrig { get; private set; }
            public string LineAdj { get; private set; }
            public List<c12DataCell> data { get; set; }
            List<c12DataCell> dataNoDot => data.Where(d => !d.IsDot).ToList();
            public List<int> chk { get; set; }

            public List<c12Group> IniDmgGrps { get; set; }
            public long PermutationsRepeat { get; set; }
            public long Permutations { get; set; }

            void initData_SetInitialDamageGrouping()
            {
                foreach (var dc in data)
                {
                    if (dc.s != '.' && dc.GetPrev != null)
                    {
                        if (dc.GetPrev.IsDot)
                            dc.IniDmgGrp = dc.GetPrev.GetPrev.IniDmgGrp + 1;
                        else
                            dc.IniDmgGrp = dc.GetPrev.IniDmgGrp;
                    }
                    else
                    {
                        if (dc.s == '.') dc.IniDmgGrp = -1;
                    }
                }
                IniDmgGrps = data.Where(d => !d.IsDot).GroupBy(d => d.IniDmgGrp).Select(x => new c12Group { grpData = x.ToList() }).ToList();

                //assign group-level check data
                if (IniDmgGrps.Count == chk.Count)
                {
                    IniDmgGrps.ForEach(x => x.GrpChk = new List<int> { chk[x.grpId] });
                }
                else if (IniDmgGrps.Count == 1)
                {
                    IniDmgGrps.ForEach(x => x.GrpChk = chk.ToList());
                }
                else
                {
                    var ick = chk.ToList();
                    for (int i = 0; i < IniDmgGrps.Count; i++)
                    {

                        var dg = IniDmgGrps[i];
                        dg.GrpChk = new List<int>();


                        if (ick.Count == 0)
                        {
                            Console.WriteLine("error!!0");
                        }
                        else if (dg.Count < chk.Min())
                        {
                            Console.WriteLine("error!!0a");
                        }
                        else if (dg.IsAllDmg) // dont need to worry about periods
                        {
                            if (ick.Count > 0 && ick.First() == dg.Count)
                            {
                                dg.GrpChk.Add(ick.First());
                                ick.Remove(ick.First());
                            }
                            else
                            {
                                Console.WriteLine("error!!1");
                            }
                        }
                        else if (i == IniDmgGrps.Count - 1)
                        {
                            if (ick.Sum() <= dg.CntAvail)
                            {
                                dg.GrpChk.AddRange(ick);
                            }
                            else
                            {
                                Console.WriteLine("error!!2");
                            }
                        }
                        else
                        {
                            dg.GrpChk.Add(ick.First());
                            ick.Remove(ick.First());

                            while (ick.Count > 0 && ick.First() < dg.CntAvail)
                            {
                                dg.GrpChk.Add(ick.First());
                                ick.Remove(ick.First());
                                dg.OprCnt++;
                            }
                        }

                        if (dg.CntAvail < 0)
                        {
                            Console.WriteLine("error!!3");
                        }
                    }
                }

            }
            void initData_ExistingCompleteDamage()
            {

                var tmpcnt = 1;
                var tmp = new List<c12DataCell>();
                foreach (var dc in data)
                {
                    if (dc.IsDot || dc.IsUnk)
                    {
                        tmp.Clear();
                    }
                    else if (dc.IsDmgStart && dc.IsDmgEnd)
                    {
                        dc.IsInCompleteDmg = true;
                        dc.CompleteDmgIdx = tmpcnt;
                        tmpcnt++;
                        tmp.Clear();
                    }
                    else if (dc.IsDmgStart && tmp.Count > 0)
                    {
                        tmp.Clear();
                        tmp.Add(dc);
                    }
                    else if (dc.IsDmgStart && tmp.Count == 0)
                    {
                        tmp.Add(dc);
                    }
                    else if (dc.IsDmgEnd && tmp.Count == 0)
                    {
                        tmp.Clear();
                    }
                    else if (dc.IsDmgEnd)
                    {
                        tmp.Add(dc);
                        tmp.ForEach(d => { d.IsInCompleteDmg = true; d.CompleteDmgIdx = tmpcnt; });
                        tmpcnt++;
                        tmp.Clear();
                    }
                    else if (dc.IsDmg && tmp.Count > 0)
                    {
                        tmp.Add(dc);
                    }
                    else
                    {
                        tmp.Clear();
                    }
                }
            }

            void initData_RemoveCompleteDamage()
            {
                if (data.Any(d => d.IsInCompleteDmg))
                {
                    var tmpData = data.ToList();
                    for (int i = 0; i < tmpData.Count; i++)
                    {
                        var dc = tmpData[i];
                        if (dc.IsInCompleteDmg)
                        {
                            dc.id = -1;
                            if (data[i].GetNext != null && data[i].GetNext.s == '.')
                                dc.id = -1;
                        }
                    }
                    data = tmpData.Where(d => d.id >= 0).ToList();
                    if (data.Last().s == '.') data.Remove(data.Last());
                }
            }

            void initData_UnknownGroups()
            {
                var ug = 0;
                foreach (var dc in data)
                {
                    if (dc.IsUnk)
                    {
                        if (ug == 0) ug++;
                        dc.UnkGrpIdx = ug;
                    }
                    else if (ug > 0 && (dc.GetNext?.IsUnk ?? false))
                    {
                        ug++;
                    }

                }
            }



            public int chkSum => chk.Sum();

            string minPossible => string.Join(".", chk.Select(c => new string('#', c))).PadRight(data.Count, '.');

            public bool IsAllDmg => dataNoDot.All(x => x.IsDmg);
            public int DmgCnt => dataNoDot.Count(x => x.IsDmg);
            public int DmgMissing => chkSum - DmgCnt;

            public bool IsAllUnk => dataNoDot.Where(d => !d.IsDot).All(x => x.IsUnk);
            public int UnkCnt => dataNoDot.Count(x => x.IsUnk);
            public int UnkGrpCnt => UnkGrpData.Count();
            public List<c12Group> UnkGrpData => data
                .Where(u => u.UnkGrpIdx > 0).GroupBy(g => g.UnkGrpIdx)
                .Select(g => new c12Group { grpData = g.ToList() }).ToList();

            public int AllCnt => data.Count();




            string getRxPattern(List<int> cnts = null)
            {
                if (cnts == null) cnts = chk;
                var rxPattern = @"#{" + chk[0].ToString() + @"}";
                for (int i = 1; i < cnts.Count; i++)
                    rxPattern += @"\.{1,}#{" + chk[i].ToString() + @"}";
                rxPattern += @"";
                return rxPattern;

            }


            public c12DataCell[] unks => data.Where(d => d.IsUnk).ToArray();


            public long getSolutions0()
            {

                foreach (var dg in IniDmgGrps)
                {
                    dg.permutations = 1;

                    if (DmgMissing == UnkCnt) continue;
                    if (dg.grpData.All(d => d.IsInCompleteDmg)) continue;
                    if (dg.ChkCnt == dg.Count) continue;

                    if (dg.IsAllUnk)
                    {
                        var mca = (new string('#', dg.DmgMissing) + new string('.', dg.Count - dg.DmgMissing)).ToList();
                        var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                        var rxp = getRxPattern(dg.GrpChk);
                        var pcnt = prems.Count(x => Regex.IsMatch(x, rxp));

                        dg.permutations = pcnt;
                    }
                    else
                    {
                        var ugp = new List<permDataGrp>();
                        foreach (var ug in dg.UnkGrpData)
                        {
                            var ipre = "";
                            var ixf = ug.grpData.First();
                            while (ixf != null && ixf.GetPrev != null && ixf.GetPrev.s == '#')
                            {
                                ipre = ixf.GetPrev.s + ipre;
                                ixf = ixf.GetPrev;
                            }; //new string(dg.grpData.Reverse<c12DataCell>().Skip(ug.grpData.Count-ug.grpData.First().id).TakeWhile(g => g.s == '#').Select(g => g.s).ToArray()),

                            var ipost = "";
                            var ixl = ug.grpData.Last();
                            while (ixl != null && ixl.GetNext != null && ixl.GetNext.s == '#')
                            {
                                ipost = ipost + ixl.GetNext.s;
                                ixl = ixl.GetNext;
                            }; //new string(dg.grpData.Skip(ug.UnkGrpData.Last().i + 1).TakeWhile(g => g.s == '#').Select(g => g.s).ToArray());

                            if (ug.Count == 1)
                            {

                                ugp.Add(new permDataGrp()
                                {
                                    pre = ipre,
                                    post = ipost,
                                    prms = new List<string> { "#", "." }
                                });
                            }
                            else
                            {
                                var mca = (new string('#', dg.DmgMissing) + new string('.', ug.Count - dg.DmgMissing)).ToList();
                                var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();
                                ugp.Add(new permDataGrp()
                                {
                                    pre = ipre,
                                    post = ipost,
                                    prms = prems
                                });

                            }
                        }
                        var xn = dg.UnkGrpData[0].grpData[0].GetNext;
                        var xp = dg.UnkGrpData[0].grpData[0].GetPrev;
                        var xpl = dg.UnkGrpData[0].grpData.Last().id;
                        var rxp = getRxPattern(dg.GrpChk);

                        var perms = getPerm(ugp, "", rxp, dg.GrpChk.Sum());
                        dg.permutations = perms.Count();

                    }

                }



                return Permutations;


                //var mc = new string('#', dmgMissing) + new string('.', unkCnt - dmgMissing);
                //var mca = mc.ToList();
                //var coboa = GetPowerSet<char>(mca).Select((x, index) => new { idx = index, chrs = x.ToArray() }).ToList();
                //var coboi = mca.DifferentCombinations(3).Select((x, index) => new { idx = index, chrs = new string(x.ToArray()) }).ToList();
                //var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                //var sss = Regex.IsMatch("#.#", rxp);
                //var fff = prems.Count(x => Regex.IsMatch(x, rxp));


            }


            public long getSolutions1()
            {

                if (DmgMissing == UnkCnt) //all ? is #
                {
                    Permutations = 1;
                }
                else if (IsAllDmg) //all data is #
                {
                    Permutations = 1;
                }
                else if (IsAllUnk) // all ?
                {
                    var mca = (new string('#', DmgMissing) + new string('.', AllCnt - DmgMissing)).ToList();
                    var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                    var rxp = getRxPattern(chk);
                    var pcnt = prems.Count(x => Regex.IsMatch(x, rxp));

                    Permutations = pcnt;
                }
                else
                {
                    var ugp = new List<permDataGrp>();
                    foreach (var ug in UnkGrpData)
                    {
                        var ipre = "";
                        var ixf = ug.grpData.First();
                        while (ixf != null && ixf.GetPrev != null && ixf.GetPrev.s != '?')
                        {
                            ipre = ixf.GetPrev.s + ipre;
                            ixf = ixf.GetPrev;
                        };

                        var ipost = "";
                        var ixl = ug.grpData.Last();
                        while (ixl != null && ixl.GetNext != null && ixl.GetNext.s != '?')
                        {
                            ipost = ipost + ixl.GetNext.s;
                            ixl = ixl.GetNext;
                        };

                        if (ug.Count == 1)
                        {

                            ugp.Add(new permDataGrp()
                            {
                                pre = ipre,
                                post = ipost,
                                prms = new List<string> { "#", "." }
                            });
                        }
                        else
                        {
                            var mc = "";
                            if (ug.Count < chk.Min())
                                mc = new string('.', ug.Count);
                            else
                                mc = (new string('#', DmgMissing) + new string('.', ug.Count - DmgMissing));

                            var mca = mc.ToList();

                            var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();
                            ugp.Add(new permDataGrp()
                            {
                                pre = ipre,
                                post = ipost,
                                prms = prems
                            });

                        }
                    }

                    var rxp = getRxPattern(chk);



                    if (UnkGrpData.Count == 1)
                    {
                        var ug = ugp.First();
                        Permutations = ug.prms.Select(g => ug.pre + g + ug.post).Count(x => Regex.IsMatch(x, rxp));
                    }
                    else
                    {
                        Permutations = getPerm(ugp, "", rxp, chk.Sum()).Count();
                    }


                }


                return Permutations;

                //var mc = new string('#', dmgMissing) + new string('.', unkCnt - dmgMissing);
                //var mca = mc.ToList();
                //var coboa = GetPowerSet<char>(mca).Select((x, index) => new { idx = index, chrs = x.ToArray() }).ToList();
                //var coboi = mca.DifferentCombinations(3).Select((x, index) => new { idx = index, chrs = new string(x.ToArray()) }).ToList();
                //var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                //var sss = Regex.IsMatch("#.#", rxp);
                //var fff = prems.Count(x => Regex.IsMatch(x, rxp));


            }

            public long getSolutions2()
            {

                if (DmgMissing == UnkCnt) //all ? is #
                {
                    Permutations = 1;
                }
                else if (IsAllDmg) //all data is #
                {
                    Permutations = 1;
                }
                else if (IsAllUnk) // all ?
                {
                    var mca = (new string('#', DmgMissing) + new string('.', AllCnt - DmgMissing)).ToList();
                    var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                    var rxp = getRxPattern(chk);
                    var pcnt = prems.Count(x => Regex.IsMatch(x, rxp));

                    Permutations = pcnt;
                }
                else
                {
                    var prms = new List<string>();

                    if (data.Count == 1)
                    {
                        prms = new List<string> { "#", "." };
                    }
                    else
                    {
                        var mc = "";
                        if (data.Count < chk.Min())
                            mc = new string('.', data.Count);
                        else
                            mc = (new string('#', DmgMissing) + new string('.', UnkCnt - DmgMissing));

                        var mca = mc.ToList();

                        prms = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                    }

                    var rxp = getRxPattern(chk);
                    Permutations = prms.Select(g => convertPermData(g)).Count(x => Regex.IsMatch(x, rxp));

                }


                return Permutations;

                //var mc = new string('#', dmgMissing) + new string('.', unkCnt - dmgMissing);
                //var mca = mc.ToList();
                //var coboa = GetPowerSet<char>(mca).Select((x, index) => new { idx = index, chrs = x.ToArray() }).ToList();
                //var coboi = mca.DifferentCombinations(3).Select((x, index) => new { idx = index, chrs = new string(x.ToArray()) }).ToList();
                //var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                //var sss = Regex.IsMatch("#.#", rxp);
                //var fff = prems.Count(x => Regex.IsMatch(x, rxp));


            }
            public long getSolutions3()
            {

                if (DmgMissing == UnkCnt) //all ? is #
                {
                    Permutations = 1;
                }
                else if (IsAllDmg) //all data is #
                {
                    Permutations = 1;
                }
                else
                {
                    var prms = new List<string>();

                    var slack = data.Count - (chk.Sum() + chk.Count() - 1);
                    var giLst = minPossible.Trim('.').Select((c, index) => new { idx = index, chr = c }).Where(c => c.chr == '.').Select(c => c.idx).ToList(); giLst.Insert(0, 0);
                    prms.Add(minPossible);
                    for (var i = 0; i < giLst.Count; i++)
                    {
                        var dcx = giLst.DifferentCombinations(i + 1).Select(x => x.ToList()).ToList();
                        for (int gi = 0; gi < dcx.Count; gi++)
                        {
                            for (int j = 1; j <= slack / dcx[gi].Count; j++)
                            {
                                prms.Add(getModData(dcx[gi], j));
                            }
                        }
                    }

                    var chrChecker = (string sChk, string sWth) => {
                        for (var i = 0; i < sChk.Length; i++)
                        {
                            if (sWth[i] != '?' && sChk[i] != sWth[i])
                                return false;
                        }
                        return true;
                    };

                    var rxp = getRxPattern(chk);
                    Permutations = prms.Count(x => chrChecker(x, LineAdj));
                    prms.ForEach(x => Console.WriteLine(x));
                }


                return Permutations;

                //var mc = new string('#', dmgMissing) + new string('.', unkCnt - dmgMissing);
                //var mca = mc.ToList();
                //var coboa = GetPowerSet<char>(mca).Select((x, index) => new { idx = index, chrs = x.ToArray() }).ToList();
                //var coboi = mca.DifferentCombinations(3).Select((x, index) => new { idx = index, chrs = new string(x.ToArray()) }).ToList();
                //var prems = GeneratePermutations(mca).Select((x, index) => new string(x.ToArray())).Distinct().ToList();

                //var sss = Regex.IsMatch("#.#", rxp);
                //var fff = prems.Count(x => Regex.IsMatch(x, rxp));


            }

            public long getSolutions(int repeat = 0)
            {


                Permutations = loadPerms();

                if (repeat > 0)
                {
                    var rdata1 = new c12Rep(LineRaw, -1, 2, false);
                    var permsR1 = rdata1.getSolutions();

                    if (permsR1 > 1 && permsR1 % Permutations == 0)
                    {
                        var permMul = permsR1 / Permutations;
                        PermutationsRepeat = (long)Math.Pow((double)permMul, (double)repeat - 1) * (long)Permutations;
                    }
                    else if (permsR1 > 1 && permsR1 % Permutations != 0)
                    {
                        var rdata5 = new c12Rep(LineRaw, -1, 5, false);
                        PermutationsRepeat = rdata5.getSolutions();
                    }
                    else if (permsR1 == 0)
                    {
                        PermutationsRepeat = 0;
                    }
                    else //1
                    {
                        PermutationsRepeat = 1;
                    }

                    return PermutationsRepeat;
                }

                return Permutations;
            }
            public long getSolutionsDP()
            {
                //adaped from: https://github.com/womogenes/AoC-2023-Solutions/blob/main/day_12/day_12_p2.py

                Permutations = 0;

                var grps = chk.ToList();
                grps.Add(0);

                var rep = LineAdj + '.';

                var n = rep.Length;
                var m = grps.Count;
                var p = grps.Max();


                long[][][] dp = new long[n][][];
                for (int i = 0; i < n; i++)
                {
                    dp[i] = new long[m][];
                    for (int j = 0; j < m; j++)
                        dp[i][j] = new long[p + 1];
                }
                //Console.WriteLine($"{rep} | {string.Join(',', grps)}\n");
                //Console.WriteLine($"c| i|{string.Join(new string(' ', p + 1), grps)}{new string(' ', p)} grp values");
                //Console.WriteLine($" |  |{string.Join(new string(' ', p + 1), grps.Select((d, i) => i))} {new string(' ', p)}j: grp Index");
                //Console.WriteLine($" |  |{(string.Join(" ",Enumerable.Repeat(string.Join("",Enumerable.Range(0,p+1)),grps.Count)))} k: Max Grp Len Index");
                //Console.WriteLine($"{new string('-', 5 + grps.Count * (p + 1))}");

                for (int i = 0; i < n; i++)
                {
                    var c = rep[i];
                    for (int j = 0; j < m; j++)
                    {
                        for (int k = 0; k < grps[j] + 1; k++)
                        {

                            if (j == 0 && k == 0) Console.Write($"{rep[i]}|{i.ToString("00")}|");

                            if (i == 0) //base case 
                            {
                                if (j != 0)
                                {
                                    dp[i][j][k] = 0;
                                }
                                else if (c == '#')
                                {
                                    dp[i][j][k] = k != 1 ? 0 : 1;
                                }
                                else if (c == '.')
                                {
                                    dp[i][j][k] = k != 0 ? 0 : 1;
                                }
                                else if (c == '?')
                                {
                                    dp[i][j][k] = k != 0 && k != 1 ? 0 : 1;
                                }
                                else
                                {
                                    Console.WriteLine("dp error in base case assignment");
                                }
                            }
                            else
                            {
                                long ansDot, ansPnd;

                                //Process answer if current char is .
                                if (k != 0)
                                {
                                    ansDot = 0;
                                }
                                else if (j > 0)
                                {
                                    ansDot = dp[i - 1][j - 1][grps[j - 1]];
                                    ansDot += dp[i - 1][j][0];
                                }
                                else
                                {
                                    // i>0, j=0, k=0.
                                    // Only way to do this is if every ? is a .
                                    ansDot = !rep[..i].Any(x => x == '#') ? 1 : 0;
                                }

                                //Process answer if current char is #
                                if (k == 0)
                                {
                                    ansPnd = 0;
                                }
                                else
                                {
                                    ansPnd = dp[i - 1][j][k - 1]; //Newest set
                                }


                                //Set dp value
                                if (c == '.')
                                {
                                    dp[i][j][k] = ansDot;
                                }
                                else if (c == '#')
                                {
                                    dp[i][j][k] = ansPnd;
                                }
                                else
                                {
                                    dp[i][j][k] = ansDot + ansPnd;
                                }
                            }

                            //Console.Write($"{dp[i][j][k]}");
                            //if (k == grps[j])
                            //{
                            //    while (k < p+1) { Console.Write(" "); k++; }
                            //}
                            //if (j == grps.Count - 1 && k >= grps[j])
                            //{
                            //    Console.WriteLine("");
                            //}

                        }
                    }
                }

                Permutations = dp[n - 1][m - 1][0];

                return Permutations;
            }

            bool pramChecker(string sChk)
            {
                for (var i = 0; i < sChk.Length; i++)
                {
                    if (LineAdj[i] != '?' && sChk[i] != LineAdj[i])
                        return false;
                }
                return true;
            }


            public long loadPerms(List<int> groups = null, int slack = -1, string prev = null)
            {
                long count = 0;
                if (groups == null)
                {
                    groups = minPossible.Trim('.').Select((c, index) => new { idx = index, chr = c }).Where(c => c.chr == '.').Select(c => c.idx).ToList();
                    groups.Insert(0, -1);
                    slack = data.Count - (chk.Sum() + chk.Count() - 1);
                    prev = minPossible;
                    if (pramChecker(minPossible))
                        count++;
                }
                var cur = prev.ToList();
                if (groups.Count > 1)
                {
                    count += loadPerms(groups.Skip(1).ToList(), slack, prev);
                }
                do
                {
                    cur.Insert(groups.First() + 1, '.');
                    cur.RemoveAt(cur.Count - 1);
                    slack -= 1;
                    var plast = new string(cur.ToArray());
                    if (pramChecker(plast))
                        count++;
                    for (int j = 1; j < groups.Count; j++)
                    {
                        groups[j]++;
                    }
                    if (groups.Count > 1 && slack > 0 && pramChecker(plast[0..groups[1]]))
                    {
                        count += loadPerms(groups.Skip(1).ToList(), slack, plast);
                    }
                } while (slack > 0);

                return count;
            }
            public void loadPermsxx(List<string> outs, List<int> groups = null, int slack = -1, string prev = null)
            {
                if (groups == null)
                {
                    outs.Add(minPossible);
                    groups = minPossible.Trim('.').Select((c, index) => new { idx = index, chr = c }).Where(c => c.chr == '.').Select(c => c.idx).ToList();
                    groups.Insert(0, -1);
                    slack = data.Count - (chk.Sum() + chk.Count() - 1);
                    prev = outs.Last();
                }
                var cur = prev.ToList();
                if (groups.Count > 1)
                {
                    loadPermsxx(outs, groups.Skip(1).ToList(), slack, prev);
                }
                do
                {
                    cur.Insert(groups.First() + 1, '.');
                    cur.RemoveAt(cur.Count - 1);
                    slack -= 1;
                    var plast = new string(cur.ToArray());
                    if (pramChecker(plast))
                        outs.Add(plast);
                    for (int j = 1; j < groups.Count; j++)
                    {
                        groups[j]++;
                    }
                    if (groups.Count > 1 && slack > 0 && pramChecker(plast[0..groups[1]]))
                    {
                        loadPermsxx(outs, groups.Skip(1).ToList(), slack, plast);
                    }
                } while (slack > 0);

            }


            public void loadPermsx(List<string> outs, List<int> groups = null, int slack = -1)
            {
                if (groups == null)
                {
                    outs.Add(minPossible);
                    groups = minPossible.Trim('.').Select((c, index) => new { idx = index, chr = c }).Where(c => c.chr == '.').Select(c => c.idx).ToList();
                    groups.Insert(0, -1);
                    slack = data.Count - (chk.Sum() + chk.Count() - 1);
                }
                var prev = outs.Last();
                var cur = outs.Last().ToList();
                if (groups.Count > 1)
                {
                    loadPermsx(outs, groups.Skip(1).ToList(), slack);
                }
                do
                {
                    cur.Insert(groups.First() + 1, '.');
                    cur.RemoveAt(cur.Count - 1);
                    slack -= 1;
                    outs.Add(new string(cur.ToArray()));
                    for (int j = 1; j < groups.Count; j++)
                    {
                        groups[j]++;
                    }
                    if (groups.Count > 1 && slack > 0)
                    {
                        loadPermsx(outs, groups.Skip(1).ToList(), slack);
                    }
                } while (slack > 0);

            }

            string getModData(List<int> ig, int len)
            {
                var oc = minPossible.ToList();

                for (int i = ig.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < len; j++)
                    {
                        oc.Insert(ig[i], '.');
                        oc.RemoveAt(oc.Count - 1);
                    }
                }

                return new string(oc.ToArray());
            }

            string convertPermData(string iul)
            {
                char[] rd = data.Select(d => d.s).ToArray();
                var i = 0;
                foreach (var dc in data.Where(d => d.IsUnk))
                {
                    rd[dc.id] = iul[i];
                    i++;
                }
                return new string(rd);
            }
            class permDataGrp
            {
                public string pre { get; set; }
                public string post { get; set; }
                public List<string> prms { get; set; }
            }

            HashSet<string> getPerm(List<permDataGrp> ls, string ps, string rxp, int maxDmg)
            {
                var lsx = ls.ToList(); lsx.RemoveAt(0);
                var perms = new HashSet<string>();
                foreach (var l in ls.First().prms)
                {
                    if (lsx.Count > 0)
                    {
                        perms.UnionWith(getPerm(lsx, ps + ls.First().pre + l, rxp, maxDmg));
                    }
                    else
                    {
                        var rst = ps + ls.First().pre + l + ls.First().post;
                        if (rst.Count(s => s == '#') <= maxDmg && Regex.IsMatch(rst, rxp))
                        {
                            perms.Add(rst);
                        }
                    }
                }
                return perms;
            }

            public override string ToString() => $"[{new string(data.Select(x => x.s).ToArray())}]" +
                $"   [{string.Join(',', chk)}] " +
                $"| prm={Permutations}" +
                //$" :x5= {PermutationsRepeat}" +
                $"|    {DateTime.Now}" +
                $"";

            //public override string ToString() => $"[{string.Join('|', IniDmgGrps.Select(x =>
            //    new string(
            //        x.grpData.Select(d => d.s).ToArray())))}]" +
            //    $"   [{string.Join(',', chk)}] " +
            //    $"| prm={Permutations.ToString("#0.0")}";
        }

        static string day12()
        {
            var d = d12_data;


            var map = d.Select((d, index) => new c12Rep(d, index, 0, false)).ToList();
            //var map = d.Select((d, index) => new c12Rep(d, index, 5, false)).ToList();
            //var map5 = d12_data0c.Select((d, index) => new c12Rep(d, index, 5, false, true)).ToList();


            foreach (var dr in map.OrderBy(x => x.id))
            {
                dr.getSolutionsDP();
                Console.WriteLine(dr);
            }


            //foreach (var dr in map.OrderBy(x => x.id))
            //{
            //    dr.getSolutions(5);
            //    Console.WriteLine(dr);
            //}

            //for (var i = 0; i < map5.Count; i++)
            //{
            //    var dr = map5[i];
            //    dr.getSolutions();
            //    if (map[dr.id].PermutationsRepeat != map5[i].Permutations)
            //        Console.WriteLine($"{dr.id} | {map[dr.id].Permutations} | {map[dr.id].PermutationsRepeat} | {map5[i].Permutations} | {map[dr.id].LineRaw}");
            //}


            Console.WriteLine($"Answer1: {map.Sum(d => d.Permutations)}");




            map = d.Select((d, index) => new c12Rep(d, index, 5, false)).ToList();
            foreach (var dr in map.OrderBy(x => x.id))
            {
                dr.getSolutionsDP();
                Console.WriteLine(dr);
            }

            long ans2 = 0;
            foreach (var m in map) ans2 += m.Permutations;
            //foreach (var m in map) ans2+=m.PermutationsRepeat;
            //map = d.Select((d, index) => new c12Rep(d, index, 5, false)).ToList();
            //foreach (var dr in map)
            //{
            //    dr.getSolutions();
            //    ans2 += dr.Permutations;
            //    Console.WriteLine(dr);
            //}


            Console.WriteLine($"Answer2: {ans2}");

            //10121205901726 too low
            //15464163264
            //17788038834112

            var cd = "";
            //Console.WriteLine("\n\n\n");
            //foreach (var dr in map.OrderBy(x => x.id))
            //    Console.WriteLine($"{dr.id}|{dr.LineRaw}|{dr.Permutations}|{dr.PermutationsRepeat}");
            //cd = string.Join("\r\n",map.OrderBy(x => x.id).Select(dr => $"{dr.id}\t{dr.LineRaw}\t{dr.Permutations}\t{dr.PermutationsRepeat}"));
            return cd;




        }


        static string[] d12_data0 =
        """          
        .??..??...?##. 1,1,3      
        ???.### 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """.Split("\r\n");
        static string[] d12_data0b =
        """   
        #?#??.???.??? 1,1,3,1
        #?#??.???.????#?#??.???.??? 1,1,3,1,1,1,3,1
        #?#??.???.????#?#??.???.????#?#??.???.??? 1,1,3,1,1,1,3,1,1,1,3,1
        #?#??.???.????#?#??.???.????#?#??.???.????#?#??.???.??? 1,1,3,1,1,1,3,1,1,1,3,1,1,1,3,1
        #?#??.???.????#?#??.???.????#?#??.???.????#?#??.???.????#?#??.???.??? 1,1,3,1,1,1,3,1,1,1,3,1,1,1,3,1,1,1,3,1
        ????.#...#... 4,1,1
        ????.#...#...?????.#...#... 4,1,1,4,1,1
        ????.#...#...?????.#...#...?????.#...#... 4,1,1,4,1,1,4,1,1
        ????.#...#...?????.#...#...?????.#...#...?????.#...#... 4,1,1,4,1,1,4,1,1,4,1,1
        ????.#...#...?????.#...#...?????.#...#...?????.#...#...?????.#...#... 4,1,1,4,1,1,4,1,1,4,1,1,4,1,1
        ????.######..#####. 1,6,5
        ????.######..#####.?????.######..#####. 1,6,5,1,6,5
        ????.######..#####.?????.######..#####.?????.######..#####. 1,6,5,1,6,5,1,6,5
        ????.######..#####.?????.######..#####.?????.######..#####.?????.######..#####. 1,6,5,1,6,5,1,6,5,1,6,5
        ????.######..#####.?????.######..#####.?????.######..#####.?????.######..#####.?????.######..#####. 1,6,5,1,6,5,1,6,5,1,6,5,1,6,5
        .??..??...?##. 1,1,3        ????.######..#####.
        .??..??...?##.? 1,1,3
        .??..??...?##.?.??..??...?##. 1,1,3,1,1,3
        .??..??...?##.?.??..??...?##.?.??..??...?##. 1,1,3,1,1,3,1,1,3
        .??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##. 1,1,3,1,1,3,1,1,3,1,1,3
        .??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##. 1,1,3,1,1,3,1,1,3,1,1,3,1,1,3
        .??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##. 1,1,3,1,1,3,1,1,3,1,1,3,1,1,3,1,1,3
        ????.#...#... 4,1,1
        ???.### 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ???? 1
        ????.... 1
        ????...#. 1,1
        ????.#..#. 1,1,1
        ????.##..##. 1,2,2
        ????.###..##. 1,3,2
        ????.######..#####. 1,6,5
        .??..??...?##. 1,1,3
        ?#???????? 1,2,1
        ?##???????? 2,2,1
        ?###???????? 3,2,1
        ???..??#??????? 1,1,5,1,2
        ???.??#??????? 1,1,5,1,2
        .??..??...?##. 1,1,3      
        ????#?#????#?????.? 1,1,1,8
        """.Split("\r\n");

        static string[] d12_data0c =
        """   
        6 ##..#?????## 2,1,3
        16 #?.?###?#?#.?.#.?.#? 1,8,1,1,1,1
        57 ?#?#?????####??#?. 12,1
        83 ???..??#??????? 1,1,5,1,2
        107 ???#..#?????? 2,6
        133 ?..??????#..?? 1,6,2
        139 .??#????## 1,7
        140 #???#??.?.# 5,1
        160 .##.???????..#? 2,7,2
        180 ?..#?????????##?.??? 1,7,1,2,3
        191 #?#..?#..?????????.? 1,1,1,3,5,1
        192 ?.????#.?# 1,4,2
        194 ?#.#?.?#?? 1,1,1
        197 ?##?.?????#.??# 2,6,1
        231 ?#???#???#???. 1,4,5
        235 ?.?#?#.??# 1,3,3
        238 ??##??#?#??#??? 2,2,2,3
        245 ?##.?#???#. 2,6
        264 .??#?..##? 4,3
        285 ?.#???????##??#...?? 3,6
        289 ?#?#???.?#??#??###. 1,2,10
        307 ???##?#.#? 5,2
        311 .#????#.?#???. 6,1
        316 #??.?#????.?# 3,2,3,1
        325 #???.?#.?# 4,2,1
        347 ..##.#???#??? 2,6
        354 ?#??###..?#?#..?? 7,3
        370 #???????.#? 1,6,1
        382 #?#??#??#?#?##.?.# 6,1,1,2,1
        389 ???#.###...??##.?#?# 3,3,1,2,1,1
        417 .??????#.?????# 1,3,1,4,1
        424 #.?#??#?#??#?# 1,1,7,1
        425 .???????##?#??#?# 4,1,4,3
        428 .#..###??###??##? 1,8,2
        446 .??#.?.#????????? 1,1,1,10
        489 .???..?#.#?###?#??# 1,1,1,1,8
        514 ?#???#?##?##. 1,7
        520 ?##..?.#????? 2,5
        521 .?.#.##???.? 1,5
        537 ???.?#???##..? 1,1,7,1
        542 ??#??##?##?###??.. 5,7
        543 ??##..#?... 2,2
        544 ?????.??..? 5,2,1
        567 ?##?#??#??#?? 4,1,1
        600 #?#???#?.? 5,1,1
        613 #..?????????? 1,6,1,1
        616 ??#??#?.?#?#???? 4,3
        621 ?##.?????##??#?.#?#. 3,1,6,1,1,1
        668 #..?#????#?? 1,2,4,1
        683 ??###?#?.?#..? 3,2,1
        691 ?#????#.?#. 6,1
        707 ###?##...#???.. 6,2,1
        729 ??????##?##??#..#?? 2,1,3,5,3
        733 ?#???..??#.??#??# 5,1,1,4,1
        755 #??#.?#?????. 4,3,3
        767 ??#.?.#??.?##?#?#?## 1,1,1,1,1,9
        771 .#?#???.?.##? 6,1,3
        772 ..?#.#??.#?## 2,2,1,2
        777 ????#?#??????. 5,1,1,3
        780 ?.#?##????.# 8,1
        802 .??#??#????#.#???#? 4,1,4,3,1
        804 ???##???##....?## 9,2
        807 #??..??????????#?#? 3,14
        881 ????#???.???.#??. 1,3
        890 .#?.?.#????.# 1,3,1
        897 .??#?#?#######?? 2,11
        898 ?..?????###.??.??? 1,2,5,2,1,1
        902 ?#?#?.??..#??? 3,3
        912 .???##?#??..#?.?#?# 4,3,1,2,1
        913 ???#?#??.?# 5,1,1
        928 #?#?.?.#?#??? 3,5
        938 ?#?##???#.#? 4,1,1,2
        939 ??#?#?#????#??#??? 5,7,3
        944 ?#..??#?##. 2,5
        953 .??.##????#???##?. 2,6
        975 #??????###???#?? 10,1
        991 ?..#???#?.? 2,3
        993 ????##.#??###?? 2,7
        12 .#?.?#??#??.?#?#?.? 1,7,5,1
        49 ..#?.?.##??##..? 1,1,6,1
        99 .??##?##??? 4,3,1
        167 #?????###.#?..??. 4,3,2,2
        177 ?#??#.??#?#.? 1,1,5,1
        220 ?.?..???#. 1,1,4
        248 ???.????#.. 3,4
        276 ?#???#?#?#.??..?# 9,2,2
        328 .##??#.??? 2,1,3
        348 ??#??.??.?#.?#?.?# 2,1,2,1,1,2
        358 ??.#??#?###?#?##. 2,1,10
        367 ?.#?#????#????#??? 5,10
        405 ?#??..??#?. 1,1,4
        480 ??#??##.#?#??#??# 5,1,1,1,2
        486 ?###?###.?????? 3,3,6
        500 ?###?#?##??? 8,2
        509 .?..#.##????? 1,1,6
        511 ???#??.?#?#? 1,2,1,3
        570 ?????.#??. 5,2
        596 ?????#???#??. 1,4,5
        747 .?..????#?????# 1,11
        781 ?#?..?.#???. 3,1,3
        825 .???.?##.?? 3,3,2
        840 ????#.#????##???.?.# 3,10,1,1
        893 .?#..?#?#?.? 2,3,1
        917 ??#?#.#????.#????? 5,1,3,4
        946 ##?.#??#?.? 3,4
        983 ?.?#???###?#???. 1,2,1,3,4
        2 #?#???##??#.?#?#?#? 3,3,1,7
        10 ?#??.#??.?. 2,3
        15 ???#??###?..#?.?#..# 9,1,1,1
        58 ?.???#?#?????.#? 10,1
        61 #?#.?#?#?##????.#.? 3,8,1
        62 ??#?##??####.?.#.?? 12,1,1,1
        67 #??.?.??????? 1,1,1,6
        73 #??.?#.??..##?? 1,2,1,3
        74 #.??.#?.#??.#?????? 1,1,2,2,5,1
        85 #??#???#????##???.? 4,4,1,2,1,1
        95 ?.?#?#??.?.#???? 5,3
        114 #?#??##.??? 7,2
        116 #?.???????.? 1,6,1
        118 #????#.?????? 1,1,1,6
        119 .?#???.#??? 4,2
        126 ..??????#.???.. 5,2
        138 ??#??#???#? 1,1,2
        146 #?#.????.? 1,1,3
        158 ?#??#??#??? 1,1,3
        175 ?#????##?# 3,4
        181 #???.#???...? 4,1,1,1
        183 .#?##???#? 1,2,2
        201 ??#?????#?.?.#?? 9,3
        207 ????????.#? 7,2
        209 #???#?.?????####### 2,2,11
        221 #????#??.?????? 1,1,1,1,6
        222 ?#.???##??. 1,3
        228 ??#?#??.?????.? 4,5
        239 ?##?..#.?.?# 3,1,1,1
        240 .#?#?.?.?????.? 3,4
        242 .?#?##???.#??? 7,2
        282 ?#??.##????# 2,4,1
        290 ?????#?.??# 4,1
        308 ?##??#.???.????#?? 6,1,1,1,2,1
        330 ?.???#?#???# 1,1,7
        338 ?.?##??..#??.??? 4,2
        349 ????#?.#??.#??#. 5,2,2,1
        364 ??#?#???####????#### 4,12
        404 .?#?#.?#?#??#?#?? 2,1,10
        427 #?.????#???..? 1,2
        430 ????????.##??? 7,5
        451 #?.#?..???#? 1,2,1,1
        463 ?##?#??##.?#????###? 9,9
        483 ????????###?#????#?# 1,15,1
        522 ?.????????###?##. 6,6
        539 ????.??.???? 4,1,2,1
        547 ??#?.??#???.# 1,2,5,1
        550 .#????#?#.???## 4,1,1,1,2
        555 ?.?#???..?.##... 4,2
        559 ?????##????# 9,1
        562 ???#???##.#? 5,2,2
        590 .?##..?##?. 3,3
        606 ??#.?#?#?????#?#?# 2,3,2,1,3
        660 ?#?#?..?#?#?? 4,3
        671 ..#.?##?.. 1,3
        701 ?.??..###???# 1,1,4,1
        705 ?#.?#.??#??#?..?? 2,1,4,2
        749 #???#?????##??? 12,1
        750 ??#?##?????.?# 1,4,3,1
        791 ..#.??.?#??? 1,2
        793 .??#.?????.?# 3,4,1
        798 #.?###?..?? 1,5,1
        808 #?#????..#?#? 3,2,4
        815 ??.#?#????# 1,1,6
        821 .?#?###.??#?#?#?. 6,7
        857 ##?.?.?.#???#?.# 3,1,5,1
        858 ?##????###???#?????? 2,8
        874 #.?#?????# 1,4,1
        918 ?##???#.?##?.?#? 3,3,3,1
        919 #????????.# 6,1,1
        932 #??.??????###?.?? 1,1,10,1
        936 ?.??????.?##?. 5,4
        940 ?#.#????#??#?.?? 2,10,1
        942 .#??#??????.#. 4,4,1
        948 ..????##?. 2,2
        949 ?#??.????##??#.??? 2,1,1,6,1,1
        959 .?#???.?#??? 5,2
        964 ???#??#.##. 2,1,2
        970 ??#?#?..?#??#??#. 5,7
        971 ???.???##?..##?##?.? 5,5
        980 ###????#?#?.. 3,6
        998 .##???.???#??## 2,1,2,1,2
        55 ?##?#???#?????? 4,6,2
        93 .??#???##?????###? 1,1,5,7
        108 ???.?????#?#???. 1,1,5,1,2
        135 ?#?###??#?? 7,2
        312 .#.??#?????#??#?? 1,3,10
        326 ...?...#??##? 1,5
        371 #??#??#???##?.#?#. 9,3,1,1
        447 ????#.?.??#???.? 4,1,6
        519 .#?..#.##?#??##??. 1,1,2,1,3,1
        591 .#??#??#????##?#?? 1,1,1,6,1
        710 ???..#.??#??#???..#? 1,1,1,9,1
        765 .?????..#.?##??#??? 5,1,2,3,1
        799 ?.?#?##?#.?..#??#? 6,1,4
        827 ..?.?###.? 1,3
        862 #?????.?#?.#???? 3,2,3,4
        866 ?.??.##?.?##? 2,3,2
        884 ??#?????###??#. 2,8,1
        911 ????#???##?? 7,2
        922 ?#???##???#?##???? 1,4,10
        11 ??#?##????#???..? 8,1,2,1
        30 ?#?.??#.?? 3,3,1
        84 .?#.#?#??#.??.. 2,3,2,1
        89 ?.????##.??#? 1,4,1,2
        101 ??????##????.??? 12,2
        105 ??#???????#???#?#.?? 16,1
        156 ?#?.#...#???? 1,1,2,1
        161 ?????##.?????. 1,4,5
        179 ?????..?##? 4,4
        190 ..???.#?##.? 2,4
        243 ?????#..#. 3,1,1
        288 .#????.??#?.#?? 5,1,1,1
        345 ??.?#??.?#?# 2,1,3
        360 ?##?????.???#??#? 2,7
        372 ?#??#??.#?#.#?? 6,1,1,1,1
        399 ?#??#??###?#?? 2,6,1,1
        406 ?.??#??#??# 1,1,5
        442 ??##??#?...?#####??? 8,8
        453 ??..?#.#??#? 1,2,1,1
        482 ?#????.?##?????? 4,9
        533 ???#??.?.??## 5,1,4
        560 ??????#..#??? 3,2,2,1
        609 ??#.???.???#??? 1,1,1,1,6
        617 ???.###???.??#??#? 5,6
        625 ?????.????#? 5,5
        635 .#???##????.???? 9,3
        654 .????.???.???? 2,1,3,3
        659 ?##???.??#? 4,1
        695 #????????#?.#???? 1,7,1,3
        696 ??.#???#.???. 1,5,3
        697 .#??#??#???##?#??? 2,2,1,8
        724 ?.???????#.?#? 1,8,2
        727 ?#?????##?#??#.?## 4,7,3
        728 ??#?????#.??#? 5,2,4
        735 ?#????#?????#??? 12,1
        736 ?#???.#??.. 3,3
        764 ???###?.??#? 6,1,1
        776 ??.##???## 1,3,3
        818 ????.???#???.?.? 3,1,5,1,1
        833 .???#..?????? 2,5
        841 .?###.#..#??.?? 4,1,3,1
        876 ???#???#????#? 5,6
        933 .??.##???? 1,5
        973 ?????##.?# 2,3,2
        979 ?.???????#?##? 1,11
        999 #???##?#???#?????.?? 17,1
        21 ##?##????? 5,1,1
        36 ?#.???#?????#?#??? 2,1,2,4,2,1
        46 ?#.?#??#???...?# 2,1,2,1,1
        51 ??????#.??#???. 5,4
        70 .?#???.#.?..# 3,1,1,1
        72 ??#???.#??. 3,2
        76 ?##?????##?????# 3,2,8
        97 #?????.????##?.????? 1,4,2,3,5
        122 .?#?#??#??????# 8,2,1
        153 ##?#??##???? 4,2,1
        182 ???###??.# 5,1
        232 ?.????????.# 6,1
        259 #.??????????????#? 1,12,2
        273 ???#?##??????#.?? 6,1
        302 ?????#?#??#??#?##? 3,13
        310 .#?##?.???#?#?? 4,5
        314 ???##.?..???#???? 3,3
        320 ?????.?#.???????#?# 2,1,2,8,1
        346 ?.#???#???.???#.? 1,4,4
        359 ?????#????.#??#?#?# 7,1,8
        388 ?##????????# 5,3,1
        392 ???#?#?#.???# 4,1,1,1
        416 ..??#?????#?.. 6,2
        434 #?#???#??#??? 7,2,1
        438 ?#???..?#.. 2,1,1
        461 ?????##???#???#.?. 7,5
        478 .??#??.??### 2,1,3
        481 ?#??...?.. 2,1
        508 ?.??#??..#? 3,2
        529 ???#???#?#??#?? 8,3
        530 ??#?#???#???#?????. 2,2,1,2,3
        549 #?.#?#???#??? 1,1,1,3
        556 .?????#.?#????#???? 6,4,4
        571 #.??#???##???#??##?? 1,16
        573 ???#??.?.#?? 4,1
        594 ..???#??????#???. 5,7
        611 #?#.?????. 3,1,2
        618 #.???.#?#????? 1,1,8
        638 ?.?#????.????????# 1,3,1,1,2,4
        656 ##???????#. 2,3,1
        658 .??##??.##? 4,2
        677 ?#?##???????.????#?# 4,1,3,3,1,1
        712 ??????#?.??# 1,5,1,1
        717 ??##???#?##???.. 3,8
        753 ??#?.???#????#??. 1,9
        761 #.???#.??? 1,3,1
        768 #.?#..?#???? 1,1,1,1
        873 ??#????##. 3,2
        875 .????##?????#??#?. 11,2
        888 ?.#???????#??. 1,3
        906 ?#.?.??#???? 1,5
        907 ??#.??####??? 2,6
        908 ?#?#?????.??.#.#.# 6,1,2,1,1,1
        920 ???###.#????#????#?? 6,2,2,5
        937 ?##??????.? 3,4
        954 ????###??###??##? 11,3
        981 ??####.??#???????# 6,4,4
        984 ??#?.????.??#? 4,1,1,4
        997 ?.#?????#??.?? 1,5
        98 .#??.????.. 2,2,1
        111 ?????#????.???# 1,4
        268 ??#?##?????.? 2,7
        284 ???????.###? 1,2,2,3
        355 #??????.????? 5,2,2
        31 ??#?#?.????#??#.. 1,1,2,1,5
        35 ??#?..???? 1,3
        64 .###.##?.?#??.?? 3,2,2,2
        71 ??#???#?.?. 5,1
        163 ???.?..#?.? 2,2
        178 #????.?#??.##?????. 3,2,7
        188 #???#??#??..??###? 2,1,4,4
        196 ..??##?..????..? 4,4,1
        214 ????#??#?..??# 6,2
        216 #?.??##??.?#?? 2,3,1
        343 ?.#???#??? 5,1
        351 ???.???#?? 2,1,4
        356 .??#?#??????#.?.?? 12,1,1
        363 ??#??????? 1,5,1
        387 ????#??..#?.?? 4,1,2,1
        398 .?.#??????##? 1,3,1,2
        412 .#.??.?????????.??? 1,1,1,7,3
        452 ??#??????#? 1,4
        465 ?#?#??.??? 3,2
        499 ?.#?.??.??# 1,2,1
        501 ??#????.#??? 1,5,1,1
        507 ??.???##?? 1,1,5
        548 ????..??#? 3,1
        561 ..???.??#?.?????? 1,1,3,6
        589 ?.???#.??.. 1,3,1
        610 ???#???????## 1,1,1,6
        640 .??.??##?????##.?. 2,3,4,1
        679 ?.?..#????#? 1,6
        725 #????#??????##..??. 10,2,1
        796 ??...##?#?.? 1,4
        843 ?#??#???????##.#.?? 8,1,3,1,1
        924 ?#?##?..??#?. 6,2
        950 #???#??#?#.?? 3,2,3,1
        0 ??????#??? 7,1
        14 ??###??##?????.#? 10,1
        45 ????##.?#.?????.?##? 1,2,2,3,1,2
        48 ??#???#??#####????? 4,13
        60 ???????.#????? 3,2,4,1
        173 ????????#.#???### 2,1,3,2,4
        263 ...#????????#????. 1,10
        391 ?????##?.? 1,5,1
        395 ??????###???#???.#? 14,1
        462 ????????#?#????####? 1,17
        532 .?##?####?##.???? 3,4,2,2
        569 ?????...?#?# 1,2,4
        587 #..????#??##???..#?# 1,8,1,1,1
        622 #?.#?????#.#???????? 2,1,4,1,4,1
        652 ??????..?..#?. 4,2
        681 ??#????????#? 6,5
        682 ???.?#??#? 3,2,2
        770 .?#?#?..?...?#??.??? 5,1,4,1
        813 ??????.???..???? 6,1,1,1,1
        829 ??????##?#??#????.?. 9,5
        830 ..???.#?###. 1,5
        889 #??.???##??.?#???? 3,2,2,1,3,1
        909 ??.##??#?#?????????? 2,9,4,1
        930 ?..#???#????? 1,1,5,1
        128 ??.#?????####.# 1,2,1,4,1
        172 ??#??.????#.??#?# 4,1,2,1,1
        281 ?#?..???#?#???#?# 2,1,7,1
        305 #..?????????? 1,4,2,1
        341 #.??.?#????##?#? 1,1,2,4,1
        459 #???..???.??#.???? 1,1,1,1,2,3
        471 ????????.??#.##?#??? 1,1,3,3,7
        476 .??????#?#????. 4,4
        494 ???##?.????#?. 3,2
        572 ??#??#?.?#.?#?? 5,1,3
        576 #???.?????##?##?#? 1,1,12
        597 .??#???#??? 2,2
        619 ??.??????.#?##??##? 3,9
        661 .????#??????..## 6,1,1,2
        684 ??#??.?#?. 2,2
        722 ?#??.?.?#?#??? 2,6
        754 ..???#?..?##???. 4,4
        789 .?..??.??.#####?? 1,1,1,7
        823 .??.?.????. 2,1,2
        867 #??????##??#. 1,1,5
        935 .????#???#??.## 2,3,3,2
        974 ?????#??#?.?# 1,1,5,1
        990 #?#???????????? 1,6,1,1,1
        992 .???##?.?#?? 4,3
        25 .??.?.#??????#????. 1,1,11
        43 .#?????#.? 1,1,2
        81 .???.?###???? 1,1,7
        96 ??????#.???#??? 4,1,1,2,2
        168 ??#??#???? 1,1,4
        213 ??#??.??#??#?.#??#? 4,1,5,1,1
        271 ??????#.?#..???#? 2,3,1,2,1
        272 ?.???.??#. 2,1
        279 ?#???.#??.?#?? 5,1,2
        301 .??#????#??#?.?.. 10,1
        369 ??##???#.?? 3,1,2
        386 ???????##???.??????? 11,1,2,2
        583 ????.??#?? 4,2
        646 ??.?#?#???????#?#? 3,8
        792 ??.##??????.?.? 7,1
        947 #?????#?.?????#? 2,1,3,6
        951 ??.?...?.??## 1,1,1,3
        957 ??##?#????##???.? 1,13
        123 ???##?...?.????###? 2,3,1,6
        277 ????????#?# 1,2,5
        432 ???..#?#??.???? 1,1,4,1,1
        436 ??#????##??#? 3,2,3
        454 ???...?#??#?#?? 1,9
        491 ?#.???...#?##?#..? 1,1,6
        528 .??.??#?###???##??? 2,11
        812 .?.??###?#?#??####?? 1,14
        871 ????????#?##?. 4,7
        894 ??????#??#??#??#?. 11,2
        945 ?##?..???#?.? 4,1,1
        956 ??????#.?#??.?# 1,3,2,1,1
        496 ?#???????. 1,4,1
        52 .????.#??.?#???#?#? 1,3,7
        104 ?????#?##?#?? 1,1,5
        147 ?#??#??#??###?????? 13,1,2
        162 ?#????#??.?#.##.?? 2,1,3,1,2,2
        195 ???#??#.?.? 4,1
        198 ????.?#???... 1,5
        203 ?#?###.???? 5,1
        275 ?##?.#.??? 3,1,2
        357 ???#??#??#.?#?###??? 1,1,1,1,8
        365 ..??.?#?#.?#? 1,4,2
        397 ???#?..??? 4,2
        402 ?????#.?#??? 1,3,4
        456 ??.??.###?? 1,4
        531 ?#?#?.?#??#??? 4,1,1,1
        751 ?????.??#?#????#???. 2,13
        806 ?####?##?????. 7,1
        905 ..??.?##?????? 1,8
        921 ???????.???????#?# 1,1,2,10
        925 ?#???#?.???#??#?? 2,1,8
        82 ?##.???.?#?#?????? 2,2,5,4
        127 ?.????##?? 1,2,2
        415 ##???.?#?##??. 3,1,6
        608 .????.?#??#?????? 3,5,5
        667 .?..?.##??.??????## 1,2,1,7
        745 ??.??#????#?..?.? 2,1,7,1
        923 ???##???.#???#???.? 6,1,1,5
        208 ##?#????????#? 2,4,1,2
        251 ???????#?.?# 3,3,1
        336 .??#??#???##? 6,3
        373 ?????????.#??#?? 5,2,3
        419 #??#???.???#?. 1,1,1,3
        488 ?.?..??##??###?#? 1,10
        557 ???##?###????#?. 8,3
        580 ?#??????#. 2,1,2
        672 ?#.#?.??????#??###? 2,1,2,3,3
        800 ###.?????#? 3,1,3
        895 #??#?????. 1,3,1
        901 #.##?????.????##??? 1,7,1,3
        963 .??????#.##?#?# 1,1,6
        5 ???#?##?????.#?? 1,8,2
        17 ..?.???#??##???##.?? 1,10
        286 ??#?##?##????#?.???? 4,7,2
        324 ????#..??#? 2,1,1
        393 .???#???.??? 1,4,3
        409 ???????#.?? 2,3,2
        413 ..##?.??????????#.? 3,6,1,1,1
        523 ?????##?#. 1,1,4
        674 ???????#??##??.??#. 3,6,1,2
        709 ???.?.??#?? 3,3
        738 .?.??.??##????.? 2,6
        743 ?.#.???###??#?.#???? 1,1,1,4,1,4
        782 ??##?????? 2,3
        783 ?#??????.?##? 1,2,1,4
        842 .?.??????#?????##??. 1,13,1
        327 .?????##??##???? 2,3,7
        450 .???#???#?#???? 4,4,3
        655 ??#??#???????.???#. 1,8,1,3
        978 ..?.?###?##?#?????.. 1,11
        24 ?#??##?#?..???#?.?? 8,1,1,2
        29 ??.#???????...??.? 1,4,1,1,1,1
        34 .?.?.?##??.??# 1,4,1
        38 ?#??.???###?. 3,6
        102 ??#?#????? 2,1,1
        136 ??.?#?.?#??#??. 1,2,7
        144 ?#??.?#???## 2,2,3
        205 .#.??###?????##? 1,4,5
        219 ????.##????? 1,5
        258 ?????#????#.?. 1,6
        353 ??#??#??#???..? 7,1
        384 ??...#????#??##?#?? 1,1,11
        457 ?#???.?????????? 4,9
        504 ???.?.?##? 2,1,3
        527 .??#????#.????.??. 7,3,1
        535 ?????##..??? 1,4,2
        584 ???#?#?.#.?.??? 6,1,1,2
        670 ?###??.?#?##?#????# 5,7,3
        704 .???.???#?#?#?? 2,9
        711 ???##??.#?.???##..?? 1,2,2,5,1
        795 ?.???.###?.? 1,4
        820 ????##??????#?? 2,4,1,2,1
        855 ??#??.?#..?.##??? 4,2,1,2,1
        885 ?#????#?#?#????.?? 5,1,1,1,2,1
        896 .?#???.#???.#? 4,1,1,1
        972 ?#??..?###??? 3,4
        976 ?#?????????????#???. 10,2
        59 #?????..??.#?? 1,1,2,3
        212 ?????.##?#. 1,4
        595 ???????#?##? 1,1,1,5
        100 ???????.?#? 4,1,1
        206 #?#??.???.??? 1,1,3,1
        230 ?#???????? 1,3,2
        329 ???#????##???? 1,10
        377 ..????.#??. 1,1,2
        383 ???..#??#??.#?????? 5,3,1
        400 ?????#???? 5,2
        403 ..??.#????#??? 2,2,2
        551 ?????.???#?. 3,1
        592 ?.?#?.???#?#??..??# 1,3,6,1
        721 #?##????#??.??.? 11,1
        752 .#?????#??? 1,1,5
        819 ???#?#???? 1,1,3
        882 ##??.????##? 3,1,2
        927 .?????.?#.???#?? 1,3,1,1,3
        965 ####?#?????#???? 8,5
        32 ?#?..??##???##???? 2,11
        103 .?????#??#.?.??###?# 3,1,1,1,5
        252 .?????#?#?#??#.????# 1,1,1,6,2,1
        444 ??#??.?#??##???#? 3,6,1
        464 .?????#??#???##?#??. 7,6
        485 #????..?#.?#??.# 1,1,2,3,1
        554 ?????.???#??#??## 4,1,2,5
        585 #..????..?#?.????# 1,2,1,2,1,1
        657 ?#?.??#??????.#??? 2,6,3
        689 ??#.?#?????#?????. 3,5,1,1,1
        706 ??????##??..?##?##?? 7,6
        757 ?#??#???#.???#???? 5,2,3
        828 #??#?#.????? 6,1,1
        853 ??##??..?.?#? 4,2
        926 #..?#?#?..?.?.?.#?? 1,4,1,1,3
        941 #?#????????.??#?? 1,1,2,3,4
        943 ?????.#?## 1,1,4
        817 #????#???? 3,3
        934 .?.??.#.??#.? 1,1,1,3
        26 ?.??#????. 1,5
        87 ??.#.??.#??##???#. 1,1,1,1,6
        109 ????.##?.?# 1,3,2
        256 ??.##?????.?#.?? 4,1,1
        283 .?#.#?#?.?..#????? 2,1,1,1,2,1
        435 ???????#???????? 3,6,4
        647 ???#???#?.#???.#??. 1,1,3,1,2,3
        744 .??###???.??#????? 8,1,1,2
        756 #???#?#?#???.?????? 2,7,5
        784 #?#?#??.?????# 7,1,1
        797 ?.???.#?.. 1,1
        859 .??###??.? 5,1
        40 .?##?##???? 6,1
        280 .????###?? 1,4
        615 .????##???#?. 1,7
        686 ..??#??????#????? 2,9
        826 ????????#????? 1,1,2,6
        900 ..??#????#????? 2,7
        293 ?.???#???? 1,1,4
        534 .??.??#??? 2,1,1
        680 .#?.?.???????? 1,1,5,1
        466 .?..?.??..##.?. 1,1,1,2,1
        75 ????.#.?#? 2,1,2
        171 ??#???.#????###?? 4,3,5
        247 #???????#???????? 1,11,1
        414 #?#??????#..??? 1,2,1,1,2
        546 ???##?????..?????# 9,2,1
        623 ??????...?# 1,2,1
        985 ?????..?#?? 3,3
        50 .??????.??.?#??#?? 1,1,1,2,4,1
        299 ??#?#?#?????.? 10,1
        300 ??.?..?#?#???##? 1,1,1,1,4
        304 ?#??.?..?. 3,1
        321 ??.??##???#.???# 1,4,2,2
        385 ????##??###?#?????.? 3,10
        433 ??.?#??..???? 1,1,1,3
        577 ????.???##?.? 3,5
        653 ????..??.????????.?? 1,2,1,1,6,1
        693 ???.##.????. 2,1
        699 #?????.#???#?.#????? 4,1,3,3,1
        742 #?.?##??#??#?????. 2,6,4,1
        811 ???..####.?.??? 4,1
        860 ?.????#????? 1,8
        226 ????#.??#????#??? 5,3,3,1
        449 ..???.??.#???#??? 1,1,8
        477 #.???#??????#?? 1,1,2,6
        598 ?##..?????#? 3,1,2
        861 #..#??.????.#?? 1,3,1,1,1
        423 ?.??#???.??# 3,1,1,1
        578 ?????#????#???# 4,4,2
        713 #???.????.? 4,2                
        """.Split("\r\n");


    }
}
