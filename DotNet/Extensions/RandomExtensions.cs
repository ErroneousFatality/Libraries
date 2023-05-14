namespace AndrejKrizan.DotNet.Extensions;
public static class RandomExtensions
{
    public static bool NextBool(this Random random, double truePercentage = 50f)
        => random.NextDouble() * 100d < truePercentage;

    /// <remarks>Works in O(count) complexity.</remarks>
    public static HashSet<int> NextSet(this Random random, int count, int min, int max)
    {
        HashSet<int> set = new();
        for (int limit = max - count + 1; limit <= max; limit++)
        {
            int value = random.Next(min, limit);
            if (set.Contains(value))
            {
                set.Add(limit);
            }
            else
            {
                set.Add(value);
            }
        }
        return set;
    }
}
