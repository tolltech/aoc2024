using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task15_2
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
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^", 9021)]
    [TestCase(@"#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<vv<<^^<<^^", 618)]
    [TestCase(@"Task15.txt", 1386070)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var split = input.SplitEmpty(Environment.NewLine + Environment.NewLine);

        var map = split[0]
            .Replace(".", "..")
            .Replace("#", "##")
            .Replace("O", "[]")
            .Replace("@", "@.")
            .SplitLines().Select(x => x.ToArray()).ToArray();
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
            if (map.Get((i, j)) == '[')
            {
                result += 100 * i + j;
            }
        }

        result.Should().Be(expected);
    }

    private bool CheckWall(Point objPosition, char[][] map, Point move)
    {
        if (map.Get(objPosition) == '.')
        {
            return false;
        }

        var newPositions = GetNewPositions(objPosition, map, move);

        var nextObjects = newPositions.Select(map.Get).ToArray();

        if (nextObjects.Any(x => x == '#'))
        {
            return true;
        }

        if (nextObjects.All(x => x == '.'))
        {
            return false;
        }

        var boxes = newPositions.Where(x => boxChars.Contains(map.Get(x))).ToArray();
        return boxes.Any(box => CheckWall(box, map, move));
    }

    private static readonly HashSet<char> boxChars = ['[', ']'];

    private Point Move(Point objPosition, char[][] map, Point move)
    {
        if (move.Row != 0)
        {
            var checkWall = CheckWall(objPosition, map, move);
            if (checkWall) return objPosition;

            BruteForceMove(objPosition, map, move);
            return objPosition + move;
        }

        return MoveHorizontal(objPosition, map, move);
    }

    private void BruteForceMove(Point objPosition, char[][] map, Point move)
    {
        var boxes = new HashSet<Point>();
        FlatBoxes(objPosition, map, move, boxes);

        var orderedBoxes = move.Row < 0
            ? boxes.OrderBy(x => x.Row).ToArray()
            : boxes.OrderByDescending(x => x.Row).ToArray();

        foreach (var box in orderedBoxes)
        {
            var newBox = box + move;
            map[newBox.Row][newBox.Col] = map.Get(box);
            map[box.Row][box.Col] = '.';
        }
    }

    private void FlatBoxes(Point objPosition, char[][] map, Point move, HashSet<Point> boxes)
    {
        var c = map.Get(objPosition);
        if (c == '.') return;

        boxes.Add(objPosition);

        if (c == '[')
        {
            boxes.Add(objPosition + (0, 1));
        }

        if (c == ']')
        {
            boxes.Add(objPosition + (0, -1));
        }

        var newPositions = GetNewPositions(objPosition, map, move);
        foreach (var newPosition in newPositions)
        {
            FlatBoxes(newPosition, map, move, boxes);
        }
    }

    private Point[] GetNewPositions(Point objPosition, char[][] map, Point move)
    {
        var obj = map.Get(objPosition);
        if (obj == '@') return [objPosition + move];

        if (obj == '[') return [objPosition + move, new Point(objPosition.Row, objPosition.Col + 1) + move];
        if (obj == ']') return [objPosition + move, new Point(objPosition.Row, objPosition.Col - 1) + move];

        throw new Exception("dasdsad");
    }

    private Point MoveHorizontal(Point robot, char[][] map, Point move)
    {
        var newRobot = robot + move;

        var result = newRobot;

        var c = map.Get(newRobot);
        if (c == '.') result = newRobot;
        if (c == '#') result = robot;

        if (c == '[' || c == ']')
        {
            var newC = MoveHorizontal(newRobot, map, move);

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