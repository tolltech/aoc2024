using System.Text.RegularExpressions;
using System.Xml.Xsl;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task05
{
    [Test]
    [TestCase(
        @"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47",
        143)]
    [TestCase(@"Task05.txt", 0)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var lines = input.SplitLines();
        var rules = new Dictionary<int, List<int>>();
        foreach (var line in lines.Where(x => x.Contains("|")))
        {
            var splits = line.SplitEmpty("|").Select(int.Parse).ToArray();
            var (left, right) =  (Left: splits[0], Right: splits[1]);

            if (!rules.ContainsKey(left)) rules[left] = new List<int>();
            
            rules[left].Add(right);
        }

        var linesForCheck = lines.Where(x => x.Contains(",")).ToArray();

        var result = 0;

        foreach (var line in linesForCheck)
        {
            var vals = line.SplitEmpty(",").Select(int.Parse).ToArray();
            for (var i = 1; i < vals.Length; i++)
            {
                var prev = vals[i - 1];
                var cur = vals[i];

                if (!IsRightOrder(prev, cur, rules)) break;

                if (i == vals.Length - 1) result += vals[vals.Length / 2];
            }
        }

        result.Should().Be(expected);
    }

    private bool IsRightOrder(int left, int right, Dictionary<int,List<int>> rules)
    {
        if (!rules.TryGetValue(left, out var rule)) return false;
        
        if (rule.Any(x=>x == right)) return true;
        
        return false;
    }
}