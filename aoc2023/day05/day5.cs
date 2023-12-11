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
        class MapPart { 
            public MapPart(string s) 
            { 
                var vals = GetSpaceDelimDigitsAsListLong(s);
                destStart = vals[0];
                srcStart = vals[1];
                Range = vals[2];
            }
            public long destStart { get; set; }
            public long srcStart { get; set; }
            public long Range { get; set; }

            public bool ContainsSrc(long s) => s >= srcStart && s <= srcStart + Range - 1;
            public long GetDest(long s) => ContainsSrc(s) ? destStart + s-srcStart: -1;
        }
        class MapSet
        {
            public List<MapPart> xmapList { get; set; } = new List<MapPart>();
            public Dictionary<long, long> map { get; set; } = new Dictionary<long, long>();

            public long GetMatch(long s) => xmapList.Where(x => x.ContainsSrc(s)).FirstOrDefault()?.GetDest(s) ?? s;

            public void MapAdd(long s) => map.Add(s,GetMatch(s));

            public void ClearMap() => map.Clear();
        }
                
        static List<MapPart> GetMapParts(ref int i, string[] d)
        {
            var mx = new List<MapPart>();
            do
            {
                i++; mx.Add(new MapPart(d[i]));
            } while (i + 1 < d.Length && d[i + 1].Length > 0 && Char.IsDigit(d[i + 1][0]));
            return mx;
        }


        static void LoadSeedMap(ref List<long> seed, Dictionary<string, MapSet> sd, string[] d)
        {
            for (int i = 0; i < d.Count(); i++)
            {
                var l = d[i];
                if (l.StartsWith("seeds: "))
                {
                    seed = GetSpaceDelimDigitsAsListLong(l.Split(":")[1]);
                }
                else if (l.StartsWith("seed-to-soil map:"))
                {
                    sd["mSeedSoil"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("soil-to-fertilizer map:"))
                {
                    sd["mSoilFert"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("fertilizer-to-water map:"))
                {
                    sd["mFertWatr"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("water-to-light map:"))
                {
                    sd["mWatrLght"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("light-to-temperature map:"))
                {
                    sd["mLghtTemp"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("temperature-to-humidity map:"))
                {
                    sd["mTempHumd"].xmapList = GetMapParts(ref i, d);
                }
                else if (l.StartsWith("humidity-to-location map:"))
                {
                    sd["mHumdLoct"].xmapList = GetMapParts(ref i, d);
                }
            }

        }

        static void BuildSeedMap(IEnumerable<long> seed, Dictionary<string, MapSet> sd) 
        {
            foreach (var sm in sd.Values) sm.ClearMap();

            foreach (var s in seed) sd["mSeedSoil"].MapAdd(s);
            foreach (var s in sd["mSeedSoil"].map.Values.Distinct()) sd["mSoilFert"].MapAdd(s);
            foreach (var s in sd["mSoilFert"].map.Values.Distinct()) sd["mFertWatr"].MapAdd(s);
            foreach (var s in sd["mFertWatr"].map.Values.Distinct()) sd["mWatrLght"].MapAdd(s);
            foreach (var s in sd["mWatrLght"].map.Values.Distinct()) sd["mLghtTemp"].MapAdd(s);
            foreach (var s in sd["mLghtTemp"].map.Values.Distinct()) sd["mTempHumd"].MapAdd(s);
            foreach (var s in sd["mTempHumd"].map.Values.Distinct()) sd["mHumdLoct"].MapAdd(s);

        }

        static long GetMinSeedLocation(IEnumerable<long> seed, Dictionary<string, MapSet> sd)
        {
            foreach (var sm in sd.Values) sm.ClearMap();

            long lm = 999999999999;
            foreach (var s in seed)
            {
                var l = sd["mSeedSoil"].GetMatch(s);
                l = sd["mSoilFert"].GetMatch(l);
                l = sd["mFertWatr"].GetMatch(l);
                l = sd["mWatrLght"].GetMatch(l);
                l = sd["mLghtTemp"].GetMatch(l);
                l = sd["mTempHumd"].GetMatch(l);
                l = sd["mHumdLoct"].GetMatch(l);

                if (l<lm) lm = l;
            }

            return lm;
        }


        static void day5()
        {
            List<long> seed = new List<long>();
            Dictionary<string, MapSet> seedMaps = new Dictionary<string, MapSet> {
                {"mSeedSoil", new MapSet()},
                {"mSoilFert", new MapSet()},
                {"mFertWatr", new MapSet()},
                {"mWatrLght", new MapSet()},
                {"mLghtTemp", new MapSet()},
                {"mTempHumd", new MapSet()},
                {"mHumdLoct", new MapSet()}
            };

            var d = d5_data;
            
            LoadSeedMap(ref seed, seedMaps, d);

            BuildSeedMap(seed, seedMaps);

            Console.WriteLine($"Answer1: {seedMaps["mHumdLoct"].map.Values.Min()}");

            var minList = new List<long>();
            for (int i = 0; i < seed.Count; i += 2)
            {
                Console.WriteLine($"  ->Processsing in seed range : {i}: {seed[i]}-{seed[i]+seed[i+1]-1}");
                var xseed = new long[seed[i + 1]];
                for (long j = 0; j < seed[i + 1]; j++) xseed[j] = seed[i] + j;
                //BuildSeedMap(xseed, seedMaps);                
                //minList.Add(seedMaps["mHumdLoct"].map.Values.Min());
                minList.Add(GetMinSeedLocation(xseed, seedMaps));
                Console.WriteLine($"    ->min : {minList.Last()}");
            }

            Console.WriteLine($"Answer2: {minList.Min()}");
        }

        static string[] d5_data0 =
        """
        seeds: 79 14 55 13

        seed-to-soil map:
        50 98 2
        52 50 48

        soil-to-fertilizer map:
        0 15 37
        37 52 2
        39 0 15

        fertilizer-to-water map:
        49 53 8
        0 11 42
        42 0 7
        57 7 4

        water-to-light map:
        88 18 7
        18 25 70

        light-to-temperature map:
        45 77 23
        81 45 19
        68 64 13

        temperature-to-humidity map:
        0 69 1
        1 0 69

        humidity-to-location map:
        60 56 37
        56 93 4
        """.Split("\r\n");


    }
}
