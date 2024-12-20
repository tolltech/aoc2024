using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task20
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
###############", 12, 8)]
    [TestCase(@"Task20.txt", 100, 0)]
    public void Task(string input, long seconds, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var start = map.Find('S');
        var end = map.Find('E');

        var distCommon = Dijkstra(start, map, null);
        commonWeight = distCommon[end];

        var result = 0L;

        for (var i = 0; i < map.Length; ++i)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (map[i][j] != '#') continue;
            //if ((i, j) != (1, 8)) continue;

            foreach (var step in Clocwise)
            {
                var dist = Dijkstra(start, map, (new Point(i, j), step));
                if (!dist.TryGetValue(end, out var value)) continue;
                var delta = commonWeight - value;
                if (delta >= seconds) result++;
            }
        }

        result.Should().Be(expected);
    }

    private static long commonWeight = long.MaxValue;
    
    private Dictionary<Point, long> Dijkstra(Point start, char[][] map, (Point Poistion, Point Step)? cheat)
    {
        var dist = new Dictionary<Point, long>();
        var marked = new HashSet<Point>();

        var pq = new PriorityQueue<Point, long>();

        dist[start] = 0;

        var maxMarkedWeight = 0L;

        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            var v = pq.Dequeue();
            if (!marked.Add(v)) continue;

            if (dist[v] > maxMarkedWeight) maxMarkedWeight = dist[v];
            if (maxMarkedWeight > commonWeight) return dist;

            var nextVs = GetNextV(map, v, marked, cheat);

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
        Point fromPoint, HashSet<Point> marked, (Point Poistion, Point Step)? cheat)
    {
        foreach (var next in Extensions.GetVerticalHorizontalNeighboursDirections(map, fromPoint))
        {
            if (marked.Contains(next.Index)) continue;

            if (next.Item is '.' or 'E' or 'S')
            {
                yield return (next.Index, 1);
                continue;
            }

            if (!cheat.HasValue) continue;

            if (cheat.Value.Poistion.Equals(next.Index) && cheat.Value.Step.Equals(next.Direction))
            {
                yield return (next.Index, 1);
            }
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