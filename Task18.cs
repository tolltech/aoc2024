using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task18
{
    [Test]
    [TestCase(@"5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0", 6, 12, 22)]
    [TestCase(@"Task18.txt", 70, 1024, 304)]
    public void Task(string input, int max, int cnt, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = new char[max + 1][];
        for (int i = 0; i <= max; ++i)
        {
            map[i] = Enumerable.Repeat('.', max + 1).ToArray();
        }
        
        foreach (var line in input.SplitLines().Take(cnt))
        {
            var ints = line.SplitEmpty(",").Select(int.Parse).ToArray();
            map[ints[1]][ints[0]] = '#';
        }


        var start = new Point(0, 0);
        var target = new Point(max, max);
        var dist = Dijkstra(start, map);

        dist[target].Should().Be(expected);
    }

    private Dictionary<Point, long> Dijkstra(Point start, char[][] map)
    {
        var dist = new Dictionary<Point, long>();
        var marked = new HashSet<Point>();

        var pq = new PriorityQueue<Point, long>();

        dist[start] = 0;

        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            var v = pq.Dequeue();
            if (!marked.Add(v)) continue;

            var nextVs = GetNextV(map, v, marked);

            foreach (var nextV in nextVs)
            {
                var weight = nextV.Weight;
                if (!dist.ContainsKey(nextV.Position) || dist[nextV.Position] > dist[v] + weight)
                {
                    dist[nextV.Position] = dist[v] + weight;
                    pq.Enqueue(nextV.Position, dist[nextV.Position]);
                }
            }
        }

        return dist;
    }

    private IEnumerable<(Point Position, int Weight)> GetNextV(char[][] map, Point fromPoint, HashSet<Point> marked)
    {
        foreach (var next in Extensions.GetVerticalHorizontalNeighbours(map,fromPoint))
        {
            if (next.Item == '#') continue;
            yield return (next.Index, 1);
        }
    }

    public static readonly (int Row, int Col) DownStep = (1, 0);
    public static readonly (int Row, int Col) LeftStep = (0, -1);
    public static readonly (int Row, int Col) UpStep = (-1, 0);
    public static readonly (int Row, int Col) RightStep = (0, 1);

    private static readonly Point[] Clocwise = [DownStep, LeftStep, UpStep, RightStep];
    private static readonly Point[] CounterClocwise = [DownStep, RightStep, UpStep, LeftStep];

    private static Point GetClockwise(Point current)
    {
        if (current.Equals(DownStep)) return LeftStep;
        if (current.Equals(LeftStep)) return UpStep;
        if (current.Equals(UpStep)) return RightStep;
        if (current.Equals(RightStep)) return DownStep;

        throw new Exception("dsa");
    }

    private static Point GetCounterClockwise(Point current)
    {
        if (current.Equals(DownStep)) return RightStep;
        if (current.Equals(LeftStep)) return DownStep;
        if (current.Equals(UpStep)) return LeftStep;
        if (current.Equals(RightStep)) return UpStep;

        throw new Exception("dsasadas");
    }
}