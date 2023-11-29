namespace AndrejKrizan.DotNet.Randoms;
public static class RandomExtensions
{
    public static bool NextBool(this Random random, double truePercentage = 50f)
        => random.NextDouble() * 100d < truePercentage;

    #region NextSet
    /// <returns>A set of random integers from [<paramref name="min"/>, <paramref name="max"/>) range.</returns>
    /// <remarks>Works in O(<paramref name="size"/>) complexity.</remarks>
    public static HashSet<int> NextSet(this Random random, int size, int min = 0, int max = int.MaxValue)
    {
        HashSet<int> set = new();
        for (int limit = max - size + 1; limit <= max; limit++)
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

    /// <returns>A random sized set of random integers from [<paramref name="min"/>, <paramref name="max"/>) range.</returns>
    public static HashSet<int> NextSet(this Random random, int min = 0, int max = int.MaxValue)
        => random.NextSet(random.Next(0, max - min + 1), min, max);
    #endregion
}
