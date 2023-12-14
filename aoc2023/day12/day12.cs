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
            public c12Rep(string s, bool part2On = false, bool exclCompleteDamage = false)
            {
                LineRaw = s;
                var sl = s.Split(' ');


                var ichk = sl[1].Split(',').Select(d => int.Parse(d)).ToList();
                chk = ichk.ToList();


                LineOrig = sl[0];
                LineAdj = LineOrig;
                if (exclCompleteDamage)
                {
                    Regex regEx;
                    string cs;
                    for (int i = 0; i < ichk.Count; i++)
                    {
                        cs = new string('#', ichk[i]);

                        //var regEx = new Regex($"\\.{cs}\\.");
                        //if (regEx.IsMatch(LineAdj))
                        //{
                        //    LineAdj = regEx.Replace(LineAdj, ".", 1);
                        //    chk.Remove(ichk[i]);
                        //    continue;
                        //}

                        //regEx = new Regex($"^{cs}\\.");
                        //if (regEx.IsMatch(LineAdj))
                        //{
                        //    LineAdj = regEx.Replace(LineAdj, ".", 1);
                        //    chk.Remove(ichk[i]);
                        //    continue;
                        //}

                        regEx = new Regex($"\\.{cs}$");
                        if (regEx.IsMatch(LineAdj))
                        {
                            LineAdj = regEx.Replace(LineAdj, "..", 1);
                            chk.Remove(ichk[i]);
                            continue;
                        }
                    }
                }
                LineAdj = Regex.Replace(LineAdj, "\\.{3,}", ".");
                //LineAdj = Regex.Replace(LineAdj, "\\.{2,}", ".").Trim('.');


                if (part2On){ var cx = chk.ToList();  for (int i = 0; i < 4; i++) chk.AddRange(cx.ToList()); }
                if(part2On) LineAdj = repeatString(LineAdj, 5);

                data = LineAdj.Select((x, index) => new c12DataCell(index, x, this)).ToList();
                
                



                //InitData
                //initData_ExistingCompleteDamage();
                //initData_RemoveCompleteDamage();
                //initData_SetInitialDamageGrouping();
                initData_UnknownGroups();







            }
            public string LineRaw { get; set; }
            public string LineOrig { get; private set; }
            public string LineAdj { get; private set; }
            public List<c12DataCell> data { get; set; }
            List<c12DataCell> dataNoDot => data.Where(d => !d.IsDot).ToList();
            public List<int> chk { get; set; }

            public List<c12Group> IniDmgGrps { get; set; }
            public int Permutations { get; set; }

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


                        if (ick.Count==0)
                        {
                            Console.WriteLine("error!!0");
                        }
                        else if (dg.Count< chk.Min())
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
                        else if (i == IniDmgGrps.Count-1)
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

            string minPossible => string.Join(".", chk.Select(c => new string('#',c))).PadRight(data.Count,'.');

            public bool IsAllDmg => dataNoDot.All(x => x.IsDmg);
            public int DmgCnt => dataNoDot.Count(x => x.IsDmg);
            public int DmgMissing => chkSum - DmgCnt;

            public bool IsAllUnk => dataNoDot.Where(d=>!d.IsDot).All(x => x.IsUnk);
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


            public int getSolutions0()
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
                            while (ixf!=null && ixf.GetPrev!=null && ixf.GetPrev.s=='#') 
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
                                    pre  = ipre,
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

                        var perms= getPerm(ugp, "", rxp, dg.GrpChk.Sum());
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


            public int getSolutions1()
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



                    if (UnkGrpData.Count==1)
                    {
                        var ug = ugp.First();
                        Permutations = ug.prms.Select(g=> ug.pre + g + ug.post).Count(x=> Regex.IsMatch(x, rxp));
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

            public int getSolutions2()
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
                    Permutations = prms.Select(g=> convertPermData(g)).Count(x => Regex.IsMatch(x, rxp));

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
            public int getSolutions3()
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
                    var giLst = minPossible.Trim('.').Select((c,index)=>new { idx = index, chr= c }).Where(c=>c.chr=='.').Select(c=>c.idx).ToList();giLst.Insert(0, 0);
                    prms.Add(minPossible);
                    for(var i = 0; i< giLst.Count; i++)
                    {
                        var dcx = giLst.DifferentCombinations(i+1).Select(x => x.ToList()).ToList();
                        for(int gi= 0; gi<dcx.Count; gi++) 
                        {
                            for (int j = 1; j <= slack/dcx[gi].Count; j++)
                            {
                                prms.Add(getModData(dcx[gi], j));
                            }
                        }
                    }

                    var chrChecker =(string sChk,string sWth)=>{ 
                        for(var i=0; i< sChk.Length; i++)
                        {
                            if (sWth[i] != '?' && sChk[i] != sWth[i])
                                return false;
                        }
                        return true; 
                    };

                    var rxp = getRxPattern(chk);
                    Permutations = prms.Count(x => chrChecker(x,LineAdj));
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

            public int getSolutions()
            {

                List<string> prms = new List<string>();
                loadPerms(prms);

                Permutations = prms.Count(x => pramChecker(x, LineAdj));
                return Permutations;
            }

            bool pramChecker(string sChk, string sWth)
            {
                for (var i = 0; i < sChk.Length; i++)
                {
                    if (sWth[i] != '?' && sChk[i] != sWth[i])
                        return false;
                }
                return true;
            }


            public void loadPerms(List<string> outs, List<int> groups = null, int slack = -1, string prev = null)
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
                    loadPerms(outs, groups.Skip(1).ToList(), slack, prev);
                }
                do
                {
                    cur.Insert(groups.First() + 1, '.');
                    cur.RemoveAt(cur.Count - 1);
                    slack -= 1;
                    var plast = new string(cur.ToArray());
                    if (pramChecker(plast, LineAdj))
                        outs.Add(plast);
                    for (int j = 1; j < groups.Count; j++)
                    {
                        groups[j]++;
                    }
                    if (groups.Count > 1 && slack > 0)
                    {
                        loadPerms(outs, groups.Skip(1).ToList(), slack, plast);
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
                    loadPerms(outs, groups.Skip(1).ToList(), slack);
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
                        loadPerms(outs, groups.Skip(1).ToList(), slack);
                    }
                } while (slack > 0);

            }

            string getModData(List<int> ig, int len)
            {
                var oc = minPossible.ToList();

                for (int i = ig.Count-1; i >= 0; i--)
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
                char[] rd = data.Select(d=>d.s).ToArray();
                var i = 0;
                foreach(var dc in data.Where(d=>d.IsUnk))
                {
                    rd[dc.id] = iul[i];
                    i++;
                }
                return new string(rd);
            }
            class permDataGrp { 
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
                        if (rst.Count(s=>s=='#') <= maxDmg && Regex.IsMatch(rst, rxp))
                        {
                            perms.Add(rst);
                        }
                    }
                }
                return perms;
            }

            public override string ToString() => $"[{new string(data.Select(x =>x.s).ToArray())}]" +
                $"   [{string.Join(',', chk)}] " +
                $"| prm={Permutations.ToString("#0.0")}";

            //public override string ToString() => $"[{string.Join('|', IniDmgGrps.Select(x =>
            //    new string(
            //        x.grpData.Select(d => d.s).ToArray())))}]" +
            //    $"   [{string.Join(',', chk)}] " +
            //    $"| prm={Permutations.ToString("#0.0")}";
        }

        static void day12()
        {
            var d = d12_data;


            var map = d.Select(d => new c12Rep(d,false,false)).ToList();

            foreach (var dr in map)
            {
                dr.getSolutions();
                Console.WriteLine(dr);
            }

            Console.WriteLine($"Answer1: {map.Sum(d => d.Permutations)}");
            
            
            
            map = d.Select(d => new c12Rep(d,true,false)).ToList();

            foreach (var dr in map)
            {
                dr.getSolutions();
                Console.WriteLine(dr);
            }


            Console.WriteLine($"Answer2: {map.Sum(d => d.Permutations)}");



        }


        static string[] d12_data0 =
        """                
        ???.### 1,1,3
        .??..??...?##. 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """.Split("\r\n");
        static string[] d12_data0b =
        """       
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


    }
}
