using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task19
{
    [Test]
    [TestCase(@"r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb", 6)]
    [TestCase(@"Task19.txt", 315)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var stripes = input.SplitLines().First().SplitEmpty(", ");

        var result = 0;
        foreach (var line in input.SplitLines().Skip(1))
        {
            if (IsPossible(line, stripes))
            {
                result++;
            }
        }

        result.Should().Be(expected);
    }

    private bool IsPossible(string line, string[] stripes)
    {
        foreach (var stripe in stripes)
        {
            if (line == stripe) return true;

            if (line.StartsWith(stripe))
            {
                var newLine = line.Substring(stripe.Length);
                if (IsPossible(newLine, stripes)) return true;
            }
        }

        return false;
    }
}