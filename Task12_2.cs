using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task12_2
{
    [Test]
    [TestCase(@"AAAA
BBCD
BBCC
EEEC", 80)]
    [TestCase(@"AAAA
BBBB", 32)]
    [TestCase(@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO", 436)]
    [TestCase(@"EEEEE
EXXXX
EEEEE
EXXXX
EEEEE", 236)]
    [TestCase(@"AAAAAA
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA
", 368)]
    [TestCase(@"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE
", 1206)]
    [TestCase(@"Task12.txt", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var result = 0L;

        var visited = new HashSet<Point>();

        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (visited.Contains((i, j))) continue;

            var region = GetRegion(map, new Point(i, j), visited);
            result += region.Area * region.Perimeter;
        }

        result.Should().Be(expected);
    }

    private (long Area, long Perimeter) GetRegion(char[][] map, Point point, HashSet<Point> visited)
    {
        var c = map[point.Row][point.Col];
        var region = new HashSet<Point>();

        var queue = new Queue<Point>();
        queue.Enqueue(point);

        var perimeter = 0L;

        while (queue.Count > 0)
        {
            var currentPoint = queue.Dequeue();
            if (!region.Add(currentPoint)) continue;

            var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, currentPoint).ToArray();
            perimeter += 4 - neighbours.Length;
            perimeter += neighbours.Count(x => x.Item != c);

            foreach (var neighbour in neighbours.Where(x => x.Item == c))
            {
                if (region.Contains(neighbour.Index)) continue;

                queue.Enqueue(neighbour.Index);
            }
        }

        foreach (var p in region)
        {
            visited.Add(p);
        }

        return (region.Count, perimeter);
    }
}