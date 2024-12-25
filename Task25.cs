using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task25
{
    [Test]
    [TestCase(@"#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####", 3)]
    [TestCase(@"Task25.txt", 0)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var blocks = input.SplitEmpty($"{Environment.NewLine}{Environment.NewLine}")
            .Select(x => x.SplitLines())
            .ToArray();

        var locks = blocks.Where(x => x[0].All(c => c == '#'))
            .Select(x => GetHeights(x).ToArray())
            .ToArray();

        var keys = blocks.Where(x => x.Last().All(c => c == '#'))
            .Select(x => GetHeights(x).ToArray())
            .ToArray();

        var result = 0;

        foreach (var l in locks)
        foreach (var k in keys)
        {
            if (l.Select((x, i) => x + k[i]).Any(x => x > 5)) continue;
            result++;
        }

        result.Should().Be(expected);
    }

    private IEnumerable<int> GetHeights(string[] block)
    {
        for (var i = 0; i < block[0].Length; i++)
        {
            yield return block.Count(r => r[i] == '#') - 1;
        }
    }
}