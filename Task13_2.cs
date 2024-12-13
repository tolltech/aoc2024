using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task13_2
{
    [Test]
    [TestCase(@"Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279", 480)]
    [TestCase(@"Task13.txt", 37680)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var blocks = input.SplitEmpty($"{Environment.NewLine}{Environment.NewLine}");

        var result = 0L;

        foreach (var block in blocks)
        {
            var lines = block.SplitLines();
            var cond = new long[2][];
            for (var i = 0; i < cond.Length; i++)
            {
                cond[i] = new long[3];
            }

            var ac = GetInput(lines[0]);
            cond[0][0] = ac.left;
            cond[1][0] = ac.right;

            var bc = GetInput(lines[1]);
            cond[0][1] = bc.left;
            cond[1][1] = bc.right;

            var cc = GetInput(lines[2]);
            cond[0][2] = 10000000000000 + cc.left;
            cond[1][2] = 10000000000000 + cc.right;

            var (a, b) = Solve(cond);

            //if (a <= 100 && b <= 100)
                result += 3 * a + b;
        }

        result.Should().Be(expected);
    }

    private (long a, long b) Solve(long[][] cond)
    {
        var d = cond[0][0] * cond[1][1] - cond[0][1] * cond[1][0];
        if (d == 0)
        {
            throw new Exception("dsaasd");
            return (0, 0);
        }

        var da = cond[1][1] * cond[0][2] - cond[0][1] * cond[1][2];
        var db = cond[0][0] * cond[1][2] - cond[0][2] * cond[1][0];

        if (da % d != 0 || db % d != 0)
        {
            return (0, 0);
        }

        return (da / d, db / d);
    }

    private (long left, long right) GetInput(string src)
    {
        var x = new string(src.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());

        var skip = src.IndexOf(x, StringComparison.Ordinal) + x.Length;

        var y = new string(src.Skip(skip).SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
        return (long.Parse(x), long.Parse(y));
    }
}