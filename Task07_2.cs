using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task07_2
{
    [Test]
    [TestCase(
        @"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20",
        11387)]
    [TestCase(@"Task07.txt", 149956401519484L)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var lines = input.SplitLines();

        var result = 0L;

        foreach (var line in lines)
        {
            var splits = line.SplitEmpty(":");
            var target = long.Parse(splits[0]);
            var ops = splits[1].SplitEmpty(" ").Select(long.Parse).ToArray();

            var results = Operate(ops, ops[0], 1);
            if (results.Contains(target))
                result += target;
        }

        result.Should().Be(expected);
    }

    private long[] Operate(long[] ops, long currentResult, int currentIndex)
    {
        var current = ops[currentIndex];
        if (currentIndex == ops.Length - 1)
        {
            return [currentResult + current, currentResult * current, long.Parse(currentResult.ToString() + current)];
        }

        return Operate(ops, currentResult * current, currentIndex + 1)
            .Concat(Operate(ops, currentResult + current, currentIndex + 1))
            .Concat(Operate(ops, long.Parse(currentResult.ToString() + current), currentIndex + 1))
            .ToArray();
    }
}