using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {

        static List<string> GetSpaceDelimDigitsAsListString(string l) => Regex.Replace(l, " {2,}", " ").Trim().Split(" ").ToList();
        static List<int> GetSpaceDelimDigitsAsListInt(string l) => GetSpaceDelimDigitsAsListString(l).Select(n => int.Parse(n)).ToList();
        static List<long> GetSpaceDelimDigitsAsListLong(string l) => GetSpaceDelimDigitsAsListString(l).Select(n => long.Parse(n)).ToList();

    }
}
