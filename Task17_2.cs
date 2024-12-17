using System.Diagnostics;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task17_2
{
    [Test]
    [TestCase(@"Register A: 2024
Register B: 0
Register C: 0

Program: 0,3,5,4,3,0", 117440)]
    [TestCase(@"Register A: 117440
Register B: 0
Register C: 0

Program: 0,3,5,4,3,0", 117440)]
    [TestCase(@"Task17.txt", 0)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var lines = input.SplitLines();

        foreach (var reg in new[] { "A", "B", "C" })
        {
            var line = lines.Single(x => x.Contains(reg));
            var r = int.Parse(line.SplitEmpty(":")[1]);
            if (reg == "A") A = r;
            if (reg == "B") B = r;
            if (reg == "C") C = r;
        }

        var program = lines.Last().SplitEmpty(":")[1].SplitEmpty(",").Select(int.Parse).ToArray();

        var output = Exec(program);

        var newA = 0;
        newA.Should().Be(expected);
    }

    [Test]
    public void TestDbg()
    {
        var program = new int[] { 2, 4, 1, 1, 7, 5, 1, 5, 4, 1, 5, 5, 0, 3, 3, 0 };
        var result = new List<string>();
        for (var a = 0; a < 10000; ++a)
        {
            A = a;
            B = 0;
            C = 0;

            var outputs = Exec(program);
            result.Add(outputs.JoinToString(","));
        }

        var s = result.JoinToString("\r\n");
    }

    [Test]
    [TestCase("2,6", "0,0,9", "0,1,9", "")]
    [TestCase("5,0,5,1,5,4", "10,0,0", "10,0,0", "0,1,2")]
    [TestCase("0,1,5,4,3,0", "2024,0,0", "0,0,0", "4,2,5,6,7,7,7,7,3,1,0")]
    [TestCase("1,7", "0,29,0", "0,26,0", "")]
    [TestCase("4,0", "0,2024,43690", "0,44354,43690", "")]
    public void TestOps(string programStr, string abc, string expectedAbc, string expectedOutput)
    {
        var program = programStr.SplitEmpty(",").Select(int.Parse).ToArray();
        var abcSplit = abc.SplitEmpty(",").Select(int.Parse).ToArray();
        A = abcSplit[0];
        B = abcSplit[1];
        C = abcSplit[2];

        var output = Exec(program);

        output.JoinToString(",").Should().Be(expectedOutput);
        $"{A},{B},{C}".Should().Be(expectedAbc);
    }

    [Test]
    [TestCase("2,6", "0,0,9", "0,1,9", "")]
    [TestCase("5,0,5,1,5,4", "10,0,0", "10,0,0", "0,1,2")]
    [TestCase("0,1,5,4,3,0", "2024,0,0", "0,0,0", "4,2,5,6,7,7,7,7,3,1,0")]
    [TestCase("1,7", "0,29,0", "0,26,0", "")]
    [TestCase("4,0", "0,2024,43690", "0,44354,43690", "")]
    public void TestOpsAlt(string programStr, string abc, string expectedAbc, string expectedOutput)
    {
        var program = programStr.SplitEmpty(",").Select(int.Parse).ToArray();
        var abcSplit = abc.SplitEmpty(",").Select(int.Parse).ToArray();
        A = abcSplit[0];
        B = abcSplit[1];
        C = abcSplit[2];

        var output = Exec(program).ToArray();

        output.JoinToString(",").Should().Be(expectedOutput);
        $"{A},{B},{C}".Should().Be(expectedAbc);
    }

    [Test]
    public void TestDbgAlt()
    {
        //var s = new List<long>();
        //var accs = new[] { 4, 5, 2, 6, 4, 4, 4, 0 };
        //var accs = new[] { 4, 5, 5, 6 };
        //var accs = new[] { 4, 5, 5, 0, 5, 3, 3, 5 };
        //var accs = new[] { 4, 5, 5, 0, 5, 3, 3, 1, 5 };
        // var accs = new[] { 4, 5, 5, 0, 5, 3, 3, 1, 3 };
        // var acc = 0L;
        // for (var i = 0; i < accs.Length; ++i)
        // {
        //     acc += accs[accs.Length - i - 1] << 3 * i;
        // }

        //2,4,1,1,7,5,1,5,4,1,5,5,0,3,3,0
        //                            4 5
        var accs = new List<long>();

        //var f2 = Find(new[] { 4L, 5, 2, 6, 4, 4, 4, 1, 3, 3 }.ToList());
        
        var f = Find(accs);
        
        var acc = 0L;
        for (var i = 0; i < accs.Count; ++i)
        {
            acc += accs[accs.Count - i - 1] << 3 * i;
            var ss = Convert.ToString(acc, 2);
        }

        acc.Should().Be(164278764924605L);
    }

    private bool Find(List<long> accs)
    {
        if (accs.Count == 16)
            return true;
        
        var acc = 0L;
        for (var i = 0; i < accs.Count; ++i)
        {
            acc += accs[accs.Count - i - 1] << 3 * i;
            var ss = Convert.ToString(acc, 2);
        }

        var suitable = new List<int>();
        for (var i = 0; i <= 7; i++)
        {
            var a = (acc << 3) + i;
            var output = ExecAlt(a).ToArray();
            var programSlice = myProgram.TakeLast(output.Length).JoinToString();
            var outputStr = output.JoinToString();

            if (output.Last() == 1)
            {
                var sss = "dsa";
            }
            
            if (programSlice == outputStr) suitable.Add(i);
        }

        if (suitable.Count == 0)
            return false;

        foreach (var s in suitable)
        {
            accs.Add(s);
            if (Find(accs)) return true;
            accs.RemoveAt(accs.Count - 1);
        }

        return false;
    }

    private static readonly int[] myProgram = new[] { 2, 4, 1, 1, 7, 5, 1, 5, 4, 1, 5, 5, 0, 3, 3, 0 };

    private static List<int> Exec(int[] program)
    {
        //var sb = new StringBuilder();
        var output = new List<int>();
        var pointer = 0;
        while (pointer < program.Length)
        {
            //sb.AppendLine($"{A}\t{B}\t{C}\t{program[pointer]}\t{OpCodes[program[pointer]]}\t{GetOp(program[pointer + 1])}\t{output.JoinToString(",")}\t{program.JoinToString(",")}");
            var opCode = program[pointer];
            var operand = program[pointer + 1];

            switch (opCode)
            {
                case 0:
                    A = (int)(A / Math.Pow(2, GetOp(operand)));
                    break;
                case 1:
                    B = B ^ operand;
                    break;
                case 2:
                    B = GetOp(operand) % 8;
                    break;
                case 3:
                    if (A != 0)
                    {
                        pointer = operand;
                        continue;
                    }

                    break;
                case 4:
                    B = B ^ C;
                    break;
                case 5:
                    output.Add((int)(GetOp(operand) % 8));
                    break;
                case 6:
                    B = (int)(A / Math.Pow(2, GetOp(operand)));
                    break;
                case 7:
                    C = (int)(A / Math.Pow(2, GetOp(operand)));
                    break;
                default: throw new Exception("switch");
            }

            pointer += 2;
        }

        //var ss = sb.ToString();
        return output;
    }

    private static IEnumerable<int> ExecAlt(long a)
    {
        do
        {
            var b = (a % 8) ^ 1;
            var c = a / pow(2, b);
            b = (b ^ 5) ^ c;

            yield return (int)(b % 8);
            a /= 8;
        } while (a != 0);
    }

    private static long pow(int x, long y)
    {
        var p = 1;
        for (var i = 0; i < y; ++i)
        {
            p *= x;
        }

        return p;
    }

    private static readonly Dictionary<int, string> OpCodes = new()
    {
        { 0, "advA" },
        { 1, "bxl" },
        { 2, "bst" },
        { 3, "jnz" },
        { 4, "bxc" },
        { 5, "out" },
        { 6, "bdv" },
        { 7, "cdv" },
    };

    private static long A;
    private static long B;
    private static long C;

    private static long GetOp(int op)
    {
        if (op is >= 0 and <= 3)
        {
            return op;
        }

        if (op == 4) return A;
        if (op == 5) return B;
        if (op == 6) return C;

        throw new Exception("Reg");
    }
}