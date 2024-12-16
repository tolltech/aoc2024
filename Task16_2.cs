using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task16_2
{
    [Test]
    [TestCase(@"###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############", 45)]
    [TestCase(@"#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################", 64)]
    [TestCase(@"Task16.txt", 609)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var map = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var deer = map.Find('S');
        var target = map.Find('E');

        var dist = Dijkstra(deer, map, out var prevs);

        var minTarget = new[]
            {
                (target, UpStep),
                (target, DownStep),
                (target, LeftStep),
                (target, RightStep)
            }.OrderBy(x => dist[x])
            .First();

        var result = 0L;

        var queue = new Queue<(Point, Point)>();
        queue.Enqueue(minTarget);
        var hash = new HashSet<(Point, Point)>();

        while (queue.Count > 0)
        {
            var q = queue.Dequeue();
            if (!hash.Add(q)) continue;

            if (dist[q] == 0) continue;

            foreach (var next in prevs[q])
            {
                if (hash.Contains(next)) continue;
                queue.Enqueue(next);
            }
        }

        // var dbg = Extensions.PrintMap(map, (c, p) =>
        // {
        //     if (hash.Select(x => x.Item1).Contains(p)) return "O";
        //     return c.ToString();
        // });

        ((long)hash.Select(x=>x.Item1).Distinct().Count()).Should().Be(expected);
    }

    private Dictionary<(Point Position, Point Direction), long> Dijkstra(Point start, char[][] map,
        out Dictionary<(Point Position, Point Direction), HashSet<(Point Position, Point Direction)>> prevs)
    {
        var dist = new Dictionary<(Point Position, Point Direction), long>();
        var marked = new HashSet<(Point Position, Point Direction)>();

        var pq = new PriorityQueue<(Point Position, Point Direction), long>();

        prevs = new Dictionary<(Point Position, Point Direction), HashSet<(Point Position, Point Direction)>>();

        dist[(start, RightStep)] = 0;

        pq.Enqueue((start, RightStep), 0);

        while (pq.Count > 0)
        {
            var v = pq.Dequeue();
            if (!marked.Add(v)) continue;

            var nextVs = GetNextV(map, v, marked);

            foreach (var nextV in nextVs)
            {
                var weight = nextV.Weight;
                if (!dist.ContainsKey(nextV.Point) || dist[nextV.Point] >= dist[v] + weight)
                {
                    if (!prevs.ContainsKey(nextV.Point))
                    {
                        prevs[nextV.Point] = [];
                    }

                    prevs[nextV.Point].Add(v);

                    dist[nextV.Point] = dist[v] + weight;
                    pq.Enqueue(nextV.Point, dist[nextV.Point]);
                }
            }
        }

        return dist;
    }

    private IEnumerable<((Point Position, Point Direction) Point, int Weight)> GetNextV(char[][] map,
        (Point Position, Point Direction) fromPoint, HashSet<(Point Position, Point Direction)> marked)
    {
        var step = fromPoint.Position + fromPoint.Direction;
        var stepItem = map.SafeGet(step);

        if (stepItem == '.' || stepItem == 'E') yield return ((step, fromPoint.Direction), 1);

        yield return ((fromPoint.Position, GetClockwise(fromPoint.Direction)), 1000);
        yield return ((fromPoint.Position, GetCounterClockwise(fromPoint.Direction)), 1000);
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