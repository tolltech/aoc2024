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
    [TestCase(@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO", 18, true)]
    [TestCase(@"EEEEE
EXXXX
EEEEE
EXXXX
EEEEE", 236)]
    [TestCase(@"EEEEE
EXXXX
EEEEE
EXXXX
EEEEE", 10, true)]
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
    [TestCase(@"Task12.txt", 872382)]
    
    [TestCase(@"AAAA
BBCC
DDDC
DCCC", 11, true)]
    public void Task(string input, long expected, bool onlyHor = false)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var visited = new HashSet<Point>();
        var regions = new List<HashSet<Point>>();

        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (visited.Contains((i, j))) continue;

            var region = GetRegion(map, new Point(i, j), visited);
            regions.Add(region);
        }

        var borderFirstPoints = new List<Point>();
        for (var i = 0; i < map.Length; i++)
        {
            char prev = default;
            char prevUp = default;
            char prevDown = default;
            
            for (var j = 0; j < map[i].Length; j++)
            {
                var curUp = map.SafeGet(i - 1, j);
                var cur = map[i][j];
                var curDown = map.SafeGet(i + 1, j);
                
                if (cur != prev)
                {
                    if (curUp != cur) borderFirstPoints.Add((i,j));
                    if (curDown != cur) borderFirstPoints.Add((i,j));
                }
                else
                {
                    if (prevUp == cur && cur != curUp)borderFirstPoints.Add((i,j));
                    if (prevDown == cur && cur != curDown)borderFirstPoints.Add((i,j));
                }
                
                prev = cur;
                prevUp = curUp;
                prevDown = curDown;
            }
        }

        if (onlyHor)
        {
            ((long)borderFirstPoints.Count).Should().Be(expected);
            return;
        }

        for (var j = 0; j < map[0].Length; j++)
        {
            char prev = default;
            char prevLeft = default;
            char prevRight = default;
            
            for (var i = 0; i < map.Length; i++)
            {
                var curLeft = map.SafeGet(i, j-1);
                var cur = map[i][j];
                var curRight = map.SafeGet(i, j+1);
                
                if (cur != prev)
                {
                    if (curLeft != cur) borderFirstPoints.Add((i,j));
                    if (curRight != cur) borderFirstPoints.Add((i,j));
                }
                else
                {
                    if (prevLeft == cur && cur != curLeft)borderFirstPoints.Add((i,j));
                    if (prevRight == cur && cur != curRight)borderFirstPoints.Add((i,j));
                }
                
                prev = cur;
                prevLeft = curLeft;
                prevRight = curRight;
            }
        }

        var result = 0L;
        foreach (var region in regions)
        {
            result += region.Count * borderFirstPoints.Count(x => region.Contains(x));
        }

        result.Should().Be(expected);
    }

    private HashSet<Point> GetRegion(char[][] map, Point point, HashSet<Point> visited)
    {
        var c = map[point.Row][point.Col];
        var region = new HashSet<Point>();

        var queue = new Queue<Point>();
        queue.Enqueue(point);

        while (queue.Count > 0)
        {
            var currentPoint = queue.Dequeue();
            if (!region.Add(currentPoint)) continue;

            var neighbours = Extensions.GetVerticalHorizontalNeighbours(map, currentPoint).ToArray();

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

        return region;
    }
}