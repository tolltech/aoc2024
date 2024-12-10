using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task10
{
    [Test]
    [TestCase(
        @"89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732",
        36)]
    [TestCase(@"Task10.txt", 548)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        map = input.SplitLines().Select(x => x.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
        var nines = new List<Point>();

        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (map[i][j] == 0) nines.Add(new Point(i, j));
        }

        var scores = new Dictionary<Point, HashSet<Point>>();
        foreach (var trailHead in nines)
        {
            Dfs(trailHead, trailHead, scores);
        }

        scores.Values.Sum(x=>x.Count).Should().Be(expected);
    }

    private void Dfs(Point trailHead, Point current, Dictionary<Point, HashSet<Point>> scores)
    {
        var val = map[current.Row][current.Col];

        if (val == 9)
        {
            if (!scores.ContainsKey(trailHead)) scores.Add(trailHead, new HashSet<Point>());

            scores[trailHead].Add(current);

            return;
        }

        foreach (var neighbour in Extensions.GetVerticalHorizontalNeighbours(map, current))
        {
            if (neighbour.Item != val + 1) continue;

            Dfs(trailHead, neighbour.Index, scores);
        }
    }

    [ThreadStatic] private static int[][] map;

    private class Block
    {
        public int Id { get; set; }
        public int Cnt { get; set; }
        public bool IsEmpty { get; set; }
    }
}