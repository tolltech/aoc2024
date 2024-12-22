using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task22
{
    [Test]
    [TestCase(@"1
10
100
2024", 2000, 37327623)]
    [TestCase(@"Task22.txt", 2000, 14392541715)]
    public void Task(string input, int cnt, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var lines = input.SplitLines().Select(long.Parse).ToArray();

        var result = 0L;

        foreach (var line in lines)
        {
            var periodInfo = GetPeriodInfo(line, cnt, out var dict);
            
            var reminder = cnt - periodInfo.StartIndex;
            var newCnt = reminder % periodInfo.Period;
            // var newResult = periodInfo.Number;
            // for (var i = 0; i < newCnt; i++)
            // {
            //     newResult = Next(newResult);
            // }
            
            result += dict[(int)newCnt];
        }

        result.Should().Be(expected);
    }

    private (long StartIndex, long Period, long Number) GetPeriodInfo(long number, long cnt, out Dictionary<int, long> numbers)
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