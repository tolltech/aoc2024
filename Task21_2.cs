using System.Text;
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
                var minCommand = GetCommands(expectedChar, commands, suitableCommands).Distinct().OrderBy(x => x.Length).ToArray();
            
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
    
    [TestCase("140A","^<<A^A>vvA>A", 70)]//70 !!! 9800
    [TestCase("140A","^<<A^Av>vA>A", 76)]//76
    
    [TestCase("169A","^<<A^>>A^AvvvA", 76)]//76 !!! 12844
    [TestCase("169A","^<<A>^>A^AvvvA", 82)]//82
    [TestCase("169A","^<<A>>^A^AvvvA", 76)]//76
    
    [TestCase("170A","^<<A^^A>vvvA>A", 72)]//72 !!! 12240
    [TestCase("170A","^<<A^^Avv>vA>A", 78)]//78
    [TestCase("170A","^<<A^^Av>vvA>A", 78)]//78
    
    [TestCase("528A","^^<AvA^^Avvv>A", 74)]//74
    [TestCase("528A","<^^AvA^^Avvv>A", 70)]//70 !!! 36960
    [TestCase("528A","<^^AvA^^A>vvvA", 74)]//74
    [TestCase("528A","<^^AvA^^Av>vvA", 80)]//80
    [TestCase("528A","<^^AvA^^Avv>vA", 80)]//80
    [TestCase("528A","^^<AvA^^Avv>vA", 84)]//84
    
    [TestCase("340A","^A^<<A>vvA>A", 70)]//70 
    [TestCase("340A","^A<<^A>vvA>A", 66)]// 66 !!! 22440
    [TestCase("340A","^A<^<A>vvA>A", 78)]// 78
    [TestCase("340A","^A^<<Av>vA>A", 76)]//76
    //94284
    [TestCase("1","^<<A", 0)]//70 !!! 9800
    public void Command(string expectedOutput, string command, int expected)
    {
        var oldCommand = command.ToList();
        for (var i = 0; i < 25; ++i)
        {
            var newCommand = new List<char>(oldCommand.Count);

            var skipCount = 0;
            while (skipCount < oldCommand.Count)
            {
                var cmd = new string(oldCommand.Skip(skipCount).TakeWhile(x => x != 'A').ToArray());
                if (cmd.Length == 0) cmd = "";
                
                newCommand.AddRange(Commands[cmd]);

                skipCount += cmd.Length + 1;
            }

            oldCommand = newCommand;
        }

        var keyboard = InitKeyboards();
        var output = ExecCommand(new string(oldCommand.ToArray()), keyboard);

        var realOutput = new string(output.Where(c => char.IsDigit(c) || c == 'A').ToArray());
        realOutput.Should().Be(expectedOutput);
        oldCommand.Count.Should().Be(expected);
    }

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