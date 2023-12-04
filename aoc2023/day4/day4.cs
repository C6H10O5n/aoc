using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aoc2023_02
{
    internal partial class Program
    {

        class Card
        {
            public Card(string s) 
            {
                var c = s.Split(": ");
                var n = c[1].Split(" | ");
                id = int.Parse(c[0].Trim().Split(" ").Last());
                Wins = Regex.Replace(n[0], " {2,}", " ").Trim().Split(" ").Select(n=>int.Parse(n)).ToArray();
                Nums = Regex.Replace(n[1], " {2,}", " ").Trim().Split(" ").Select(n => int.Parse(n)).ToArray();
            }
            public int id { get; private set; }
            public int[] Wins { get; private set; }
            public int[] Nums { get; private set; }

            public int NumsWin => Wins?.Intersect(Nums).Count() ?? 0;
            public int Score => NumsWin < 1 ? 0 : NumsWin == 1 ? 1 : (int)Math.Pow(2, NumsWin - 1);

            public int WinningCardCount { get; set; } = 0;
        }

        static void cardCnt(Card card, List<Card> d)
        {
            for (int i=1; i<=card.NumsWin; i++)
            {
                var di = i + card.id-1;
                if (di < d.Count)
                {
                    d[di].WinningCardCount++;
                    if (d[di].NumsWin > 0) cardCnt(d[di], d);
                }
            }          
        }

        static void day4()
        {
            var cards = d4_data.Select(d => new Card(d)).ToList();

            Console.WriteLine($"Answer1: {cards.Sum(x => x.Score)}");

            foreach(var card in cards)
                cardCnt(card, cards);

            Console.WriteLine($"Answer2: {cards.Count+cards.Sum(c=>c.WinningCardCount)}");
        }

        static string[] d4_data0 =
        """
        Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        """.Split("\r\n");


    }
}
