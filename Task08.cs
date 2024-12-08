using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task08
{
    [Test]
    [TestCase(
        @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............",
        14)]
    [TestCase(@"Task08.txt", 332)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var lines = input.SplitLines();

        var result = 0L;

        var totalAntennas = new List<(char Freq, Point2 Point)>();
        for (var i = 0; i < lines.Length; i++)
        for (var j = 0; j < lines[i].Length; j++)
        {
            var c = lines[i][j];
            if (c == '.') continue;

            totalAntennas.Add((c, new Point2(i, j)));
        }

        var antennas = totalAntennas.GroupBy(x => x.Freq)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Point).ToArray());

        var antinodes = new Dictionary<char, HashSet<Point2>>();

        foreach (var antenna in antennas)
        {
            var c = antenna.Key;
            var points = antenna.Value;

            for (var i = 0; i < points.Length; i++)
            for (var j = 0; j < points.Length; j++)
            {
                if (i == j) continue;

                var first = points[i];
                var second = points[j];

                var nodes = GetAntiNodes(first, second)
                    .Where(x => InsideMap(lines, x))
                    .ToArray();

                if (!antinodes.TryGetValue(c, out _))
                {
                    antinodes[c] = new HashSet<Point2>();
                }

                foreach (var node in nodes)
                {
                    antinodes[c].Add(node);
                }
            }
        }
        
        var totalAntinodes = antinodes.Values.SelectMany(x => x).ToHashSet();
        totalAntinodes.Count().Should().Be(expected);
    }

    private Point2[] GetAntiNodes(Point2 first, Point2 second)
    {
        var deltaX = second.Col - first.Col;
        var deltaY = second.Row - first.Row;

        return
        [
            new Point2(second.Col + deltaX, second.Row + deltaY),
            new Point2(first.Col - deltaX, first.Row - deltaY)
        ];
    }

    private bool InsideMap(string[] lines, Point2 point)
    {
        if (point.Col < 0 || point.Row < 0) return false;
        if (point.Row >= lines.Length) return false;
        if (point.Col >= lines[0].Length) return false;

        return true;
    }
    
    private string DBG(string[] map, HashSet<(int, int)> visited)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < map.Length; ++i)
        {
            for (var j = 0; j < map[i].Length; ++j)
            {
                var c = map[i][j];

                if (visited.Contains((i, j))) c = '#';

                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}