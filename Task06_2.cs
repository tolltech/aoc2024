using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task06_2
{
    [Test]
    [TestCase(
        @"....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...",
        6)]
    [TestCase(@"Task06.txt", 2165)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var guard = Extensions.First(map, '^');
        var direction = UpStep;
        var index = guard.Index;
        map[index.Row][index.Col] = '.';

        var result = 0;
        for (var i = 0; i < map.Length; i++)
        for (int j = 0; j < map[0].Length; j++)
        {
            if (map[i][j] != '.') continue;

            if (CheckCycle(map, direction, index, (i, j)))
                result++;
        }

        result.Should().Be(expected);
    }

    private bool CheckCycle(char[][] map, (int Row, int Col) direction, (int Row, int Col) index,
        (int Row, int Col) obstacle)
    {
        var result = new HashSet<((int, int), (int, int))>();
        while (true)
        {
            //var s = DBG(map, result, index);
            if (!result.Add((index, direction)))
                return true;

            (int Row, int Col) newIndex = (index.Row + direction.Row, index.Col + direction.Col);
            if (newIndex.Row < 0 || newIndex.Col < 0 || newIndex.Row >= map.Length ||
                newIndex.Col >= map[0].Length) break;

            if (map[newIndex.Row][newIndex.Col] == '#' || newIndex == obstacle)
            {
                direction = GetNextDirection(direction);
            }
            else
            {
                index = newIndex;
            }
        }

        return false;
    }

    public static readonly (int Row, int Col) DownStep = (1, 0);
    public static readonly (int Row, int Col) LeftStep = (0, -1);
    public static readonly (int Row, int Col) UpStep = (-1, 0);
    public static readonly (int Row, int Col) RightStep = (0, 1);

    private (int Row, int Col) GetNextDirection((int Row, int Column) currentDirection)
    {
        if (currentDirection == DownStep) return LeftStep;
        if (currentDirection == LeftStep) return UpStep;
        if (currentDirection == UpStep) return RightStep;
        if (currentDirection == RightStep) return DownStep;

        throw new Exception("Wrongdirection");
    }

    private string DBG(char[][] map, HashSet<(int, int)> visited, (int, int) guard)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < map.Length; ++i)
        {
            for (var j = 0; j < map[i].Length; ++j)
            {
                var c = map[i][j];

                if (visited.Contains((i, j))) c = 'X';

                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}