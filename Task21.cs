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

        var firstKeyboard = new Keyboard();
        var currentKeyboard = firstKeyboard;
        for (var i = 0; i < 1; ++i)
        {
            var nextKeyboard = new Keyboard();
            currentKeyboard.Next = nextKeyboard;
            currentKeyboard = nextKeyboard;
        }
        
        

        0L.Should().Be(expected);
    }

    //029A: <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
    //980A: <v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A
    //179A: <v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    //456A: <v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A
    //379A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
    
    [Test]
    [TestCase("<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A", "029A")]
    [TestCase("<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A", "980A")]
    [TestCase("<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A", "179A")]
    [TestCase("<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A", "456A")]
    [TestCase("<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A", "379A")]
    public void Dbg(string command, string expected)
    {
        var firstKeyboard = new Keyboard();
        var currentKeyboard = firstKeyboard;
        for (var i = 0; i < 1; ++i)
        {
            var nextKeyboard = new Keyboard();
            currentKeyboard.Next = nextKeyboard;
            currentKeyboard = nextKeyboard;
        }

        var result = string.Empty;
        foreach (var c in command)
        {
            var r = firstKeyboard.Exec(c);

            if (r == 'E') throw new Exception("Dsdsad");
            
            if (r != default)
                result += r;
        }

        new string(result.Where(c => char.IsDigit(c) || c == 'A').ToArray()).Should().Be(expected);
    }   

    private static readonly NumericKeyboard Numeric = new();
    
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