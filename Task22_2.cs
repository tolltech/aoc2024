using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task22_2
{
    [Test]
    [TestCase(@"1
2
3
2024", 23)]
    [TestCase(@"Task22.txt", 1628)]
    [TestCase(@"123", 0, Ignore = "dsa")]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var lines = input.SplitLines().Select(long.Parse).ToArray();

        var buyers = new List<Dictionary<string, int>>();
        foreach (var line in lines)
        {
            var newResult = line;
            buyers.Add(new Dictionary<string, int>());
            var queue = new Queue<long>();
            for (var i = 0; i < 2000; i++)
            {
                var buyer = buyers.Last();

                if (queue.Count == 4)
                {
                    var key = queue.JoinToString();
                    var val = (int)newResult % 10;

                    if (!buyer.TryGetValue(key, out _))
                    {
                        buyer[key] = val;
                    }
                }

                var tmp = Next(newResult);
                queue.Enqueue(tmp % 10 - newResult % 10);
                if (queue.Count > 4) queue.Dequeue();

                newResult = tmp;
            }
        }

        var result = 0L;

        var keys = buyers.SelectMany(x => x.Keys).Distinct().ToArray();
        var maxSum = 0L;
        foreach (var key in keys)
        {
            var keyMaxSum = 0;
            foreach (var buyer in buyers)
            {
                if (buyer.TryGetValue(key, out var val))
                    keyMaxSum += val;
            }

            if (keyMaxSum > maxSum) maxSum = keyMaxSum;
        }

        maxSum.Should().Be(expected);
    }

    private (long StartIndex, long Period, long Number) GetPeriodInfo(long number, long cnt,
        out Dictionary<int, long> numbers)
    {
        var mem = new Dictionary<long, int>();

        numbers = new Dictionary<int, long>();

        mem[number] = 0;
        numbers[0] = number;
        var current = number;
        for (var i = 1; i <= cnt; i++)
        {
            current = Next(current);
            if (mem.TryGetValue(current, out var ind))
            {
                return (ind, i - ind, current);
            }

            mem[current] = i;
            numbers[i] = current;
        }

        return (0, long.MaxValue, number);
    }

    [Test]
    [TestCase(123, @"15887950
16495136
527345
704524
1553684
12683156
11100544
12249484
7753432
5908254")]
    public void TestTen(long start, string expected)
    {
        var result = new List<long>();
        var current = start;
        for (var i = 0; i < 10; ++i)
        {
            current = Next(current);
            result.Add(current);
        }

        var expectedInts = expected.SplitLines().Select(int.Parse).ToArray();

        result.ToArray().Should().BeEquivalentTo(expectedInts);
    }

    private static long Next(long current)
    {
        var result = current * 64;
        current = Prune(Mix(result, current));

        result = current / 32;
        current = Prune(Mix(result, current));

        result = current * 2048;
        current = Prune(Mix(result, current));

        return current;
    }

    private static long Mix(long x, long y) => x ^ y;
    private static long Prune(long x) => x % 16777216;
}