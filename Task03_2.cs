using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task03_2
{
    [Test]
    [TestCase(
        @"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))",
        48)]
    [TestCase(@"Task03.txt", 74838033)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;

        var muls = Regex.Matches(input, @"mul\(\d{1,3},\d{1,3}\)");
        var dos = Regex.Matches(input, @"do\(\)");
        var donts = Regex.Matches(input, @"don't\(\)");


        var totalMatches = muls.Concat(dos).Concat(donts).OrderBy(x => x.Index).ToArray();

        var todo = true;
        foreach (var match in totalMatches)
        {
            if (match.Value.Contains("mul"))
            {
                if (!todo) continue;
                
                var splits = match.Value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new string(x.Where(char.IsDigit).ToArray()))
                    .Select(int.Parse)
                    .ToArray();

                result += splits[0] * splits[1];
            }
            else if (match.Value.Contains("don't"))
            {
                todo = false;
            }
            else
            {
                todo = true;
            }
        }

        result.Should().Be(expected);
    }
}