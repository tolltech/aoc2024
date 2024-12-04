using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task03
{
    [Test]
    [TestCase(
        @"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))",
        161)]
    [TestCase(@"Task03.txt", 170807108)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;

        var matches = Regex.Matches(input, @"mul\(\d{1,3},\d{1,3}\)");

        foreach (Match match in matches)
        {
            var splits = match.Value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new string(x.Where(char.IsDigit).ToArray()))
                .Select(int.Parse)
                .ToArray();

            result += splits[0] * splits[1];
        }

        result.Should().Be(expected);
    }
}