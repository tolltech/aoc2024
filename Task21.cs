using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task21
{
    [Test]
    [TestCase(@"029A
980A
179A
456A
379A", 126384)]
    [TestCase(@"140A
169A
170A
528A
340A
", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var commands = GetAllCommands().ToArray();
        var suitableCommands = new List<string>();

        var expectedCommand = "<0^2";//^^>9vvvA
        foreach (var expectedChar in expectedCommand)
        {
            var minCommand = GetCommands(expectedChar, commands, suitableCommands).Distinct().OrderBy(x => x.Length).ToArray();
            
            suitableCommands.Add(minCommand.First());
        }

        var resultCommand = suitableCommands.JoinToString();
        0L.Should().Be(expected);
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

    private static IEnumerable<string> GetAllCommands()
    {
        var verticals = new[] { "", "v", "^" };
        var horizontals = new[] { "", ">", ">>", "<", "<<" };
        var aS = new[] { "A", "AA", "AAA" };
        
        foreach (var horizontal in horizontals)
        foreach (var vertical in verticals) 
        foreach (var a in aS)
        {
            yield return $"{horizontal}{vertical}{a}";
        }
    }
    
    //029A: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
    //980A: <v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A
    //179A: <v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    //456A: <v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A
    //379A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    
    [Test]
    [TestCase("<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A", "029A")]
    [TestCase("<vA<AA>>^AvAA<^A>A<vA<A>^A>AvA^A", "02")]
    [TestCase("<vA<AA>>^AvAA<^A>A<vA<A>^A>AvA^A<vA<A>^A>AAvA<A>^A<A>A<vA<A>>^AAAvA<^A>A", "029A")]
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