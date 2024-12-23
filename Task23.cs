using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task23
{
    [Test]
    [TestCase(@"kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn", 7)]
    [TestCase(@"Task23.txt", 1358)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;
        var lines = input.SplitLines();

        var comps = new Dictionary<string, HashSet<string>>();

        foreach (var line in lines)
        {
            var splits = line.SplitEmpty("-");
            for (var i = 0; i < splits.Length; i++)
            {
                var comp = splits[i];
                if (!comps.ContainsKey(comp))
                {
                    comps[comp] = [];
                }

                comps[comp].Add(splits[(i + 1) % 2]);
            }
        }
        
        var used = new HashSet<string>();

        foreach (var comp in comps)
        {
            if (used.Contains(comp.Key)) continue;

            var sets = new List<string[]>();

            GetSet(comps, comp.Key, sets, 3, null, null);

            foreach (var set in sets)
            {
                if (set.Length != 3) continue;
                if (set.All(x => !x.StartsWith("t"))) continue;

                used.Add(set.JoinToString());
            }
        }

        used.Count.ToLong().Should().Be(expected);
    }

    private static void GetSet(Dictionary<string, HashSet<string>> comps, string comp, List<string[]> sets,
        int setCount, List<string> visited, HashSet<string> visitedHash)
    {
        if (visited == null) visited = [];
        if (visitedHash == null) visitedHash = new HashSet<string>();

        if (visited.Count > setCount) return;

        if (visited.Count > 0 && visited[0] == comp)
        {
            if (visited.Count == setCount) sets.Add(visited.OrderBy(x => x).ToArray());
            return;
        }

        if (visitedHash.Contains(comp)) return;

        visited.Add(comp);
        visitedHash.Add(comp);

        foreach (var next in comps[comp])
        {
            GetSet(comps, next, sets, setCount, visited, visitedHash);
        }

        visitedHash.Remove(comp);
        visited.RemoveAt(visited.Count - 1);
    }
}