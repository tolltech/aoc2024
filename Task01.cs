using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task01
{
    [Test]
    [TestCase(
        @"3   4
4   3
2   5
1   3
3   9
3   3",
        11)]
    [TestCase(@"Task01.txt", 1722302)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;

        var lines = input.SplitLines();

        var left = new List<int>();
        var right = new List<int>();

        foreach (var line in lines)
        {
            var split = line.SplitEmpty(" ").Select(int.Parse).ToArray();
            left.Add(split[0]);
            right.Add(split[1]);
        }

        var leftStack = new Stack<int>(left.OrderBy(x => x));
        var rightStack = new Stack<int>(right.OrderBy(x => x));

        while (leftStack.Count > 0 && rightStack.Count > 0)
        {
            result += Math.Abs(leftStack.Pop() - rightStack.Pop());
        }

        result.Should().Be(expected);
    }
}