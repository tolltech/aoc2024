using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task01
{
    [Test]
    [TestCase(
        @"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet",
        142)]
    [TestCase(@"Task01.txt", 55029)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;
        foreach (var line in input.Split(new[]{"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries))
        {
            result += int.Parse($"{line.First(char.IsDigit)}{line.Last(char.IsDigit)}");                
        }

        result.Should().Be(expected);
    }
}