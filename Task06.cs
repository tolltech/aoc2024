using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task06
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
        41)]
    [TestCase(@"Task06.txt", 5531)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var result = new HashSet<(int, int)>();

        var guard = Extensions.First(map, '^');
        var direction = UpStep;
        var index = guard.Index;
        map[index.Row][index.Col] = '.';

        while (true)
        {
            var s = DBG(map, result, index);
            
            result.Add(index);

            (int Row, int Col) newIndex = (index.Row + direction.Row, index.Col + direction.Col);
            if (newIndex.Row < 0 || newIndex.Col < 0 || newIndex.Row >= map.Length ||
                newIndex.Col >= map[0].Length) break;

            if (map[newIndex.Row][newIndex.Col] == '#')
            {
                direction = GetNextDirection(direction);
            }
            else
            {
                index = newIndex;
            }
        }

        result.Count.Should().Be(expected);
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