using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task11_2
{
    [Test]
    [TestCase(@"0 1 10 99 999", 1, 7)]
    [TestCase(@"125 17", 6, 22)]
    [TestCase(@"125 17", 25, 55312)]
    [TestCase(@"Task11.txt", 25, 228668)]
    [TestCase(@"Task11.txt", 75, 270673834779359)]
    public void Task(string input, int count, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var stones = input.SplitEmpty().Select(long.Parse).ToArray();

        var cash = new Dictionary<(long, int), long>();

        var result = 0L;

        foreach (var stone in stones)
        {
            result += Calculate(stone, count, cash);
        }

        result.Should().Be(expected);
    }

    private long Calculate(long stone, int count, Dictionary<(long, int), long> cash)
    {
        if (cash.TryGetValue((stone, count), out var cashed))
        {
            return cashed;
        }

        var stoneStr = stone.ToString();

        long result;
        if (count == 0)
        {
            result = 1;
        }
        else if (stone == 0)
        {
            result = Calculate(1, count - 1, cash);
        }
        else if (stoneStr.Length % 2 == 0)
        {
            var left = stoneStr.Substring(0, stoneStr.Length / 2);
            var right = stoneStr.Substring(stoneStr.Length / 2);


            result = Calculate(long.Parse(left), count - 1, cash)
                     + Calculate(long.Parse(right), count - 1, cash);
        }
        else
        {
            result = Calculate(stone * 2024, count - 1, cash);
        }

        cash[(stone, count)] = result;
        return result;
    }
}