using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task09_2
{
    [Test]
    [TestCase(
        @"2333133121414131402",
        2858)]
    [TestCase(@"Task09.txt", 6327174563252L)]
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

        var root = new Node
        {
            Prev = null,
            Block = blocks[0]
        };

        var currentNode = root;

        for (var i = 1; i < blocks.Length; i++)
        {
            var newNode = new Node
            {
                Block = blocks[i],
                Prev = currentNode,
            };
            currentNode.Next = newNode;
            currentNode = newNode;
        }

        var nodesReverse = root.FlatNode().Reverse().ToArray();
        
        var dbg3 = string.Join(string.Empty, root.Flat().Select(x => x.IsEmpty ? $" ({(x.Cnt)}) " : $" {x.Id}:{x.Cnt} "));

        foreach (var nodeToReplace in nodesReverse.Where(x => !x.Block.IsEmpty))
        {
            Node suitableNode = null;

            foreach (var n in root.FlatNode())
            {
                if (n == nodeToReplace) break;
                
                if (n.Block.IsEmpty && n.Block.Cnt >= nodeToReplace.Block.Cnt)
                {
                    suitableNode = n;
                    break;
                }
            }
            
            if (suitableNode == null) continue;

            nodeToReplace.Block.IsEmpty = true;

            if (suitableNode.Block.Cnt == nodeToReplace.Block.Cnt)
            {
                suitableNode.Block.Id = nodeToReplace.Block.Id;
                suitableNode.Block.IsEmpty = false;
            }
            else
            {
                var newEmptyNode = new Node
                {
                    Block = new Block
                    {
                        IsEmpty = true,
                        Cnt = suitableNode.Block.Cnt - nodeToReplace.Block.Cnt,
                    }
                };

                var tmp = suitableNode.Next;
                suitableNode.Next = newEmptyNode;
                newEmptyNode.Next = tmp;
                newEmptyNode.Prev = suitableNode;

                suitableNode.Block.Id = nodeToReplace.Block.Id;
                suitableNode.Block.IsEmpty = false;
                suitableNode.Block.Cnt = nodeToReplace.Block.Cnt;
            }

            var dbg = string.Join(string.Empty,
                root.Flat().Select(x => x.IsEmpty ? $" ({(x.Cnt)}) " : $" {x.Id}:{x.Cnt} "));
        }

        var dbg2 = string.Join(string.Empty, root.Flat().Select(x => x.IsEmpty ? $" ({(x.Cnt)}) " : $" {x.Id}:{x.Cnt} "));
        var sum = 0L;

        var node = root;
        var index = 0;
        while (node != null)
        {
            if (node.Block.IsEmpty)
            {
                index += node.Block.Cnt;
                node = node.Next;
                continue;
            }

            for (var i = 0; i < node.Block.Cnt; i++)
            {
                sum += (long)node.Block.Id * (index + i);
            }

            index += node.Block.Cnt;
            node = node.Next;
        }

        sum.Should().Be(expected);
    }

    private class Block
    {
        public int Id { get; set; }
        public int Cnt { get; set; }
        public bool IsEmpty { get; set; }
    }

    [DebuggerDisplay("{Block.Id}:{Block.Cnt}:{Block.IsEmpty}")]
    private class Node
    {
        public Block Block { get; set; }

        public Node Prev { get; set; }
        public Node Next { get; set; }

        public IEnumerable<Block> Flat()
        {
            var node = this;
            while (node != null)
            {
                yield return node.Block;
                node = node.Next;
            }
        }

        public IEnumerable<Node> FlatNode()
        {
            var node = this;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }
    }
}