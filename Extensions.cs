﻿using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace AoC_2024;

public static class Extensions
{
    public static int ToIntFromBin(this string src)
    {
        return Convert.ToInt32(src, 2);
    }

    public static long ToLongFromBin(this string src)
    {
        return Convert.ToInt64(src, 2);
    }

    private static readonly Dictionary<char, string> bin = new Dictionary<char, string>
    {
        { '0', "0000" },
        { '1', "0001" },
        { '2', "0010" },
        { '3', "0011" },
        { '4', "0100" },
        { '5', "0101" },
        { '6', "0110" },
        { '7', "0111" },
        { '8', "1000" },
        { '9', "1001" },
        { 'A', "1010" },
        { 'B', "1011" },
        { 'C', "1100" },
        { 'D', "1101" },
        { 'E', "1110" },
        { 'F', "1111" },
    };

    public static string ToBinFromHex(this string src)
    {
        var sb = new StringBuilder();

        foreach (var c in src)
        {
            sb.Append(bin[c]);
        }

        return sb.ToString().TrimStart('0');
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> ts)
    {
        return new HashSet<T>(ts);
    }

    public static int MaxOrDefault<T>(this IEnumerable<T> src, Func<T, int> pred)
    {
        var array = src.ToArray();
        return !array.Any() ? 0 : array.Max(pred);
    }

