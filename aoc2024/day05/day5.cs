using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace aoc2024
{
    internal partial class Program
    {
        class c5rule
        {
            public c5rule(List <int> plst)
            {
                P1 = plst[0];
                P2 = plst[1];
            }

            public int P1 { get; set; }
            public int P2 { get; set; }
        }

        class c5ruleSet
        {            
            public c5ruleSet(List<c5rule> rules)
            {
                RuleList = rules;
                PageRuleDict = rules.Select(x=>x.P1).Distinct().ToDictionary(x => x, y=>rules.Where(r=>r.P1==y).Select(y=>y.P2).ToList());
            }           
            
            public List<c5rule> RuleList = null;

            public Dictionary<int,List<int>> PageRuleDict = null;

            public bool CheckPages(List<int> pgList)
            {
                for (int i=1; i<pgList.Count; i++)
                {
                    var pg=pgList[i];
                    if (PageRuleDict.ContainsKey(pg))
                    {
                        if (pgList.Take(i).Any(x=> PageRuleDict[pg].Contains(x)))
                        {
                            return false;
                        }
                    }
                }
                
                return true;
            }

            public List<int> ReorderPages(List<int> pgList)
            {
                var ol = pgList.ToList();
                
                for (int i = 1; i < pgList.Count; i++)
                {
                    var pg = ol[i];
                    if (PageRuleDict.ContainsKey(pg))
                    {
                        int? bx = ol.Take(i).Where(x => PageRuleDict[pg].Contains(x)).FirstOrDefault();
                        if (bx!=null && bx>0)
                        {
                            var si = ol.FindIndex(x => x == bx);
                            ol.RemoveAt(si);
                            ol.Insert(i, bx??0);
                            i--;
                        }
                    }
                }

                return ol;
            }

        }

        static void day5()
        {           
            Console.WriteLine($"Answer1: {day5LogicPart1()}");
            Console.WriteLine($"Answer2: {day5LogicPart2()}");
        }

        

        static int day5LogicPart1() 
        {
            var rules = new c5ruleSet(d5_data_rules.Select(x => new c5rule(GetPipeDelimDigitsAsListInt(x))).ToList());

            var sum = 0;
            foreach (var s in d5_data)
            {
                var l = GetCommaDelimDigitsAsListInt(s);
                if (rules.CheckPages(l))
                {
                    sum+= l[l.Count/2];
                }
            }

            return sum;
        }

        static int day5LogicPart2()
        {

            var rules = new c5ruleSet(d5_data_rules.Select(x => new c5rule(GetPipeDelimDigitsAsListInt(x))).ToList());

            var sum = 0;
            foreach (var s in d5_data)
            {
                var l = GetCommaDelimDigitsAsListInt(s);
                if (!rules.CheckPages(l))
                {
                    sum += rules.ReorderPages(l)[l.Count / 2];
                }
            }

            return sum;
        }

        static string[] d5_data0 =
        """        
        75,47,61,53,29
        97,61,53,29,13
        75,29,13
        75,97,47,61,53
        61,13,29
        97,13,75,29,47
        """.Split(Environment.NewLine);

        static string[] d5_data_rules0 =
        """        
        47|53
        97|13
        97|61
        97|47
        75|29
        61|13
        75|53
        29|13
        97|29
        53|29
        61|53
        97|53
        61|29
        47|13
        75|47
        97|75
        47|61
        75|61
        47|29
        75|13
        53|13
        """.Split(Environment.NewLine);

    }
}
