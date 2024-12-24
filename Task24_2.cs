using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2024;

[TestFixture]
public class Task24_2
{
    [Test]
    [TestCase(@"Task24_2.txt", "cdj,dhm,gfm,mrb,qjd,z08,z16,z32")]
    public void Task(string input, string expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        ParseInput(input, out _, out var conditions);

        var zs = conditions.Where(x => x.Key.StartsWith("z")).Select(x => x.Key)
            .OrderBy(x => x).ToArray();

        //z08 cdj
        //z32 qfm
        //qjd dhm

        var stranges = new HashSet<string>();
        
        for (var i = 0; i <= 44; ++i)
        {
            var zLeftCond = conditions.Single(x =>
                x.Value.Source.Contains($"x{i:00}")
                && x.Value.Source.Contains($"y{i:00}")
                && x.Value.Source.Contains($"XOR")
            );

           var target = zLeftCond.Key;
           if (target.StartsWith("z")) continue;
           var connectedConditions = conditions.Where(x => x.Value.Left == target || x.Value.Right == target).ToArray();
           foreach (var c in connectedConditions)
           {
               if (c.Value.Op == "OR") stranges.Add(target);
           }
        }
        
        for (var i = 0; i <= 44; ++i)
        {
            var zRightCond = conditions.Single(x =>
                x.Key == $"z{i:00}"
            );
            
            if (zRightCond.Value.Op != "XOR") stranges.Add(zRightCond.Key);
        }
        
        for (var i = 1; i <= 44; ++i)
        {
            var xyAndCondition = conditions.Single(x =>
                x.Value.Source.Contains($"x{i:00}")
                && x.Value.Source.Contains($"y{i:00}")
                && x.Value.Source.Contains($"AND"));

            var target = xyAndCondition.Key;
            var connectedConditions = conditions.Where(x => x.Value.Left == target || x.Value.Right == target).ToArray();
            foreach (var c in connectedConditions)
            {
                if (c.Value.Op != "OR") stranges.Add(target);
            }
        }

        for (var i = 0; i <= 44; ++i)
        {
            var zCondition = conditions.Single(x=>x.Key == $"z{i:00}");
            if (zCondition.Value.Op != "XOR")
            {
                stranges.Add(zCondition.Key);
                
                var xyXorCondition = conditions.Single(x => x.Value.Source.Contains($"x{i:00}")
                                                            && x.Value.Source.Contains($"y{i:00}")
                                                            && x.Value.Source.Contains($"XOR"));
                var target = xyXorCondition.Key;
            
                if (target.StartsWith("z")) continue;
            
                var targetXorCondition = conditions.SingleOrDefault(x =>
                    x.Value.Op == "XOR" && (x.Value.Right == target || x.Value.Left == target));

                if (targetXorCondition.Key != null)
                    stranges.Add(targetXorCondition.Key);
            }
        }

        var resultCount = stranges.Count;
        //cdj,dhm,gfm,mrb,qjd,z08,z16,z32

        stranges.OrderBy(x => x).JoinToString(",").Should().Be(expected);
        
        // foreach (var condition in conditions)
        // {
        //     var cond = condition.Value;
        //     var result = condition.Key;
        //     if (cond.S)
        //     {
        //         
        //     }
        // }
        

        // input = Swap(input, "-> z08", "-> cdj");
        // input = Swap(input, "-> z32", "-> qfm");
        // input = Swap(input, "-> qjd", "-> dhm");
        // input = Swap(input, "-> qjd", "-> dhm");
        //
        // for (var i = 1; i <= 44; ++i)
        // {
        //     var zLeftCond = conditions.Single(x =>
        //         x.Value.Source.Contains($"x{i:00}")
        //         && x.Value.Source.Contains($"y{i:00}")
        //         && x.Value.Source.Contains($"XOR")
        //     );
        //
        //     input = input.Replace(zLeftCond.Key, $"xyz{i:00}");
        // }
        //
        // ParseInput(input, out _, out conditions);
        // foreach (var condition in conditions)
        // {
        //     if (condition.Key.StartsWith("z") && !condition.Value.Source.Contains($"xyz"))
        //     {
        //         var error = "";
        //     }
        // }
        //
        // var values = new Dictionary<string, bool>();
        // var xStr = "11111111111101111111110111111111110110111111";
        // var yStr = "01011100011111111011111111011101110111100011";
        //
        // var x = xStr.ToLongFromBin();
        // var y = xStr.ToLongFromBin();
        //
        // for (var i = 0; i < 44; ++i)
        // {
        //     values[$"x{i:00}"] = xStr[43 - i] == '1';
        //     values[$"y{i:00}"] = yStr[43 - i] == '1';
        // }
        //
        // // input = Swap(input, "-> z08", "-> cdj");
        // // input = Swap(input, "-> z32", "-> qfm");
        // // input = Swap(input, "-> qjd", "-> dhm");
        //
        // var candidates = conditions.Select(x => x.Key).Except(new[] { "z08", "cdj", "z32", "qfm", "qjd", "dhm", "z16", "mrb" })
        //     .ToArray();
        //
        // for (var i = 0; i < candidates.Length; i++)
        // for (var j = i + 1; j < candidates.Length; j++)
        // {
        //     var candidate1 = candidates[i];
        //     var candidate2 = candidates[j];
        //     
        //     var newInput = Swap(input, candidate1, candidate2);
        //     ParseInput(newInput, out _, out conditions);
        //     
        //     try
        //     {
        //         var result = new StringBuilder();
        //
        //         foreach (var z in zs)
        //         {
        //             result.Append(GetValue(z, conditions, values) ? '1' : '0');
        //         }
        //
        //         var resultStr = new string(result.ToString().Reverse().ToArray());
        //         var resultLong = resultStr.ToLongFromBin();
        //
        //         if (resultLong == x + y)
        //         {
        //             var ss = "";
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }
        // }


