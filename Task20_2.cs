using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task20_2
{
    [Test]
    [TestCase(@"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############", 76, 3)]
    [TestCase(@"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############", 74, 7)]
    [TestCase(@"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############", 72, 29)]
    [TestCase(@"Task20.txt", 100, 1327)]
    public void Task(string input, long seconds, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var start = map.Find('S');
        var end = map.Find('E');

        var distCommon = Mark(start, map);

        commonWeight = distCommon[end];

        var result = 0L;

        var dotes = distCommon.OrderBy(x => x.Value).Select(x => x.Key).ToArray();

        foreach (var dote in dotes)
        {
            var dist = Dijkstra(dote, map);
            foreach (var d in dist)
            {
                var item = map.Get(d.Key);
                if (item != '.' && item != 'E') continue;
                
                var delta = distCommon[d.Key] - (distCommon[dote] + d.Value);
                if (delta > 0 && delta >= seconds) result++;
            }
        }

        result.Should().Be(expected);
    }

    private static long commonWeight = long.MaxValue;

    private static Dictionary<Point, long> Mark(Point start, char[][] map)
    {
        var result = new Dictionary<Point, long>();
        var p = 0L;
        var current = start;

        result[start] = 0L;
        
        while (true)
        {
            var next = Extensions
                .GetVerticalHorizontalNeighbours(map, current)
                .Where(x => x.Item != '#')
                .Single(x => !result.ContainsKey(x.Index));

            result[next.Index] = ++p;

            current = next.Index;
            
            if (next.Item == 'E') break;
        }

        return result;
    }

    private Dictionary<Point, long> Dijkstra(Point start, char[][] map)
    {
        var dist = new Dictionary<Point, long>();
        var marked = new HashSet<Point>();
        
        var pq = new PriorityQueue<Point, long>();
        pq.Enqueue(start, 0);
        dist[start] = 0;

        while (pq.Count > 0)
        {
            var v = pq.Dequeue();
            if (!marked.Add(v)) continue;

            if (dist[v] >= 20) continue;
            if (!v.Equals(start) && (map.Get(v) == '.' || map.Get(v) == 'E')) continue;

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

    private IEnumerable<(Point Position, int Weight)> GetNextV(char[][] map,
        Point fromPoint, HashSet<Point> marked)
    {
        foreach (var next in Extensions.GetVerticalHorizontalNeighboursDirections(map, fromPoint))
        {
            if (marked.Contains(next.Index)) continue;
            if (map.Get(fromPoint) == '.' && next.Item == '.') continue;

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