using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task04
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
        18)]
    [TestCase(@"Task04.txt", 2557)]
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
        const string xmas = "XMAS";

        var c = matrix[i][j].ToString();

        var words = Extensions.GetAllNeighbours(matrix, (i, j), 3).Select(x => c + new string(x)).ToArray();
        return words.Count(x => x == xmas);
    }
}