    public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val = default)
    {
        return dict.TryGetValue(key, out var v) ? v : val;
    }

    public static string[] SplitEmpty(this string str, params string[] separators)
    {
        return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] SplitLines(this string str)
    {
        return str.SplitEmpty("\r", "\n");
    }

    public static IEnumerable<(T Item, (int Row, int Col) Index)> GetAllNeighbours<T>(T[][] map,
        (int Row, int Col) src)
    {
        for (var i = -1; i <= 1; ++i)
        for (var j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0) continue;

            var row = src.Row + i;
            var col = src.Col + j;

            if (row < 0 || col < 0 || row >= map.Length || map.Length == 0 || col >= map[0].Length) continue;

            yield return (map[row][col], (row, col));
        }
    }

    public static IEnumerable<T[]> GetAllNeighbours<T>(T[][] map,
        (int Row, int Col) src, int depth)
    {
        for (var i = -1; i <= 1; ++i)
        for (var j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0) continue;

            var result = new List<T>(depth);

            for (var d = 0; d < depth; ++d)
            {
                var newI = i + i * d;
                var newJ = j + j * d;

                var row = src.Row + newI;
                var col = src.Col + newJ;

                if (row < 0 || col < 0 || row >= map.Length || map.Length == 0 || col >= map[0].Length) continue;

                result.Add(map[row][col]);
            }

            yield return result.ToArray();
        }
    }

    public static IEnumerable<(T Item, (int Row, int Col) Index)> GetVerticalHorizontalNeighbours<T>(T[][] map,
        (int Row, int Col) src)
    {
        for (var i = -1; i <= 1; ++i)
        for (var j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0) continue;
            if (j != 0 && i != 0) continue;

            var row = src.Row + i;
            var col = src.Col + j;

            if (row < 0 || col < 0 || row >= map.Length || map.Length == 0 || col >= map[0].Length) continue;

            yield return (map[row][col], (row, col));
        }
    }

    public static IEnumerable<(T Item, (int Row, int Col) Index)> GetDiagonalNeighbours<T>(T[][] map,
        (int Row, int Col) src)
    {
        for (var i = -1; i <= 1; ++i)
        for (var j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0) continue;
            if (j == 0 || i == 0) continue;

            var row = src.Row + i;
            var col = src.Col + j;

            if (row < 0 || col < 0 || row >= map.Length || map.Length == 0 || col >= map[0].Length) continue;

            yield return (map[row][col], (row, col));
        }
    }

    public static IEnumerable<(T Item, (int Row, int Col) Index, (int, int) Direction)>
        GetVerticalHorizontalNeighboursDirections<T>(T[][] map,
            (int Row, int Col) src)
    {
        for (var i = -1; i <= 1; ++i)
        for (var j = -1; j <= 1; j++)
        {
            if (j == 0 && i == 0) continue;
            if (j != 0 && i != 0) continue;

            var row = src.Row + i;
            var col = src.Col + j;

            if (row < 0 || col < 0 || row >= map.Length || map.Length == 0 || col >= map[0].Length) continue;

            yield return (map[row][col], (row, col), (i, j));
        }
    }

    public static (T Item, (int Row, int Col) Index) First<T>(T[][] map, T val)
    {
        for (var i = 0; i < map.Length; ++i)
        for (var j = 0; j < map[i].Length; ++j)
        {
            if (Equals(val, map[i][j])) return (val, (i, j));
        }

        throw new Exception("Value not found");
    }

    public static string JoinToString<T>(this IEnumerable<T> items, string separator = "")
    {
        return string.Join(separator, items);
    }

    public static string GetLastOrEmpty(this string src)
    {
        return src.Length > 0 ? src.Last().ToString() : string.Empty;
    }

    public static T SafeGet<T>(this T[][] map, int row, int col)
    {
        if (row < 0 || col < 0 || row >= map.Length || col >= map[row].Length) return default;
        return map[row][col];
    }

    public static T SafeGet<T>(this T[][] map, Point point)
    {
        var row = point.Row;
        var col = point.Col;
        if (row < 0 || col < 0 || row >= map.Length || col >= map[row].Length) return default;
        return map[row][col];
    }

    public static T Get<T>(this T[][] map, Point point)
    {
        return map[point.Row][point.Col];
    }

    public static readonly (int Row, int Col) DownStep = (1, 0);
    public static readonly (int Row, int Col) LeftStep = (0, -1);
    public static readonly (int Row, int Col) UpStep = (-1, 0);
    public static readonly (int Row, int Col) RightStep = (0, 1);

    public static string PrintMap<T>(T[][] map, Func<T, string> printFunc = null)
    {
        var sb = new StringBuilder();

        foreach (var t in map)
        {
            foreach (var c in t)
            {
                sb.Append(printFunc != null ? printFunc(c) : c.ToString());
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string PrintMap<T>(T[][] map, Func<T, Point, string> printFunc)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < map.Length; i++)
        {
            for (var j = 0; j < map[i].Length; j++)
            {
                var c = map[i][j];
                sb.Append(printFunc != null ? printFunc(c, (i, j)) : c.ToString());
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static Point Find<T>(this T[][] map, T item)
    {
        for (var i = 0; i < map.Length; i++)
        for (var j = 0; j < map[i].Length; j++)
        {
            if (Equals(map[i][j], item)) return new Point(i, j);
        }

        throw new Exception("Value not found");
    }

    public static long ToLong(this int src)
    {
        return src;
    }

    public static List<List<T>> Permute<T>(T[] nums)
    {
        var list = new List<List<T>>();
        return DoPermute(nums, 0, nums.Length - 1, list);
    }

    public static List<List<T>> DoPermute<T>(T[] nums, int start, int end, List<List<T>> list)
    {
        if (start == end)
        {
            list.Add([..nums]);
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                Swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                Swap(ref nums[start], ref nums[i]);
            }
        }

        return list;
    }

    static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
}

[DebuggerDisplay("X:{X},Y:{Y}")]
public struct Point : IEquatable<Point>
{
    public Point()
    {
    }

    public Point(int Row, int Col)
    {
        this.Row = Row;
        this.Col = Col;
    }

    public int Col;
    public int Row;

    public int X
    {
        get => Col;
        set => Col = value;
    }

    public int Y
    {
        get => Row;
        set => Row = value;
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.Y + p2.Y, p1.X + p2.X);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.Y - p2.Y, p1.X - p2.X);
    }

    public static implicit operator (int Row, int Col)(Point p)
    {
        return (p.Row, p.Col);
    }

    public static implicit operator Point((int Row, int Col) p)
    {
        return new Point(p.Row, p.Col);
    }

    public bool Equals(Point other)
    {
        return Col == other.Col && Row == other.Row;
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Col, Row);
    }
}