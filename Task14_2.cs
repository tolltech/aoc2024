using System.Data.Common;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task14_2
{
    [Test]
    [TestCase(@"Task14.txt", 101, 103, 100, 6876)]
    public void Task(string input, int wide, int tall, int seconds, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var robots = input.SplitLines().Select(line =>
            {
                var split = line.SplitEmpty(" ");
                var robot = new Robot();

                var p = split[0].Replace("p=", "").SplitEmpty(",").Select(int.Parse).ToArray();

                robot.Start = new Point(p[1], p[0]);
                robot.Position = new Point(p[1], p[0]);
                
                var v = split[1].Replace("v=", "").SplitEmpty(",").Select(int.Parse).ToArray();

                robot.Velocity = new Point(v[1], v[0]);

                robot.MapSize = new Point(tall, wide);
                
                return robot;
            })
            .ToArray();

        var dbs = new List<string>();
        for (var i = 0; i < 10000; i++)
        {
            foreach (var robot in robots)
            {
                robot.Move(i);
            }

            var dbg = Dbg(robots);
            dbs.Add(dbg);
        }

        var s = new StringBuilder();
        for (var index = 0; index < dbs.Count; index++)
        {
            var db = dbs[index];

            if (!db.Contains("*********"))
            {
                continue;
            }
            
            s.AppendLine($"{index}");
            s.AppendLine();
            s.AppendLine($"{db}");
            s.AppendLine();
        }
//6876
        var ss = s.ToString();

        var qCnt = robots.GroupBy(x => x.GetQuadrant()).Where(x => x.Key != -1).Select(x => x.Count()).ToArray();

        var result = (long)qCnt.Aggregate(1, (x, y) => x * y);
        
        6876L.Should().Be(expected);
    }

    private string Dbg(Robot[] robots)
    {
        var mapSize = robots.First().MapSize;
        var lines = new char[mapSize.Row][];
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = Enumerable.Range(0, mapSize.Col).Select(x => ' ').ToArray();
        }

        foreach (var robot in robots)
        {
            lines[robot.Position.Y][robot.Position.X] = '*';
        }

        return lines.Select(x => new string(x)).JoinToString("\r\n");
    }

    class Robot
    {
        public Point Start { get; set; }
        public Point Position { get; set; }
        public Point Velocity { get; set; }

        public Point MapSize { get; set; }

        private int GetPosition(int start, int velocity, int size, int seconds)
        {
            var p = start + velocity * seconds;

            var d = Math.Abs(p) % size;
            var sign = Math.Sign(p);
            
            return (size + d * sign) % size;
        }

        public Point Move(int seconds)
        {
            var position = new Point(
                GetPosition(Start.Y, Velocity.Y, MapSize.Y, seconds),
                GetPosition(Start.X, Velocity.X, MapSize.X, seconds)
            );

            Position = position;
            
            return position;
        }

        public int GetQuadrant()
        {
            var middleX = MapSize.X / 2;
            var middleY = MapSize.Y / 2;
            
            var newX = Position.X - middleX;
            var newY = Position.Y - middleY;

            if (newX == 0 || newY == 0) return -1;

            if (newX < 0 && newY < 0) return 1;
            if (newX < 0 && newY > 0) return 2;
            if (newX > 0 && newY > 0) return 3;
            if (newX > 0 && newY < 0) return 4;

            throw new Exception("dsdsdsdsd");
        }
    }
}