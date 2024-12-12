using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task11
{
    [Test]
    [TestCase(@"0 1 10 99 999", 1, 7)]
    [TestCase(@"125 17", 6, 22)]
    [TestCase(@"125 17", 25, 55312)]
    [TestCase(@"Task11.txt", 25, 228668)]
    public void Task(string input, int count, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var stones = input.SplitEmpty().Select(long.Parse).ToArray();

        for (var i = 0; i < count; i++)
        {
            var newStones = new List<long>(stones.Length * 2);

            foreach (var stone in stones)
            {
                var stoneStr = stone.ToString();
                if (stone == 0)
                {
                    newStones.Add(1);
                }
                else if (stoneStr.Length % 2 == 0)
                {
                    var left = stoneStr.Substring(0, stoneStr.Length / 2);
                    var right = stoneStr.Substring(stoneStr.Length / 2);
                    
                    newStones.Add(long.Parse(left));
                    newStones.Add(long.Parse(right));
                }
                else
                {
                    newStones.Add(stone * 2024);
                }
            }
            
            //var dCnt = newStones.Distinct().Count();
            
            stones = newStones.ToArray();
        }

        stones.Length.Should().Be(expected);
    }
}