        //resultLong.Should().Be(expected);
    }

    private static string Swap(string src, string left, string right)
    {
        var tmp = Guid.NewGuid().ToString();
        return src.Replace(left, tmp).Replace(right, left).Replace(tmp, right);
    }

    private static void ParseInput(string input, out Dictionary<string, bool> values,
        out Dictionary<string, (string Left, string Right, string Op, string Source)> conditions)
    {
        values = new Dictionary<string, bool>();
        conditions = new Dictionary<string, (string Left, string Right, string Op, string Source)>();

        foreach (var line in input.SplitLines())
        {
            if (line.Contains(':'))
            {
                var splits = line.SplitEmpty(": ");
                values[splits[0]] = int.Parse(splits[1]) == 1;
            }
            else
            {
                var splits = line.Split(" -> ");
                var pairsInput = splits[0].SplitEmpty(" ");

                conditions.Add(splits[1], (pairsInput[0], pairsInput[2], pairsInput[1], splits[0]));
            }
        }
    }

    private static int maxLevel = 0;

    private bool GetValue(string s,
        Dictionary<string, (string Left, string Right, string Op, string Source)> conditions,
        Dictionary<string, bool> values, int level = 0)
    {
        level += 1;

        if (level > 50) throw new Exception("deep");

        if (maxLevel < level) maxLevel = level;

        if (values.TryGetValue(s, out var r)) return r;

        var condition = conditions[s];

        switch (condition.Op)
        {
            case "AND":
                return GetValue(condition.Left, conditions, values, level) &&
                       GetValue(condition.Right, conditions, values, level);
            case "OR":
                return GetValue(condition.Left, conditions, values, level) ||
                       GetValue(condition.Right, conditions, values, level);
            case "XOR":
                return GetValue(condition.Left, conditions, values, level) ^
                       GetValue(condition.Right, conditions, values, level);
            default: throw new Exception("dsf");
        }
    }

    private string GetStringValue(string s, Dictionary<string, (string Left, string Right, string Op)> conditions,
        Dictionary<string, bool> values)
    {
        if (values.TryGetValue(s, out _)) return s;

        var condition = conditions[s];

        switch (condition.Op)
        {
            case "AND":
                return " ( " + GetStringValue(condition.Left, conditions, values
                ) + " && " + GetStringValue(condition.Right, conditions, values
                ) + " ) ";
            case "OR":
                return " ( " + GetStringValue(condition.Left, conditions, values
                ) + " || " + GetStringValue(condition.Right, conditions, values
                ) + " ) ";
            case "XOR":
                return " ( " + GetStringValue(condition.Left, conditions, values
                ) + " ^^ " + GetStringValue(condition.Right, conditions, values
                ) + " ) ";
            default: throw new Exception("dsf");
        }
    }
}