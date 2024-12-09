using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task09
{
    [Test]
    [TestCase(
        @"2333133121414131402",
        1928)]
    [TestCase(@"Task09.txt", 6307275788409)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        // var blocks = input!.Select((c, i) => new Block { Id = i, Cnt = int.Parse(c.ToString()), IsEmpty = i % 2 == 1 })
        //     .ToArray();
        var blocks = new Block[input!.Length];

        var blockId = -1;
        for (var i = 0; i < input.Length; i++)
        {
            var cnt = int.Parse(input[i].ToString());
            var isEmpty = i % 2 == 1;
            if (!isEmpty) blockId++;

            blocks[i] = new Block { Id = blockId, Cnt = cnt, IsEmpty = isEmpty };
        }
        
        var result = new List<int>(10 * input.Length);

        foreach (var block in blocks)
        {
            for (var i = 0; i < block.Cnt; i++)
            {
                if (block.IsEmpty) result.Add(-1);
                else result.Add(block.Id);
            }
        }

        var skippedInds = new Queue<int>(result.Select((x, i) => (x, i)).Where(x => x.x == -1).Select(x => x.i));
        
        for (var i = result.Count - 1; i >= 0; i--)
        {
            var block = result[i];
            if (block == -1) continue;

            if (skippedInds.Count == 0) break;
            var ind = skippedInds.Dequeue();
            
            if (ind >= i) break;
            
            result[ind] = block;
            result[i] = -1;

            var dbg = string.Join(string.Empty, result.Select(x => x == -1 ? "." : x.ToString()));
        }

        var dbg2 = string.Join(string.Empty, result.Select(x => x == -1 ? "." : x.ToString()));
        var sum = 0L;
        for (var index = 0; index < result.Count; index++)
        {
            var block = result[index];

            if (block == -1) break;
            
            sum += (long)block * index;
        }
        
        sum.Should().Be(expected);
    }

    private class Block
    {
        public int Id { get; set; }
        public int Cnt { get; set; }
        public bool IsEmpty { get; set; }
    }
}