using System;
using System.Diagnostics;

namespace aoc2023_02
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            var timer = new Stopwatch();

            Console.WriteLine("Advent of Code 2023:.....");


            Console.WriteLine($"\n\nDay 1 Problem:");
            timer.Start();
            day1();
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");


            Console.WriteLine($"\n\nDay 2 Problem:");
            timer.Start();
            day2();
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");


            Console.WriteLine($"\n\nDay 3 Problem:");
            timer.Start();
            day3();
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");


            Console.WriteLine($"\n\nDay 4 Problem:");
            timer.Start();
            day4();
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");



            Console.WriteLine("\n\n-------------------------\nDone.....\n\n\n\n");

        }
    }
}
