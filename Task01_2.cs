using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task01_2
{
    [Test]
    [TestCase(
        @"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet",
        142)]
    [TestCase(
        @"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen",
        281)]
    [TestCase(@"Task01.txt", 55686)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var numbers = new[]
            {
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine"
            }.Select((s, i) => (s, i + 1))
            .ToDictionary(x => x.s, x => x.Item2);

        var result = 0;
        foreach (var line in input.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var first = line.Select((c, i) => (c, i)).FirstOrDefault(x => char.IsDigit(x.c));
            var last = line.Select((c, i) => (c, i)).LastOrDefault(x => char.IsDigit(x.c));

            foreach (var number in numbers)
            {
                var firstIndex = line.IndexOf(number.Key, StringComparison.Ordinal);
                if ((firstIndex < first.i || first.c == default) && firstIndex != -1)
                {
                    first = (number.Value.ToString()[0], firstIndex);
                }
            }

            foreach (var number in numbers)
            {
                var lastIndex = line.LastIndexOf(number.Key, StringComparison.Ordinal);
                if ((lastIndex > last.i || last.c == default) && lastIndex != -1)
                {
                    last = (number.Value.ToString()[0], lastIndex);
                }
            }

            result += int.Parse($"{first.c}{last.c}");
        }

        result.Should().Be(expected);
    }
}