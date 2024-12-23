using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task23_2
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
td-yn", "co,de,ka,ta")]
    [TestCase(@"Task23.txt", "cl,ei,fd,hc,ib,kq,kv,ky,rv,vf,wk,yx,zf")]
    public void Task(string input, string expected)
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

        var all = comps.SelectMany(comp => comp.Value).Distinct().ToHashSet();
        var clicks = new List<HashSet<string>>();
        extend(comps, all, new HashSet<string>(), clicks, new HashSet<string>());

        var result = clicks.OrderBy(x => x.Count).Last().OrderBy(x => x).JoinToString(",");
        result.Should().Be(expected);
    }

    //ПРОЦЕДУРА extend (candidates, not):
    // ПОКА candidates НЕ пусто И not НЕ содержит вершины, СОЕДИНЕННОЙ СО ВСЕМИ вершинами из candidates, 
    // ВЫПОЛНЯТЬ:
    // 1 Выбираем вершину v из candidates и добавляем её в compsub
    // 2 Формируем new_candidates и new_not, удаляя из candidates и not вершины, не СОЕДИНЕННЫЕ с v
    // 3 ЕСЛИ new_candidates и new_not пусты
    // 4 ТО compsub – клика
    // 5 ИНАЧЕ рекурсивно вызываем extend (new_candidates, new_not)
    // 6 Удаляем v из compsub и candidates, и помещаем в not

    private void extend(Dictionary<string, HashSet<string>> comps, HashSet<string> passedCandidates,
        HashSet<string> passedNot, List<HashSet<string>> clicks, HashSet<string> compSub)
    {
        var candidates = passedCandidates.ToHashSet();
        var not = passedNot.ToHashSet();
        while (candidates.Count > 0 && !not.Any(n => candidates.All(x => comps[n].Contains(x))))
        {
            var v = candidates.First();
            compSub.Add(v);

            var newCandidates = candidates.Where(c => comps[c].Contains(v)).ToHashSet();
            var newNot = not.Where(c => comps[c].Contains(v)).ToHashSet();

            if (newCandidates.Count == 0 && newNot.Count == 0)
            {
                clicks.Add(compSub.ToHashSet());
            }
            else
            {
                extend(comps, newCandidates, newNot, clicks, compSub);
            }

            compSub.Remove(v);
            candidates.Remove(v);
            not.Add(v);
        }
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