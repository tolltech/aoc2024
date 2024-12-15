using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task15
{
    [Test]
    [TestCase(@"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^", 10092)]
    [TestCase(@"########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<", 2028)]
    [TestCase(@"Task15.txt", 1414416)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var split = input.SplitEmpty(Environment.NewLine + Environment.NewLine);

        var map = split[0].SplitLines().Select(x => x.ToArray()).ToArray();
        var moves = split[1].Replace("\r", "").Replace("\n", "");

        var robot = new Point();
        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (map[i][j] == '@')
            {
                //map[i][j] = '.';
                robot = new Point(i, j);
                break;
            }
        }

        foreach (var move in moves)
        {
            robot = Move(robot, map, Moves[move]);

            var dbg = Extensions.PrintMap(map);
        }

        var result = 0L;

        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (map.Get((i, j)) == 'O')
            {
                result += 100 * i + j;
            }
        }

        result.Should().Be(expected);
    }

    private Point Move(Point robot, char[][] map, Point move)
    {
        var newRobot = robot + move;
        
        var result = newRobot;

        var c = map.Get(newRobot);
        if (c == '.') result = newRobot;
        if (c == '#') result = robot;

        if (c == 'O')
        {
            var newC = Move(newRobot, map, move);
            
            if (newC.Equals(newRobot)) result = robot;
            else result = newRobot;
        }

        if (result.Equals(newRobot))
        {
            map[newRobot.Row][newRobot.Col] = map.Get(robot);
            map[robot.Row][robot.Col] = '.';
        }

        return result;
    }

    private static readonly Dictionary<char, Point> Moves = new()
    {
        { '^', Extensions.UpStep },
        { 'v', Extensions.DownStep },
        { '<', Extensions.LeftStep },
        { '>', Extensions.RightStep },
    };
    
    class Robot
    {
        public Point Start { get; set; }
        public Point Position { get; set; }
        public Point Velocity { get; set; }

        public Point MapSize { get; set; }

        private int GetPosition(int start, int velocity, int size, int seconds)
        {
            var p = start + velocity * seconds;

            var d = Math.Abs(p) % size;
            var sign = Math.Sign(p);

            return (size + d * sign) % size;
        }

        public Point Move(int seconds)
        {
            var position = new Point(
                GetPosition(Start.Y, Velocity.Y, MapSize.Y, seconds),
                GetPosition(Start.X, Velocity.X, MapSize.X, seconds)
            );

            Position = position;

            return position;
        }

        public int GetQuadrant()
        {
            var middleX = MapSize.X / 2;
            var middleY = MapSize.Y / 2;

            var newX = Position.X - middleX;
            var newY = Position.Y - middleY;

            if (newX == 0 || newY == 0) return -1;

            if (newX < 0 && newY < 0) return 1;
            if (newX < 0 && newY > 0) return 2;
            if (newX > 0 && newY > 0) return 3;
            if (newX > 0 && newY < 0) return 4;

            throw new Exception("dsdsdsdsd");
        }
    }
}