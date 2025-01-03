﻿using System;
namespace aoc2024
{
    internal partial class Program
    {
        static string[] d16_data =
        """
        #############################################################################################################################################
        #.............#.......#...#...............#...........#.......................#...........#.............#.#.............#...#.....#........E#
        #.#.#########.#.#####.#.###.#####.#######.#.#.###.###.#.#####.#.#.###.#.#####.#######.#.###.#.#.###.###.#.#.#.#####.###.#.###.###.#.#######.#
        #.#.........#...#...#.#...#.....................#.#.........#.#...#...#...#.#...#...#.#.....................#.......#.#.#...#.#.#...#...#.#.#
        ###########.#####.###.###.#####.#.#.#######.#.###.#######.#.#.#########.#.#.###.#.#.#.#.#.###.#.###.###.#####.#####.#.#.###.#.#.#####.#.#.#.#
        #...........#...#...#.#.....#...#.......#...............................#.....#...#.#.#...#.#.#.#.#...#.#...#.#.......#.#...#.........#...#.#
        #.#########.#.#.#.#.#.#.#####.#######.#.#.#.###.#.#.#.#####.#########.#.###.#######.#.#####.#.#.#.#.#.#.#.#.#.#.#####.#.#.#####.###########.#
        #.#.......#.#.........#.#.....#.....#...#.......#.#.#.....#.#...#.....#.#...#...#...#.#...#.#.#...#...#...#...#.....#...#.......#...........#
        #.#.#####.###.#.#####.#.#.#########.#.###########.#.#####.#.###.#.#####.#.###.#.#.###.#.#.#.#.#.#.#.#########.#####.###########.#.#####.###.#
        #...#...#...#.#.....#.#...........#.#.....#.#...#.#.#...#...#...#...#.....................#.#...#.#...........#...#.#.........#.#...#.....#.#
        #.###.#.#.#.#.#####.#.###########.#.#.###.#.#.#.#.#.###.#####.#.###.###.#.#.#######.###.#.#.###.###.#####.#####.#.#.###.#.###.#.###.#.#.#.#.#
        #...#.#.#.#.....#.....................#...#.#.#...#.....#.....#.#.#...#.......................#.#...#...#.#.....#.........#...#.#...#.#.#.#.#
        #.###.#.#########.###.#.#######.#.###.#.###.#.#########.#.###.#.#.###.#.###.#.#####.#.#####.###.#.#.#.#.#.#.#####.###.#.#.###.###.###.#.###.#
        #.#...#.......................#.#.#.....#...#...#.....#.#...#.#...#...#...#.#.....#.#...#...#...#.#.#.#.#.......#...#.#.#...#.#...#.#.#.#...#
        #.#.###########.#.#.#.###.#.#.#.###.#.###.#.###.#.#.###.#####.###.#.#####.#.###.#.#.#.#.#####.###.#.#.#.#.#######.###.#.###.#.#.###.#.#.#.###
        #.#.#.........#...#.#.....#.#.#.......#...#...#...#.#...#.....#...#.......#...#.#.#.#.#.#.....#...#...#.#.#.....#.....#.#...#...#...#.#...#.#
        #.#.###.#.###.#####.#######.#.#.#####.#.#.###.###.###.###.#####.#############.#.#.#.###.#.#.###.###.###.###.###.#.#######.#.#####.#.#.#####.#
        #.#.#.....#.............#...........#...#.#...#...#...#...#...#.#.....#.......#.#.#.....#.#.#...#.....#.....#...#.........#.#...#.#...#.....#
        ###.#.#.###.#.###.#####.###.###.#####.###.###.#.###.###.###.###.###.###.###.#####.#.#####.#.#.#####.#########.###########.###.#.###.#####.#.#
        #...#...#...#...#.....#...#...#.#.....#.....#.#.#.......#...........#...#...#...#...#...#.#.#.....#.#.......#...#.....#...#...#...#...#...#.#
        #.###.#.#.###.#.#####.###.###.#.#.#####.###.###.#.#####.#.#####.#####.#######.#.#####.#.#.#.#####.#.#.#.###.###.#.###.#####.#####.###.#.###.#
        #...#.#.......#...#...#...#...#.#.#.....#.#.....#.#.....#.#...#.#.....#.......#.......#.#.....#...#...#...#...#.#...#.......#...#...#...#...#
        #.#.###.#####.###.#.###.###.###.#.###.#.#.#######.#.#.#####.#.###.#####.###############.#.###.#.#######.#.#####.#.#.#########.#####.###.#.#.#
        #...#.....#...#...#.#...#...#...#.....#...#...#...#.#.#.....#...#.....#.#...#.........#.#...#...#.....#.#.....#.....#.......#.....#...#...#.#
        #.###.#.#.###.#.#.#.#.###.###.#.#########.#.#.#.###.#.#.#######.###.#.#.#.###.#######.#.#.#.#######.#.#.#####.#####.#.###.###.###.###.#.#.###
        #...#...#...#.#.#.#.#.........#.........#.#.#.#.#...#.#.#.............#.#.#.......#...#...#.........#.#...#...#.....#.#...#...#.....#...#...#
        #.#.###.###.#.#.#.#.###########.#######.#.#.#.#.#.###.#.###########.#.#.#.#.#####.#.#.#.#.###########.###.#.#.#.###.#.#####.###.#.#####.#.#.#
        #.#.....#...#...#.#.......#.....#...#...#.#.#.#...#...#.........#.....#.#...#...#.#.#...#.#.......#...#.....#...#...#.........#.#.#...#.....#
        #.#######.###.###.#######.###.#.#.#.#####.#.#.#.###.###########.#######.#####.#.#.#.###.#.#.#######.###.#########.#######.#####.###.#.#.#.#.#
        #...#.....#.......#.....#...#...#.#.#.......#.....#...........#.#.......#.....#.#.#...#.#.#.......#.....#.......#...#...#...#...#...#.#...#.#
        #.#.#.#######.#####.###.#.#.#####.#.#.#############.#########.#.#.#######.#####.#####.#.#.#.#####.###########.#.###.#.#.###.#.###.###.#.#.#.#
        #...#...........#.....#.#.#...#...#...#.......#...#...#.......#...#...#.....#.#.#.....#.#.#.#.....#...........#.....#.#...#.#.#...#.....#.#.#
        #.#.###########.#.#####.#.###.#.#######.#####.#.#.###.#.#####.#####.#.#.###.#.#.#.#.#####.#.#.###.#.#.###.#########.#.###.###.#.#########.#.#
        #...#.......#...#...#...#.#...#.......#...#.#...#.....#.#...#...#...#.....#.#...#.#.#.....#.#.#...#.#...#.#.......#...#...#...#.....#.....#.#
        #.###.#####.#.#.#.###.###.#.###.#########.#.#.#.#######.#.#.###.#.###.#####.#.###.#.#.#####.#.#####.#.#.###.#####.#####.###.#######.#.#.#.#.#
        #.#.#.#.......#.#.#...#.....#...#.......#...#.#.#.......#.#.#...#.....#.....#.#...#.#...#.#.#.....#.#.#.......#...#...#.#.......#...#...#.#.#
        #.#.#.###########.#.#####.#.#.#.#.#####.###.#.#.#.###.###.#.#.#########.#####.#.###.###.#.#.#####.#.#.###.#####.#.###.#.#######.#.###.###.#.#
        #...#.......#.....#...#...#.#.#.#...#.#...#.#...#...#.#.#.#.#.........#...#...#...#...#.#.......#.#...#.....#...#.....#...#...#.#.#...#...#.#
        ###.#######.#.#######.#.###.#.#.###.#.###.#.#########.#.#.#.#########.###.###.###.###.#.#########.###.#.###.#.###.#######.#.#.#.#.###.#####.#
        #...#...#...#...#.....#...#.#.#...#...#.#...#.........#.#.#.........#...#...#...#.#...#.........#.....#.#.#...#...#.....#.#.#...#...#.......#
        #.#####.#.#####.#.#######.#.#.#.#.###.#.#######.#######.#.###.#.#######.#.#.#.#.#.#############.#######.#.###.#.###.#.#.#.#.###.###.#######.#
        #.....#.#.#.....#.#.......#.#...#...#.#...#.....#.#.......#...#.#.......#.#.#...#...#...........#.....#.#.......#...#.#.#...#.#...#.......#.#
        #####.#.#.#.#####.#####.#########.###.#.#.#.#.###.#.#######.#####.#######.#.#.#####.#.#########.#.###.#.#.#######.###.#######.###.###.###.#.#
        #.....#.#...#...#.....#...........#...#.#...#.....#.#.....#...#...#.......#.#.......#.#...#...#...#.....#...#...#...#...........#.....#...#.#
        #.#####.#####.#######.###########.#.#######.###.#.#.#.###.#.#.#.###########.#########.#.#.#.###.#.###########.#.###.#.###.#########.#.#.###.#
        #.....#...#.#.......#.......#...#.#.......#...#.#.....#.#.#.#.#.....#...............#.#.#...#...#.....#...#...#...#.....#...........#...#.#.#
        #####.#.#.#.#.#####.#######.#.#.#########.#.#.#.#####.#.#.#.#.#####.#.###.#.#######.#.#.#####.#######.#.#.#.#.###.#####.#############.###.#.#
        #.....#.#...#.#...#.......#...#.........#.#.#.....#.......#.#.....#.#.#...#.#.....#.#.#...#...#...#...#.#...#...#.......#...........#.#.....#
        #.#####.#.###.#.#########.###########.###.#.#.###.#####.#.#.#.#.###.#.#.###.###.#.#.#.#.#.#.###.#.#.###.#######.###############.###.#.#####.#
        #.........#...#.........#...........#...#.#.....#.....#.....#.#.#...#.#...#.....#.#.#.#.#.......#.#...#.......#.................#...#.....#.#
        #.###.#.#.#.#.#.#######.#####.###.#.###.#.###.#######.#######.###.###.###.#########.#.###############.#######.#.#######.#############.###.#.#
        #...#...#...............#...#...#...#...#...#.#.....#...#...#.........#.......#.....#.#.............#.....#...#.......#.#...#...#.........#.#
        ###.#.###.###.###.#######.#.###.###.#.#####.###.###.#.#.#.#.###.#############.#.#.###.#.###########.#.###.#.#########.###.#.#.#.#.###.#####.#
        #.#.#...#.#.......#...#...#...#...#.#.....#.....#.#.#.#...#...#.#.......#...#.#.#.#...#.#.......#...#.#.....#...#.....#...#.#.#...#.....#...#
        #.#.#.#.#.#.#.#####.#.#.#####.###.#.#####.#######.#.#.#######.#.#.#####.#.###.#.###.###.#######.#.#####.###.#.#.#.#####.###.#.#####.#.###.###
        #...#.#...#.#.......#...#...#.....#.#...#.......#...#...#...#.#.#.....#.#.....#...#.#.........#.........#...#.#.#...#...#.#...#...#.......#.#
        #.###.#.#.#.#.###.#######.#.#######.#.#.#######.#.#####.###.#.#######.#.#.###.###.#.###.#####.#######.###.###.#.###.#.###.#####.#.#.#.#####.#
        #.#.......#...#.........#.#.......#.#.#.....#.....#.#...#.......#...#.#.#...#.#...#...#.#...#.......#.#...#...#...#.#.#.........#.#...#...#.#
        #.#####.#.###.#.#######.#.###.#.###.#.#.###.#.#####.#.###.#.###.#.#.#.#.#.#.#.#.#####.#.###.#######.###.#####.###.#.#.#.#####.###.###.#.#.#.#
        #.....#...#...#.......#.#...#.#.#...#.#.#...#...#.#...#...#.#...#.#...#...#.#.#.....#.#...#.......#.....#.....#...#.#.......#...#...#...#...#
        #####.#.###.#####.###.#.#.###.#.#.#####.#.###.#.#.#.#####.#.#.#.#.#########.#.#####.#.###.#####.#.###.#.#.#####.###.#######.#######.#.#####.#
        #...#.#...#.....#...#.#.#.....#.#.#.....#.....#.#...#.....#.#...#...#.#...#.#...........#.....#.#...#...#.#.........#.......#...........#...#
        #.###.#.#######.#####.###.#.#####.#.###########.###.#.#.#####.#####.#.#.#.#.#.#########.#####.#.#.#.#####.#.#########.#######.###########.###
        #.#...#.........#...#.#...#...#...#.#.....#...#...#.#.#.#.....#...#...#.#.#.#.#...#.....#...#...#.....#...#.#...#.....#.#.................#.#
        #.#.###.#########.#.#.#.#####.#.###.#.###.#.#####.#.###.#.###.#.#.#.###.#.#.###.#.#.#######.###.#####.#.###.#.#.#.#####.#.#.###.###########.#
        #.#.....#.........#...#.#.....#...#.#...#.#.......#.....#...#.#.#...#...#...#...#...#.......#.......#...#.#...#.#.#.....#.#...#.....#.......#
        #.#####.#.###.###.#####.#.###.###.#.###.#.#####.###########.###.#####.###.#.#.#######.#.#####.###.#.#####.#.#.###.#.#.###.###.#####.#.#######
        #...#.....#...#...#...#.#...#...#.#.#.........#.............#...#...#...#.#.#.........#.....#.#.#.#...#.....#.....#.#.....#...#.#...#.......#
        #.#.#.#.###.#.#.###.#.#.###.#####.#.#.#.#####.#.###.#######.#.###.#.###.#.#.#.#.###########.#.#.#.###.#.#.###.#####.#######.#.#.#.#########.#
        #.#...#.....#...#.#.#...#.#.......#.#...#...#...#.....#.....#.#...#.#...#...#.#...........#.....#.#...#...#...#.....#.....#.#.#.#...........#
        #.#####.#####.###.#.###.#.#########.###.###.###.#.###.###.#.#.###.###.#######.#################.#.###.###.#.###.#####.###.#.#.#.###########.#
        #.....#...#.#.....#.....#.....#...#...#.#.....#...#.#.#.....#.#...#.........#.........#...#...#.#.......#.#...#.#.#...#.#.#.#.#...........#.#
        #####.###.#.#.###.#####.###.#.#.#.#.#.#.#.#.#.#####.#.#.#.#.#.#.#.#.#######.#.###.#.#.#.#.#.#.#.#####.#.#.###.#.#.#.###.#.#.#.###.#########.#
        #.....#...#.....#.....#.....#.#.#...#.#...#.#.....#.#...#.....#.#.#.....#...#...#...#...#...#.#...#...#.....#.#...#.#.#...#...#...#.........#
        #.###.#.###.#.#.#.###########.###.###.#####.#####.#.#####.#.#.#.#.#####.#.###########.###.###.#####.###.#####.#.###.#.#.###.#.#.#.#.#########
        #.#.....#.....#...#.......#.#...#...#.......#...#.......#.#.#...#.#...#.#.#.................#.....#.#.....#...#.#...#...#...#...#.#...#.....#
        #.#.#.###.#.#.###.#.###.#.#.###.###.###.###.#.#.#####.###.#.###.###.#.#.#.#.#######.#######.#####.#.#.#####.###.#.###.#####.#####.###.#####.#
        #.#.#.#.....#...#.#.#.#.#.#...#...#...........#...#...#...#...#.#...#...#.#...#.....#.....#.......#...#.#...#...#.#.#.#.....#...#.....#...#.#
        #.#.###.###.#.#.###.#.#.#.###.###.###.###.#######.#.###.#######.#.###.###.###.#######.###.#########.###.#.###.###.#.#.#.#####.#.#.###.#.#.#.#
        #.#.......#.....#.....#.#.#...#...#...#...#...#...#.#.#.........#...#.#...#...#.......#.#.#.......#.....#.#...#...#.....#...#.#.#.....#.#...#
        #.#######.#.#.###.#####.#.#.#.#.###.#######.#.#.###.#.#############.###.###.###.#######.#.#.#.###.#######.#.###.#########.#.#.#.#.###.#.###.#
        #.#.#...#.#.........#...#.#.#.#.#...........#.#...#.#.............#...#...#.............#.#.#...#.........#...#...#.....#.#...#.#...#.#.#...#
        #.#.#.#.###.#.#######.###.#.#.#.#######.#.###.#####.###.#######.#####.###.#####.#########.#.###.###########.#####.#.###.#.#####.#.#.#.#.#####
        #...#.#...#.....#.....#.#...#.#.......#.#...#...........#.......#.......#.....#.#...#.....#.#.#.#.#...#...#.#.....#.#.....#.....#.#.#.#.....#
        #####.###.#.#.###.#####.#####.#######.#.###.#######.#############.#####.#####.###.#.#.#####.#.#.#.#.#.#.#.#.#.#####.#.#####.#####.#.#.#.###.#
        #.....#.#.#.......#.........#...#...#.........#...#.........#...#.....#.....#...#.#...#.....#.#...#.#...#...#.....#...#...#.................#
        #.#####.#.#.#.#.#########.#####.#.#.#.#####.#.###.#.#######.#.#.#.###.###.#.###.#.#########.#.#.#.#.#############.###.###.#####.#.#####.#.###
        #...#.#...#...#.#.......#.........#.#.....#.......#.......#...#.#...#.#...#...#.#.#.......#...#.#.#.#...............#.#.....#.#.#.#...#.#...#
        ###.#.#.###.#.#.#.#####.###########.#####.#######.#######.#.###.###.#.#.###.#.#.#.#.###.#.#####.#.#.#.#.#############.#.###.#.#.#.#.#.#####.#
        #...#...........#.#...#...#...#.........#...#...#...#...#.#...#...#.#.#.....#.#...#.#...#.#.....#...#...........#...#...#.....#.#...#.....#.#
        #.#####.###.#.#.#.###.###.#.#.#############.#.#.#.###.#.#.###.###.###.###.###.#######.###.#.#####.#############.#.#.#####.#.#.#.#.#######.#.#
        #.#...#...#.#.#.#...#...#...#.#.....#.....#.#.#.#.#...#...#.....#.....#.....#.#.......#.#...#.....#.....#...#.#...#.......#.#.#.........#...#
        #.#.#.###.#.#.#.###.#.###.###.#.###.#.###.#.###.#.#.#######.###########.###.#.#####.###.#####.#####.###.#.#.#.#########.###.#.###.###.#####.#
        #...#.#.#.....#.#...#.....#...#...#.#.#.#.....#...#.....#...............#.#.#.......#.......#.......#.#...#.#.........#.....#.#...#.#.#.....#
        #.###.#.###.###.#.#####.#.#.#.#.#.#.#.#.#.###.#####.###.#.#############.#.#.#########.#.###.#########.#####.#.#####.#########.#.###.#.#.#####
        #...#.#...#.....#...#...#...#.#...#.#.#.#...#.#...#.#...#.#.........#...#.........#...#.#...#...#.....#.....#...#.#...#.......#...#...#.#...#
        ###.#.###.#.#######.#.###.#.#.###.#.#.#.###.#.#.#.###.###.#.#.###.###.#######.###.#####.#.###.###.###.#.#.#####.#.###.#.###.#####.###.#.#.###
        #.#.#...#.........#.#.#.....#.....#.......#.#...#.....#.....#.#.#.#.........#...#...#...#.#.....#...#...#.....#.......#.#.#.....#.#.....#...#
        #.#.###.#########.#.#.#.#######.#.#########.###############.#.#.#.#.###.###.#.#####.#.###.#####.###.#####.###.###.###.#.#.#.#.#.#.#.#.#####.#
        #.#.#.#.#...#...#.#.......#...#.#.....#.....#.............#.#...#.#.......#.#.......#.#.......#...#.#.#.....#.#...#.........#.......#.....#.#
        #.#.#.#.#.#.#.#.#.#.#####.#.#.#.#.#####.###.#.#.#########.#.#####.#########.#.#######.#######.#.###.#.#.#.#.#.#####.#.#####.#.#######.###.#.#
        #.#.#.#...#.#.#.#.#.....#...#.#...#.....#...#.#...#.................#...#.......#.....#...#.....#...#.....#.#...#...#...#...#.#.......#.....#
        #.#.#.#####.###.#.#.###.#####.#####.#####.#.#####.#.###############.#.#.#.#####.#.#.###.#.#####.#.#######.#.###.#.#######.###.#.#####.#####.#
        #.........#.#...#.#.#.......#...#.#...#...........#...#...#.........#.#...#...#.#.#.....#.....#.#.....#...#.#...#.....#.....#.#.#...#...#.#.#
        #.#########.#.###.#.#.#####.###.#.###.#####.#####.#.#.#.#.#######.###.#####.###.###########.#.#.#####.#####.#.#####.#.#.#####.#.#.#.#.#.#.#.#
        #...........#.....#.#.#...#...#.#.........#.....#.......#.#.....#.#.#.#...#...............#.#.#.#...#.....#.#.....#.#.....#...............#.#
        #####.#.#.###.#######.#.#.#.###.###.#####.###.#.###.#####.#.###.#.#.#.#.#.#.#############.###.#.###.#####.#.#.###.#########.###.#.###.#.#.#.#
        #...#.#.#.#...........#.#...#...#...#...#.....#.......#.....#.#.#...#.#.#...#.....#...#...#...#.........#...#.....#...#...#.....#...#...#...#
        #.#.#.#.#.#############.#####.###.#.###.###.#####.###.#######.#.#####.#####.#.###.###.#.###.#################.#####.#.#.#.#######.#.#.#####.#
        #.#...#...#.....#...#...#.....#...#.....#...#.......#...#.....#...#...#...#.#.#.#...#...#...#...............#.......#...#.......#.#.......#.#
        #.#######.#.###.#.###.###.#######.#####.#.#.#.#.#######.#.#.#####.#.###.#.#.#.#.###.#.###.###.###########.#####################.###.#####.###
        #.....#...#...#.#.....#...#.......#...#...#.#.#.#.......#.#.........#...#.#.#...#...#.....#...#.....#...#.#...............#...#...#.#...#...#
        #.###.###.###.#.#######.###.#######.#.#####.###.#.#.#####.###########.###.###.#.#.#.#####.#.###.###.#.#.#.#.#######.#####.#.#.###.#.###.###.#
        #...#.....#...#.........#...........#.....#.....#.#.#...#...#...#...#...#...#...#.#.#...#.#.#...#.#...#.#...#.....#...#.....#...#.#.....#...#
        ###.#####.#.#############.#.#.#####.#.#########.#.###.#.###.#.#.#.#.###.###.#.###.#.#.#.###.#.###.#####.#####.#.#.###.#######.###.#####.#.###
        #...#...#.#.#.....#.....#...#.....#.#.#...#.....#.#...#.#.#.#.#.#.#...#...#.#.#...#.#.......#...#...#.....#...#.#...#.....#...#...#.....#...#
        #.#####.#.#.#.###.#.#.#.#.#####.###.#.#.#.#.###.#.#.###.#.#.#.#.#.#.#.#.#.#.#.#.#####.#########.#.###.#####.###.###.#####.#.###.#######.###.#
        #.#...#.#.#.#.#.#...#.#.#...........#.#.#...#.#...#.#.#.#.....#.#...#...#.#.#.#.....#.#.....#.....#...#.......#...#.#...#.#...#.#.....#.#...#
        #.#.#.#.#.#.#.#.#####.#.#.#.#.#########.#####.#.###.#.#.###########.###.#.#.#.###.#.#.#.###.#######.#.#.#########.#.###.#.#####.###.#.###.#.#
        #...#...#...#.......#.#.#.#.#.#...#.....#.....#.#...#.#.......#.....#...#.#...#...#...#.#.#.#.....#.#...#.....#...#.#...#.#...............#.#
        #######.#.#########.#.#.#.###.#.#.#.#####.###.#.#.###.#######.#.#####.###.#####.#.#####.#.#.#.###.#.#.###.###.#.###.#.#.#.#.#.###.#####.#.#.#
        #.#.....#...#.....#.#.#...#...#.#...#.....#...#...#.........#...#...#.....#.....#.....#.#.#...#.#...#...#...#...#.#...#.#.#.#.....#...#...#.#
        #.#.#####.#.#.###.#.#.#####.###.#####.###.#.#######.#############.#.#######.#####.###.#.#.#####.###.###.###.#####.#####.#.#.#.#.#.#.#####.#.#
        #.#.#.....#...#.#.........#.....#.#...#...#.#.......#.............#.#.....#.#...#...#...#.......#...#.....#.......#.....#...#.......#.....#.#
        #.#.#.#.#.#####.#######.#.#.#####.#.###.###.###.###.#.#####.#######.#.###.###.#.###.#####.#.#####.###.#.#######.#.#.###########.#.###.###.#.#
        #...#.#.....#...........#.#...#.#.....#.#.....#...#.......#...#...#...#...#...#...#.#.#...#.#.....#...#.........#...#...........#.....#...#.#
        #.#######.#.###.#.#####.#.###.#.#.###.#.#####.#.#.###########.#.#.#####.#.#.#####.#.#.#.#.###.#####.###.#.#.#####.###.#####.###.#.#####.#.#.#
        #.......#.#...#.#.#...#.#...#.#...#...#.#...#.#.#.......#.....#.#.....#...#.#...#.#.#...#.....#.....#...#.....#...#.....#...#...#...#...#...#
        #######.#####.#.#.#.###.#.#.#.#.#####.#.#.#.#.#.#######.#.#####.#.#######.#.#.#.#.#.#########.#.#.#####.###.#.#.#####.###.#.#.#.###.#.###.###
        #.....#.#...#.#.#.#.....#.#.............................#.#...#.#.#.......#.#.#...#...#.......#.........#.#...#.#.....#...#...#...#.#.....#.#
        #####.#.#.#.#.#.#.###.###.#.#.###.#.#.#####.###.#.#######.#.#.#.#.#.#####.#.#.#####.#.#.#####.#.#.#.###.#.#.###.###.#.#.###.###.#.#.#####.#.#
        #.....#...#...#.#...#.#...#.#.#...#.#.#.....#...#.....#.....#.#.#...#.......#.....#.#...#...#...#.....#.#.#.#.......#...#...#...#.#.#.....#.#
        #.###.#########.###.#.#.###.#.###.#.#.#.#####.#.#####.#######.#.#####.###.#######.#.#####.#.#.###.#.#.#.#.#.#########.###.#.#.#.###.###.#.#.#
        #.#.#.......#...#...#...#.#.#...#...#.#.#.#...#.....#.#.......#.....#.#.......#.#.#.......#.#.....#.#.#...#...#.......#.......#...#.....#...#
        #.#.###.#####.###.#######.#.###.#####.#.#.#.###.###.#.#.###########.#.#.#.###.#.#.###########.###.#.#.###.#.#.#.###.###.###.###.#.#.#####.#.#
        #.#.....#...#.#.#.#...........#.#.....#.#...#.....#.#...#.......#...#...#...#.#...............#...#.#...#...#.#...#.#...#...................#
        #.###.###.#.#.#.#.#######.###.#.#######.#.#####.#.#.#####.#####.#.#####.#.#.#.###############.#.#.#.###.###.#.###.#.#.#####.###.###.#.#.#.#.#
        #...#.#...#...#.#.......#.#.....#.......#.......#.................#...#.....#...#.....#.........#.....#.......#...#.#.#.....#...#...#.#.#...#
        #.#.###.#######.#######.###.#.###.#############.#.#########.#####.#.#.###.#####.#.###.#.###.#####.#.#######.###.###.#.#.#####.#.###.#.#.#.###
        #S#...................#...........#.......................#.........#...........#...#.....#.......................#.....#...........#.......#
        #############################################################################################################################################
        """.Split(Environment.NewLine);
    }
}

