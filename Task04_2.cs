using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task04_2
{
    [Test]
    [TestCase(
        @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX",
        9)]
    [TestCase(@"Task04.txt", 1854)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var matrix = input.SplitLines().Select(x => x.ToArray()).ToArray();

        var result = 0;

        for (var i = 0; i < matrix.Length; i++)
        for (var j = 0; j < matrix.GetLength(0); j++)
        {
            result += Check(matrix, i, j);
        }

        result.Should().Be(expected);
    }

    private int Check(char[][] matrix, int i, int j)
    {
        var c = matrix[i][j];

        if (c != 'A') return 0;

        var chars = Extensions.GetDiagonalNeighbours(matrix, (i, j)).ToArray();

        if (chars.Any(x => x.Item is 'X' or 'A')) return 0;
        if (chars.GroupBy(x => x.Item).Any(x => x.Count() > 2)) return 0;

        var first = chars.First();
        var opposite = chars.SingleOrDefault(x =>
            Math.Abs(x.Index.Col - first.Index.Col) == 2 && Math.Abs(x.Index.Row - first.Index.Row) == 2);

        if (opposite == default) return 0;

        if (first.Item == opposite.Item)
            return 0;

        return 1;
    }
}