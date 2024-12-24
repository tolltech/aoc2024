using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task24_2
{
    [Test]
    [TestCase(@"x00: 1
x01: 0
x02: 1
x03: 1
x04: 0
y00: 1
y01: 1
y02: 1
y03: 1
y04: 1

ntg XOR fgs -> mjb
y02 OR x01 -> tnw
kwq OR kpj -> z05
x00 OR x03 -> fst
tgd XOR rvg -> z01
vdt OR tnw -> bfw
bfw AND frj -> z10
ffh OR nrd -> bqk
y00 AND y03 -> djm
y03 OR y00 -> psh
bqk OR frj -> z08
tnw OR fst -> frj
gnj AND tgd -> z11
bfw XOR mjb -> z00
x03 OR x00 -> vdt
gnj AND wpb -> z02
x04 AND y00 -> kjc
djm OR pbm -> qhw
nrd AND vdt -> hwm
kjc AND fst -> rvg
y04 OR y02 -> fgs
y01 AND x02 -> pbm
ntg OR kjc -> kwq
psh XOR fgs -> tgd
qhw XOR tgd -> z09
pbm OR djm -> kpj
x03 XOR y03 -> ffh
x00 XOR y04 -> ntg
bfw OR bqk -> z06
nrd XOR fgs -> wpb
frj XOR qhw -> z04
bqk OR frj -> z07
y03 OR x01 -> nrd
hwm AND bqk -> z03
tgd XOR rvg -> z12
tnw OR pbm -> gnj", 2024)]
    [TestCase(@"Task24.txt", 55114892239566)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var values = new Dictionary<string, bool>();

        var conditions = new Dictionary<string, (string Left, string Right, string Op)>();

        foreach (var line in input.SplitLines())
        {
            if (line.Contains(':'))
            {
                var splits = line.SplitEmpty(": ");
                values[splits[0]] = int.Parse(splits[1]) == 1;
            }
            else
            {
                var splits = line.Split(" -> ");
                var pairsInput = splits[0].SplitEmpty(" ");

                conditions.Add(splits[1], (pairsInput[0], pairsInput[2], pairsInput[1]));
            }
        }

        var zs = conditions.Where(x => x.Key.StartsWith("z")).Select(x => x.Key)
            .OrderBy(x => x).ToArray();

        var result = new StringBuilder();

        foreach (var z in zs)
        {
            result.Append(GetValue(z, conditions, values) ? '1' : '0');
        }

        var resultStr = new string(result.ToString().Reverse().ToArray());

        var resultLong = resultStr.ToLongFromBin();
        resultLong.Should().Be(expected);
    }

    private bool GetValue(string s, Dictionary<string, (string Left, string Right, string Op)> conditions,
        Dictionary<string, bool> values)
    {
        if (values.TryGetValue(s, out var r)) return r;

        var condition = conditions[s];

        switch (condition.Op)
        {
            case "AND":
                return GetValue(condition.Left, conditions, values) && GetValue(condition.Right, conditions, values);
            case "OR":
                return GetValue(condition.Left, conditions, values) || GetValue(condition.Right, conditions, values);
            case "XOR":
                return GetValue(condition.Left, conditions, values) ^ GetValue(condition.Right, conditions, values);
            default: throw new Exception("dsf");
        }
    }
}