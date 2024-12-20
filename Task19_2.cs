using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task19_2
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
bbrgwb", 16)]
    [TestCase(@"Task19.txt", 315)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var stripes = input.SplitLines().First().SplitEmpty(", ");

        var result = 0L;
        foreach (var line in input.SplitLines().Skip(1))
        {
            result += IsPossible(line, stripes);
        }

        result.Should().Be(expected);
    }

    private static Dictionary<string, long> cash = new();

    private long IsPossible(string line, string[] stripes)
    {
        if (cash.TryGetValue(line, out var possible)) return possible;
        
        var result = 0L;
        foreach (var stripe in stripes)
        {
            if (line == stripe)
            {
                result += 1;
                continue;
            }

            if (line.StartsWith(stripe))
            {
                var newLine = line.Substring(stripe.Length);
                result += IsPossible(newLine, stripes);
            }
        }
        
        cash.Add(line, result);
        
        return result;
    }
}