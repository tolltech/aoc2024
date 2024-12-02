using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task02
{
    [Test]
    [TestCase(
        @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9",
        2)]
    [TestCase(@"Task02.txt", 549)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;

        var lines = input.SplitLines();

        foreach (var line in lines)
        {
            var levels = line.SplitEmpty(" ").Select(int.Parse).ToArray();
            var delta = levels[1] - levels[0];

            if (!CheckSafe(delta, null)) continue;

            var i = 1;
            while (++i < levels.Length)
            {
                var prev = levels[i - 1];
                var current = levels[i];

                var newDelta = current - prev;
                if (!CheckSafe(newDelta, delta)) break;

                delta = newDelta;
            }

            if (i == levels.Length) result++;
        }

        result.Should().Be(expected);
    }

    private static bool CheckSafe(int delta, int? prevDelta)
    {
        if (Math.Abs(delta) > 3) return false;
        if (delta == 0) return false;
        if (prevDelta.HasValue && Math.Sign(prevDelta.Value) != Math.Sign(delta)) return false;

        return true;
    }
}