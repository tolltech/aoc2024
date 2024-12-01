using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task01_2
{
    [Test]
    [TestCase(
        @"3   4
4   3
2   5
1   3
3   9
3   3",
        31)]
    [TestCase(@"Task01.txt", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();

        var left = new List<int>();
        var right = new List<int>();

        foreach (var line in lines)
        {
            var split = line.SplitEmpty(" ").Select(int.Parse).ToArray();
            left.Add(split[0]);
            right.Add(split[1]);
        }
        
        var rightStack = right.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        foreach (var x in left)
        {
            result += x * rightStack.SafeGet(x);
        }

        result.Should().Be(expected);
    }
}