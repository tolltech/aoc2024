﻿using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task21_2
{
    [Test]
    [TestCase(@"<0^2^^>9vvvA
^^^9<8vvv0>A
^<<1^^7>>9vvvA
^^<<4>5>6vvA
^3<<^^7>>9vvvA", 126384)]
    [TestCase(@"^<<1^4>vv0>A
^<<1^>>6^9vvvA
^<<1^^7>vvv0>A
^^<5v2^^8vvv>A
^3^<<4>vv0>A
", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var commands = GetAllCommands().ToArray();


        var result = 0L;
        foreach (var line in input.SplitLines())
        {
            var suitableCommands = new List<string>();
            foreach (var expectedChar in line)
            {
                var minCommand = GetCommands(expectedChar, commands, suitableCommands).Distinct().OrderBy(x => x.Length)
                    .ToArray();

                suitableCommands.Add(minCommand.First());
            }

            var resultCommand = suitableCommands.JoinToString();

            result += ((long)resultCommand.Length) * int.Parse(new string(line.Where(char.IsDigit).ToArray()));
        }

        result.Should().Be(expected);
    }

    private IEnumerable<string> GetCommands(char target, string[] commands, List<string> suitableCommands)
    {
        foreach (var command1 in commands)
        foreach (var command2 in commands)
        foreach (var command3 in commands)
        foreach (var command4 in commands)
        {
            var keyBoard = InitKeyboards();

            foreach (var command in suitableCommands)
            {
                ExecCommand(command, keyBoard);
            }

            var bigCommand = command1 + command2 + command3 + command4;

            for (var i = 0; i < bigCommand.Length; i++)
            {
                var c = bigCommand[i];

                var r = keyBoard.Exec(c);

                if (r == target)
                {
                    yield return bigCommand.Substring(0, i + 1);
                    break;
                }

                if (r != default) break;
            }
        }
    }

    //^^^9<8vvv0>A
    // ^<<1^^7>>9vvvA
    // ^^<<4>5>6vvA
    // ^3<<^^7>>9vvvA

    // 179A: <v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    // 456A: <v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A
    // 379A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A

    [Test]
    // [TestCase("029A","<A^A>^^AvvvA", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A")]
    // [TestCase("980A","^^^A<AvvvA>A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A")]
    // [TestCase("179A","^<<A^^A>>AvvvA", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]
    // [TestCase("456A","^^<<A>A>AvvA", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A")]
    // [TestCase("379A","^A<<^^A>>AvvvA", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]

    //^<<1^4>vv0>A
    // ^<<1^>>6^9vvvA
    // ^<<1^^7>vvv0>A
    // ^^<5v2^^8vvv>A
    // ^3^<<4>vv0>A
    [TestCase("140A", "^<<A^A>vvA>A", 70)] //70 !!! 9800
    [TestCase("140A", "^<<A^Av>vA>A", 76)] //76
    [TestCase("169A", "^<<A^>>A^AvvvA", 76)] //76 !!! 12844
    [TestCase("169A", "^<<A>^>A^AvvvA", 82)] //82
    [TestCase("169A", "^<<A>>^A^AvvvA", 76)] //76
    [TestCase("170A", "^<<A^^A>vvvA>A", 72)] //72 !!! 12240
    [TestCase("170A", "^<<A^^Avv>vA>A", 78)] //78
    [TestCase("170A", "^<<A^^Av>vvA>A", 78)] //78
    [TestCase("528A", "^^<AvA^^Avvv>A", 74)] //74
    [TestCase("528A", "<^^AvA^^Avvv>A", 70)] //70 !!! 36960
    [TestCase("528A", "<^^AvA^^A>vvvA", 74)] //74
    [TestCase("528A", "<^^AvA^^Av>vvA", 80)] //80
    [TestCase("528A", "<^^AvA^^Avv>vA", 80)] //80
    [TestCase("528A", "^^<AvA^^Avv>vA", 84)] //84
    [TestCase("340A", "^A^<<A>vvA>A", 70)] //70 
    [TestCase("340A", "^A<<^A>vvA>A", 66)] // 66 !!! 22440
    [TestCase("340A", "^A<^<A>vvA>A", 78)] // 78
    [TestCase("340A", "^A^<<Av>vA>A", 76)] //76
    //94284
    [TestCase("1", "^<<A", 26)] //v<<A>>^Av<A<A>>^AAvAA<^A>A
    public void Command(string expectedOutput, string command, long expected)
    {
        var commands = command.Split('A');

        var result = 0L;
        var cache = new Dictionary<(string, int), long>();
        for (var ci = 0; ci < commands.Length - 1; ci++)
        {
            var cmd = commands[ci];
            result += GetLength(cmd, 25, cache);
        }

        result.Should().Be(expected);
    }

    [Test]
    [TestCase(2, 94284)]
    [TestCase(25, 0)]
    [TestCase(2, 126384, false)]
    public void SuperCommand(int deep, long expected, bool prod = true)
    {
        var input = prod
            ? new[]
            {
                ("140A", "^<<A^A>vvA>A"), //^<<A^A>vvA>A
                ("169A", "^<<A^>>A^AvvvA"), //^<<A^>>A^AvvvA
                ("170A", "^<<A^^A>vvvA>A"), //^<<A^^A>vvvA>A
                ("528A", "<^^AvA^^Avvv>A"), //<^^AvA^^Avvv>A
                ("340A", "^A^<<A>vvA>A"),
            }
            : new[]
            {
                ("029A", "<A^A^^>AvvvA"),
                ("980A", "^^^A<AvvvA>A"),
                ("179A", "^<<A^^A>>AvvvA"),
                ("456A", "^^<<A>A>AvvA"),
                ("379A", "^A^^<<A>>AvvvA"),
            };

        input = input.SelectMany(Mutate).ToArray();

        var resultDict = new Dictionary<string, long>();
        var minCommands = new Dictionary<string, string>();

        var cache = new Dictionary<(string, int), long>();

        CommandsMutated = Promote();

        foreach (var tuple in input)
        {
            var commands = tuple.Item2.Split('A');

            var result = 0L;
            for (var ci = 0; ci < commands.Length - 1; ci++)
            {
                var cmd = commands[ci];
                result += GetLength(cmd, deep, cache);
            }

            if (!resultDict.TryGetValue(tuple.Item1, out var val) || val > result)
            {
                resultDict[tuple.Item1] = result;
                minCommands[tuple.Item1] = tuple.Item2;
            }
        }

        var realResult = 0L;

        foreach (var d in resultDict)
        {
            var x = long.Parse(new string(d.Key.Where(char.IsDigit).ToArray()));
            realResult += x * d.Value;
        }

        realResult.Should().Be(expected);
    }

    private Dictionary<string, HashSet<string>> Promote()
    {
        var result = new Dictionary<string, HashSet<string>>();

        result[""] = ["A"];

        var baseChars = new[] { '<', '>', '^', 'v' };
        var srcCommands = new HashSet<string>();

        foreach (var c1 in baseChars)
        foreach (var c2 in baseChars)
        foreach (var c3 in baseChars)
        foreach (var c4 in baseChars)
        {
            srcCommands.Add(c1.ToString() + c2 + c3 + c4);
            srcCommands.Add(c1.ToString() + c2 + c3);
            srcCommands.Add(c1.ToString() + c2);
            srcCommands.Add(c1.ToString());
        }

        foreach (var srcCommand in srcCommands)
        {
            var mutations = MutateSrcCommand(srcCommand).ToHashSet();
            result[srcCommand] = mutations;
        }

        return result;
    }

    private IEnumerable<string> MutateSrcCommand(string srcCommand)
    {
        var command = $"A{srcCommand}A";

        var resultPaths = new string[][]
        {
            ["E"],
            ["E"],
            ["E"],
            ["E"],
            ["E"],
        };

        for (var i = 1; i < command.Length; i++)
        {
            var paths = GetPaths(command[i - 1], command[i]);
            resultPaths[i - 1] = paths.ToArray();
        }

        //max 5
        foreach (var p0 in resultPaths[0])
        foreach (var p1 in resultPaths[1])
        foreach (var p2 in resultPaths[2])
        foreach (var p3 in resultPaths[3])
        foreach (var p4 in resultPaths[4])
        {
            yield return new[] { p0, p1, p2, p3, p4 }.Where(x => x != "E").JoinToString("A") + "A";
        }
    }


    //("140A", "^<<A^A>vvA>A"),//^<<A^A>vvA>A
    // ("169A", "^<<A^>>A^AvvvA"),//^<<A^>>A^AvvvA
    // ("170A", "^<<A^^A>vvvA>A"),//^<<A^^A>vvvA>A
    // ("528A", "<^^AvA^^Avvv>A"),//<^^AvA^^Avvv>A
    // ("340A", "^A^<<A>vvA>A"),
    private IEnumerable<(string, string)> Mutate((string Number, string Buttons) command)
    {
        var commands = command.Buttons.SplitEmpty("A");
        var newCommands = new List<List<string>>();
        for (var i = 0; i < commands.Length; i++)
        {
            var cmd = commands[i];
            var position = i == 0 ? 'A' : command.Number[i - 1];
            var target = command.Number[i];

            var cmds = Extensions.Permute(cmd.ToArray()).Select(x => new string(x.ToArray())).Distinct().ToArray();

            var mutated = new List<string>();
            foreach (var mutateCmd in cmds)
            {
                var keyboard = new NumericKeyboard();
                keyboard.SetPosition(position);

                foreach (var c in mutateCmd)
                {
                    var r = keyboard.Exec(c);
                    if (r == 'E') break;
                }

                if (keyboard.GetPosition() == target)
                    mutated.Add(mutateCmd);
            }

            newCommands.Add(mutated);
        }

        foreach (var c0 in newCommands[0])
        foreach (var c1 in newCommands[1])
        foreach (var c2 in newCommands[2])
        foreach (var c3 in newCommands[3])
        {
            yield return (command.Number, $"{c0}A{c1}A{c2}A{c3}A");
        }
    }

    private long GetLength(string cmd, int deepLevel, Dictionary<(string, int), long> cache)
    {
        if (cache.TryGetValue((cmd, deepLevel), out var r))
        {
            return r;
        }

        if (deepLevel == 0) return cmd.Length + 1L;

        var nextCommands = CommandsMutated[cmd];
        
        var result = long.MaxValue;

        foreach (var nextCommand in nextCommands)
        {
            var internalResult = 0L;
            var cmds = nextCommand.Split('A');
            for (var i = 0; i < cmds.Length - 1; i++)
            {
                var nextCmd = cmds[i];
                internalResult += GetLength(nextCmd, deepLevel - 1, cache);
            }

            if (internalResult < result) result = internalResult;
        }
        
        cache[(cmd, deepLevel)] = result;

        return result;
    }

    private static Dictionary<string, HashSet<string>> CommandsMutated = new();

    private static readonly Dictionary<string, string> Commands = new()
    {
        { "", "A" },
        { "<", "v<<A>>^A" },
        { "v", "v<A>^A" },
        { "^", "<A>A" },
        { ">", "vA^A" },
        { "<<", "v<<AA>>^A" },
        { ">v", "vA<A>^A" },
        { ">>", "vAA^A" },
        { ">^", "vA<^A>A" },
        { "v<", "v<A<A>>^A" },
        { "<^", "v<<A>^A>A" },
        { "v<<", "v<A<AA>>^A" },
        { "^<<", "<Av<AA>>^A" },
        { ">>^", "vAA<^A>A" },
        { "vvv", "v<AAA>^A" },
        { ">^^", "vA<^AA>A" },
        { "^^^", "<AAA>A" },
        { "^^", "<AA>A" },
        { "vv", "v<AA>^A" },
        { "<<^^", "v<<AA>^AA>A" },
        { "^^<<", "<AAv<AA>>^A" },
        { ">vv", "vA<AA>^A" },
        { "v>v", "v<A>A<A>^A" },
        { "^>>", "<A>vAA^A" },
        { ">^>", "vA<^A>vA^A" },
        { "^^<", "<AAv<A>>^A" },
        { "<<^", "v<<AA>^A>A" },
        { "vvv>", "v<AAA>A^A" },
        { "<^^", "v<<A>^AA>A" },
        { ">vvv", "vA<AAA>^A" },
        { "vv>v", "v<AA>A<A>^A" },
        { "v>vv", "v<A>A<AA>^A" },
        { "<^<", "v<<A>^Av<A>>^A" },
        { "^<^", "<Av<A>^A>A" },
        { "^^>", "<AA>vA^A" },
        { "^>^", "<A>vA<^A>A" },
        { "^<^<", "<Av<A>^Av<A>>^A" },
        { "^<<^", "<Av<AA>^A>A" },
        { "<^^<", "v<<A>^AAv<A>>^A" },
        { "<^<^", "v<<A>^Av<A>^A>A" },
    };

    private static IEnumerable<string> GetAllCommands()
    {
        var verticals = new[] { "", "v", "^" };
        var horizontals = new[] { "", ">", ">>", "<", "<<" };

        var commands = new[]
        {
            "",
            "<",
            "v",
            "^",
            ">",
            "<<",
            ">v",
            ">>",
            ">^",
            "v<",
            "^<",
            "<v<",
            ">>^",
        };

        var aS = new[] { "A", "AA", "AAA" };

        foreach (var horizontal in commands)
        foreach (var a in aS)
        {
            yield return $"{horizontal}{a}";
        }
    }

    //029A: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
    //980A: <v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A
    //179A: <v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    //456A: <v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A
    //379A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A

    [Test]
    [TestCase("<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A", "029A")]
    [TestCase("v<A<AA>>^AvAA^<A>A<v<A>>^AvA^A<v<A>>^AAvA<A>^A<A>Av<A<A>>^AAA<A>vA^A", "029A")]
    [TestCase("<vA<AA>>^AvAA<^A>A<vA<A>^A>AvA^A", "02")]
    [TestCase("<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A", "980A")]
    [TestCase("<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A", "179A")]
    [TestCase("<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A", "456A")]
    [TestCase("<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A", "379A")]
    public void Dbg(string command, string expected)
    {
        var firstKeyboard = InitKeyboards();

        var result = ExecCommand(command, firstKeyboard);

        new string(result.Where(c => char.IsDigit(c) || c == 'A').ToArray()).Should().Be(expected);
    }

    [TestCase("^>>>", @"^>>>
>^>>
>>^>
>>>^")]
    [TestCase("^>^>", @"^>^>
>^^>
>>^^
>^>^
^^>>
^>>^")]
    public void TestPermute(string str, string expected)
    {
        var mutations = Extensions.Permute(str.ToArray());
        var actual = mutations.Select(x => new string(x.ToArray())).Distinct().ToArray();
        actual.OrderBy(x => x).ToArray().Should().BeEquivalentTo(expected.SplitLines().OrderBy(x => x).ToArray());
    }

    private static string ExecCommand(string command, Keyboard keyboard)
    {
        var result = string.Empty;
        foreach (var c in command)
        {
            var r = keyboard.Exec(c);

            if (r == 'E') throw new Exception("Dsdsad");

            if (r != default)
                result += r;
        }

        return result;
    }

    private static Keyboard InitKeyboards()
    {
        var firstKeyboard = new Keyboard();
        var currentKeyboard = firstKeyboard;
        for (var i = 0; i < 1; ++i)
        {
            var nextKeyboard = new Keyboard();
            currentKeyboard.Next = nextKeyboard;
            currentKeyboard = nextKeyboard;
        }

        Numeric = new NumericKeyboard();
        return firstKeyboard;
    }

    private static NumericKeyboard Numeric;

    private class NumericKeyboard
    {
        private char[][] Map { get; } =
        [
            ['7', '8', '9'],
            ['4', '5', '6'],
            ['1', '2', '3'],
            [default, '0', 'A']
        ];

        private Point Position { get; set; } = (3, 2);

        public void SetPosition(char c)
        {
            var point = Map.Find(c);
            Position = point;
        }

        public char GetPosition()
        {
            return Map.Get(Position);
        }

        public char Exec(char c)
        {
            Point step;
            switch (c)
            {
                case '<':
                    step = LeftStep;
                    break;
                case '>':
                    step = RightStep;
                    break;
                case '^':
                    step = UpStep;
                    break;
                case 'v':
                    step = DownStep;
                    break;
                case 'A': return Map.Get(Position);
                default: throw new Exception("key unknown");
            }

            if (Map.SafeGet(Position + step) == default) return 'E';

            Position += step;

            return c;
        }
    }

    public static IEnumerable<string> GetPaths(char fromChar, char toChar)
    {
        if (fromChar == toChar)
        {
            yield return "";
            yield break;
        }

        switch (fromChar, toChar)
        {
            case ('A', '^'):
                yield return "<";
                break;
            case ('A', '<'):
                yield return "<v<";
                yield return "v<<";
                break;
            case ('A', '>'):
                yield return "v";
                break;
            case ('A', 'v'):
                yield return "v<";
                yield return "<v";
                break;

            case ('^', 'A'):
                yield return ">";
                break;
            case ('^', '<'):
                yield return "v<";
                break;
            case ('^', '>'):
                yield return "v>";
                yield return ">v";
                break;
            case ('^', 'v'):
                yield return "v";
                break;

            case ('<', '^'):
                yield return ">^";
                break;
            case ('<', 'A'):
                yield return ">>^";
                yield return ">^>";
                break;
            case ('<', '>'):
                yield return ">>";
                break;
            case ('<', 'v'):
                yield return ">";
                break;

            case ('>', '^'):
                yield return "<^";
                yield return "^<";
                break;
            case ('>', '<'):
                yield return "<<";
                break;
            case ('>', 'A'):
                yield return "^";
                break;
            case ('>', 'v'):
                yield return "<";
                break;

            case ('v', '^'):
                yield return "^";
                break;
            case ('v', '<'):
                yield return "<";
                break;
            case ('v', '>'):
                yield return ">";
                break;
            case ('v', 'A'):
                yield return ">^";
                yield return "^>";
                break;

            default: throw new Exception("Dsaddsa");
        }
    }

    public static readonly (int Row, int Col) DownStep = (1, 0);
    public static readonly (int Row, int Col) LeftStep = (0, -1);
    public static readonly (int Row, int Col) UpStep = (-1, 0);
    public static readonly (int Row, int Col) RightStep = (0, 1);

    private class Keyboard
    {
        private char[][] Map { get; set; } =
        [
            [default, '^', 'A'],
            ['<', 'v', '>']
        ];

        private Point Position { get; set; } = (0, 2);

        public Keyboard Next { get; set; }

        public char Exec(char c)
        {
            Point step;
            switch (c)
            {
                case '<':
                    step = LeftStep;
                    break;
                case '>':
                    step = RightStep;
                    break;
                case '^':
                    step = UpStep;
                    break;
                case 'v':
                    step = DownStep;
                    break;
                case 'A':
                    if (Next != null) return Next.Exec(Map.Get(Position));
                    else return Numeric.Exec(Map.Get(Position));
                default: throw new Exception("key unknown");
            }

            if (Map.SafeGet(Position + step) == default) return 'E';

            Position += step;

            return default;
        }
    }
}