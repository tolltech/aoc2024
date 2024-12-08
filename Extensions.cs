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
        
        public static IEnumerable<(T Item, (int Row, int Col) Index, (int, int) Direction)> GetVerticalHorizontalNeighboursDirections<T>(T[][] map,
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
                if (Equals(val, map[i][j])) return (val, (i,j));
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
    }

    public struct Point2
    {
        public Point2()
        {
            
        }
        
        public Point2(int Row, int Col)
        {
            this.Row = Row;
            this.Col = Col;
        }
        
        public int Col;
        public int Row;
    }
