using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {
        class pcCameCard 
        {
            Dictionary<char, int> cv = new Dictionary<char, int> { {'A',14}, {'K',13}, {'Q',12}, {'J',11}, {'T',10}, {'9',9}, {'8',8}, {'7',7}, {'6',6}, {'5',5}, {'4',4}, {'3',3}, {'2',2} };
            public pcCameCard( char c, bool useJokers) { Code = c; Value = c=='J' && useJokers ? 1 : cv[c]; }
            public char Code { get; set; }
            public int Value { get; set; }
        }
        class pcCamelHand
        {
            public pcCamelHand(string s, bool useJokers = false)
            {
                var sl = GetSpaceDelimDigitsAsListString(s);
                Cards = sl[0].Select(c => new pcCameCard(c,useJokers)).ToList();
                Bet = long.Parse(sl[1]);
                UseJokers= useJokers;
            }

            Dictionary<int, int[]> scores = new Dictionary<int, int[]>
            {
                {1 ,new int[]{ 1, 2, 4, 6, 7, 7 } }, //"hc"=1
                {2 ,new int[]{ 2, 4, 6, 7, 7, 7 } }, //"1p"=2
                {3 ,new int[]{ 3, 5, 6, 7, 7, 7 } }, //"2p"=3
                {4 ,new int[]{ 4, 6, 7, 7, 7, 7 } }, //"3k"=4
                {5 ,new int[]{ 5, 6, 7, 7, 7, 7 } }, //"fh"=5
                {6 ,new int[]{ 6, 7, 7, 7, 7, 7 } }, //"4k"=6
                {7 ,new int[]{ 7, 7, 7, 7, 7, 7 } }  //"5k"=7
            };

            public List<pcCameCard> Cards = new List<pcCameCard>();
            public long Bet {  get; set; }
            public bool UseJokers;
            public string CardView => string.Join("", Cards.Select(c => c.Code));
            public int rank => UseJokers ? rankWithJokers : rank5;


            int[] cardInt =>Cards.Select(c => c.Value).ToArray();
            int[] cardIntNoJokers => Cards.Where(c=>c.Code!='J').Select(c => c.Value).ToArray();
            int jokerCnt => Cards.Where(c => c.Code == 'J').Count();
            bool hasJoker => jokerCnt > 0;


            int rank5 =>
                  cardInt.Distinct().Count() == 1 ? 7 //5k

                : cardInt.Distinct().Count() == 2 && cardInt.GroupBy(x => x).Max(x => x.Count()) == 4 ? 6 //4k
                : cardInt.Distinct().Count() == 2 && cardInt.GroupBy(x => x).Max(x => x.Count()) == 3 ? 5 //fh

                : cardInt.Distinct().Count() == 3 && cardInt.GroupBy(x => x).Max(x => x.Count()) == 3 ? 4 //3k
                : cardInt.Distinct().Count() == 3 && cardInt.GroupBy(x => x).Max(x => x.Count()) == 2 ? 3 //2p

                : cardInt.Distinct().Count() == 4 ? 2 //1p
                : cardInt.Distinct().Count() == 5 ? 1 //hc

                : 0;
            int rank4 =>
                  cardIntNoJokers.Distinct().Count() == 1 ? 6 //4k
                : cardIntNoJokers.Distinct().Count() == 2 && cardIntNoJokers.GroupBy(x => x).Max(x => x.Count()) == 3 ? 4 //3k
                : cardIntNoJokers.Distinct().Count() == 2 && cardIntNoJokers.GroupBy(x => x).Max(x => x.Count()) == 2 ? 3 //2p
                : cardIntNoJokers.Distinct().Count() == 3 ? 2 //1p
                : cardIntNoJokers.Distinct().Count() == 4 ? 1 //hc
                : 0;
            int rank3 =>
                  cardIntNoJokers.Distinct().Count() == 1 ? 4 //3k
                : cardIntNoJokers.Distinct().Count() == 2 ? 2 //1p
                : cardIntNoJokers.Distinct().Count() == 3 ? 1 //hc
                : 0;
            int rank2 =>
                  cardIntNoJokers.Distinct().Count() == 1 ? 2 //1p
                : cardIntNoJokers.Distinct().Count() == 2 ? 1 //hc
                : 0;

            int rankWithJokers
            {
                get
                {
                    if (jokerCnt == 0) return rank5;
                    if (jokerCnt == 1) return scores[rank4][jokerCnt];
                    if (jokerCnt == 2) return scores[rank3][jokerCnt];
                    if (jokerCnt == 3) return scores[rank2][jokerCnt];
                    if (jokerCnt == 4) return 7;
                    if (jokerCnt == 5) return 7; //five of kind
                    return 1;
                }
            }

            public double strength => rank+double.Parse("0."+string.Join("",Cards.Select(c=>c.Value.ToString("00"))));

            public pcCamelHand xhd { get; set; }

        }
        class pcCamelGame
        {
            public List<pcCamelHand> Hands { get; set; } = new List<pcCamelHand>();

            public long GetWinnings()
            {
                long winnings = 0;
                long i = 1;
                foreach(var hand in Hands.OrderBy(c=>c.strength)) 
                {
                    winnings += hand.Bet * i;
                    i++;
                }
                return winnings;
            }
        }

        static void day7()
        {
            var d = d7_data;
            var game = new pcCamelGame();
            foreach (var l in d) game.Hands.Add(new pcCamelHand(l));

            Console.WriteLine($"Answer1: {game.GetWinnings()}");


            var game2 = new pcCamelGame();
            foreach (var l in d) game2.Hands.Add(new pcCamelHand(l,true));

            Console.WriteLine($"Answer2: {game2.GetWinnings()}");
        }

        static string[] d7_data0 =
        """
        32T3K 765
        T55J5 684
        KK677 28
        KTJJT 220
        QQQJA 483
        """.Split("\r\n");


    }
}
