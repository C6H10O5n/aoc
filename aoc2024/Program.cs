using System.Diagnostics;

namespace aoc2024;

internal partial class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var timer = new Stopwatch();

        Console.WriteLine("Advent of Code 2023:.....");

        //Console.WriteLine($"\n\nDay 1 Problem:");
        //timer.Start();
        //day1();
        //timer.Stop();
        //Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");

        Console.WriteLine($"\n\nDay 2 Problem:");
        timer.Start();
        day2();
        timer.Stop();
        Console.WriteLine($"Elapsed Time: {timer.Elapsed.TotalSeconds} seconds");

        Console.WriteLine("\n\n-------------------------\nDone.....\n\n\n\n");
        Console.ReadLine();
    }
}

